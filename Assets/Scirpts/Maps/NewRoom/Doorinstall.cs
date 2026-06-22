using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;

public class Doorinstall : MonoBehaviour
{
    public Vector2Int ParentRoomGridPos { get; set; }
    public Vector2Int myRoomPos; // 이 문이 있는 방의 좌표
    public Vector2Int destinationRoomPos; // 이 문이 연결된 방의 좌표
    [Header("설정")]
    [SerializeField] private Vector2Int wallDirection; // 문이 어느 방향으로 방을 생성할지
    [SerializeField] private GameObject doorPrefab;    // 소환할 문 프리팹

    [Header("연출 설정")]
    [SerializeField] private float openDelay = 0.5f; // 문이 열리는 연출 시간

    private GameObject instantiatedDoor;
    private bool isPlayerNearby = false;
    private bool isProcessing = false; // 중복 실행 방지
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
                StartCoroutine(OpenAndTeleportRoutine());
            }
        }
    }

    private void SpawnDoor()
    {
        instantiatedDoor = Instantiate(doorPrefab, transform.position, Quaternion.identity);
    }
    private IEnumerator OpenAndTeleportRoutine()
    {
        isProcessing = true; // 처리 중 상태로 변경

        // 설정한 시간만큼 대기 (딜레이)
        yield return new WaitForSeconds(openDelay);

        // RoomManager의 로직 호출
        RoomManager1.Instance.BuildRoomFromDoor(this, playerTransform);

        isProcessing = false; // 다시 처리 가능 상태로 변경
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
    public void SetDestination(Vector2Int dest)
    {
        destinationRoomPos = dest;
    }
}
