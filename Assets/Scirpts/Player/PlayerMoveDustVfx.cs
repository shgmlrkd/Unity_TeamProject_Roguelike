using UnityEngine;

public class PlayerMoveDustVfx : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private CharacterInputManager inputManager;
    [SerializeField] private Transform dustTransform;
    [SerializeField] private GameObject dustObject;
    [SerializeField] private SpriteRenderer dustSpriteRenderer;

    [Header("Position")]
    [SerializeField] private float sideDistance = 0.35f;
    [SerializeField] private Vector3 positionOffset = new Vector3(0f, -0.25f, 0f);

    [Header("Option")]
    [SerializeField] private bool flipByDirection = true;
    //최소 설정
    [SerializeField] private float minMoveInput = 0.01f;
    [SerializeField] private float minHorizontalInput = 0.01f;

    private bool isDustActive;

    // 1 = 오른쪽에 먼지, -1 = 왼쪽에 먼지
    private int dustSide = -1;

    private void Awake()
    {
        if (inputManager == null)
        {
            inputManager = GetComponent<CharacterInputManager>();
        }

        if (dustTransform == null && dustObject != null)
        {
            dustTransform = dustObject.transform;
        }

        if (dustObject == null && dustTransform != null)
        {
            dustObject = dustTransform.gameObject;
        }

        if (dustSpriteRenderer == null && dustObject != null)
        {
            dustSpriteRenderer = dustObject.GetComponentInChildren<SpriteRenderer>();
        }

        SetDustActive(false);
    }

    private void Update()
    {
        if (inputManager == null || !inputManager.enabled)
        {
            SetDustActive(false);
            return;
        }

        Vector2 moveInput = inputManager.MoveInput;

        if (moveInput.sqrMagnitude <= minMoveInput * minMoveInput)
        {
            SetDustActive(false);
            return;
        }

        // 위/아래 방향이 45도 초과하면 먼지 출력 안 함
        if (Mathf.Abs(moveInput.y) > Mathf.Abs(moveInput.x))
        {
            SetDustActive(false);
            return;
        }

        UpdateDustSide(moveInput.x);
        UpdateDustPosition();
        UpdateDustFlip();

        SetDustActive(true);
    }

    private void UpdateDustSide(float moveX)
    {
        if (moveX > minHorizontalInput)
        {
            // 오른쪽으로 이동 중이면 먼지는 왼쪽
            dustSide = -1;
        }
        else if (moveX < -minHorizontalInput)
        {
            // 왼쪽으로 이동 중이면 먼지는 오른쪽
            dustSide = 1;
        }

        // moveX가 0에 가까운 위/아래 이동이면
        // 기존 dustSide 유지
    }

    private void UpdateDustPosition()
    {
        if (dustTransform == null)
        {
            return;
        }

        float xPosition = dustSide * sideDistance;

        dustTransform.localPosition = new Vector3(
            xPosition + positionOffset.x,
            positionOffset.y,
            positionOffset.z
        );

        // 회전 금지
        dustTransform.localRotation = Quaternion.identity;
    }

    private void UpdateDustFlip()
    {
        if (!flipByDirection || dustSpriteRenderer == null)
        {
            return;
        }

        // 먼지가 오른쪽에 있을 때 좌우 반전
        dustSpriteRenderer.flipX = dustSide < 0;
    }

    private void SetDustActive(bool active)
    {
        if (isDustActive == active)
        {
            return;
        }

        isDustActive = active;

        if (dustObject != null)
        {
            dustObject.SetActive(active);
        }
    }
}