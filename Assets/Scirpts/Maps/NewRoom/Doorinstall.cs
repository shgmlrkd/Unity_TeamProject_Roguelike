using UnityEngine;
using UnityEngine.InputSystem;

public class Doorinstall : MonoBehaviour
{
    public Vector2Int ParentRoomGridPos { get; set; }
    [Header("설정")]
    [SerializeField] private Vector2Int wallDirection; // 문이 어느 방향으로 방을 생성할지
    [SerializeField] private GameObject doorPrefab;    // 소환할 문 프리팹

    private GameObject instantiatedDoor;
    private bool isPlayerNearby = false;
    private Transform playerTransform;

    // 외부에서 방향을 가져가기 위한 프로퍼티
    public Vector2Int WallDirection => wallDirection;

    private void Update()
    {
        if (isPlayerNearby && Keyboard.current.eKey.wasPressedThisFrame)
        {
            if (instantiatedDoor == null)
            {
                SpawnDoor();
            }
            else
            {
                // RoomManager의 새로운 로직 호출 (parentRoom 제거)
                RoomManager1.Instance.BuildRoomFromDoor(this, playerTransform);
            }
        }
    }

    private void SpawnDoor()
    {
        instantiatedDoor = Instantiate(doorPrefab, transform.position, Quaternion.identity);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            isPlayerNearby = true;
            playerTransform = collision.transform;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            isPlayerNearby = false;
        }
    }
}
