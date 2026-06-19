using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Tilemaps;

public class Doorinstall : MonoBehaviour
{
    [Header("설정")]
    [SerializeField] private Vector2Int wallDirection;
    [SerializeField] private GameObject doorPrefab; // 소환할 문 프리팹

    private DynamicRoom parentRoom;
    private RoomManager1 roomManager;
    private bool isPlayerNearby = false;
    private GameObject instantiatedDoor; // 현재 생성된 문 오브젝트
    private Transform playerTransform;

    private void Start()
    {
        roomManager = FindFirstObjectByType<RoomManager1>();

        // 부모를 확실하게 찾기 위한 로직
        parentRoom = GetComponentInParent<DynamicRoom>();

        // 만약 그래도 null이라면? 같은 프리팹 안에 있는걸 보장하기 위해
        if (parentRoom == null)
        {
            parentRoom = GetComponent<DynamicRoom>(); // 자기 자신도 확인
        }

        if (parentRoom == null)
        {
            Debug.LogError($"{gameObject.name}이 DynamicRoom을 찾지 못했습니다! 프리팹 구성을 확인하세요.");
        }
    }
    public void SetParentRoom(DynamicRoom room)
    {
        this.parentRoom = room;
        Debug.Log($"{gameObject.name}의 부모 방이 {room.name}(으)로 설정되었습니다.");
    }


    private void Update()
    {
        if (isPlayerNearby && Keyboard.current.eKey.wasPressedThisFrame)
        {
            Debug.Log("E키가 눌렸습니다. 현재 문 상태: " + (instantiatedDoor == null ? "없음" : "있음"));
            if (instantiatedDoor == null)
            {
                SpawnDoor(); // 1단계: 문 프리팹 소환
            }
            else
            {
                OpenDoorAndProceed(); // 2단계: 문 열고 다음 방으로
            }
        }
    }

    private void SpawnDoor()
    {
        instantiatedDoor = Instantiate(doorPrefab, transform.position, Quaternion.identity);
    }

    private void OpenDoorAndProceed()
    {
        // 실제 방(DynamicRoom)을 찾기 위해 자기 자신부터 상위로 탐색합니다.
        if (parentRoom == null)
        {
            Debug.LogError($"{gameObject.name}: parentRoom이 비어있습니다! 방 생성시 RoomManager가 제대로 주입했는지 확인하세요.");
            return;
        }

        // [최후의 수단] 그래도 null이라면? (방 프리팹 구조가 복잡한 경우)
        // 씬 전체에서 이 문과 가장 가까운 DynamicRoom을 찾습니다.
        if (parentRoom == null)
        {
            DynamicRoom[] allRooms = FindObjectsByType<DynamicRoom>(FindObjectsSortMode.None);
            float minDistance = float.MaxValue;

            foreach (var room in allRooms)
            {
                float dist = Vector3.Distance(transform.position, room.transform.position);
                if (dist < minDistance)
                {
                    minDistance = dist;
                    parentRoom = room;
                }
            }
        }

        // 이제 진짜로 없으면 에러 발생
        if (parentRoom == null)
        {
            Debug.LogError($"{gameObject.name}이 소속된 방(DynamicRoom)을 찾을 수 없습니다!");
            return;
        }

        // [연출 및 이동]
        TilemapRenderer tr = instantiatedDoor.GetComponentInChildren<TilemapRenderer>();
        if (tr != null) tr.enabled = false;
        Collider2D col = instantiatedDoor.GetComponentInChildren<Collider2D>();
        if (col != null) col.enabled = false;

        roomManager.BuildRoomAndTeleport(parentRoom, wallDirection, playerTransform);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player")) { isPlayerNearby = true; playerTransform = collision.transform; }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player")) isPlayerNearby = false;
    }

}