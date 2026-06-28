using UnityEngine;

public class Door : MonoBehaviour
{
    [Header("문 방향 데이터")]
    public Vector2Int doorDirection;

    [Header("순간이동 좌표 세팅")]
    [SerializeField] private float teleportDistance = 3f;

    [Header("자식 오브젝트 바인딩")]
    [SerializeField] private GameObject closeDoorObject;
    [SerializeField] private GameObject openDoorObject;

    public bool isOpened { get; private set; } = false;
    private RoomManager roomManager;

    private void Start()
    {
        roomManager = FindFirstObjectByType<RoomManager>();
        CloseDoor();
    }
    public void OpenDoor()
    {
        isOpened = true;
        if (closeDoorObject != null) closeDoorObject.SetActive(false);
        if (openDoorObject != null) openDoorObject.SetActive(true);
    }
    public void CloseDoor()
    {
        isOpened = false;
        if (closeDoorObject != null) closeDoorObject.SetActive(true);
        if (openDoorObject != null) openDoorObject.SetActive(false);
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && isOpened && roomManager != null)
        {
            float w = roomManager.roomWidth;
            float h = roomManager.roomHeight;

            Vector3 targetPos = new Vector3(doorDirection.x * w, doorDirection.y * h, 0)
                              + new Vector3(doorDirection.x * teleportDistance, doorDirection.y * teleportDistance, 0);

            collision.transform.position += targetPos;
        }
    }
}