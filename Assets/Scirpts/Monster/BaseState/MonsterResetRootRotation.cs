using System.Collections;
using UnityEngine;

public class MonsterResetRootRotation : MonoBehaviour
{
    private Quaternion startRotation;

    private void Awake()
    {
        // 처음 프리팹 상태의 회전값 저장
        startRotation = transform.localRotation;
    }

    private void OnEnable()
    {
        // 켜지자마자 1차 초기화
        transform.localRotation = startRotation;

        // Animator가 같은 프레임에 다시 덮어쓸 수 있어서 한 프레임 뒤에도 초기화
        StartCoroutine(ResetAfterFrame());
    }

    private IEnumerator ResetAfterFrame()
    {
        // 현재 프레임이 끝날 때까지 기다림
        yield return null;

        // Animator가 한 번 적용된 뒤 다시 Root 회전 복구
        transform.localRotation = startRotation;
    }
}
