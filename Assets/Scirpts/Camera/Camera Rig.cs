using UnityEngine;
using DG.Tweening;
using System;

public class CameraRig : MonoBehaviour
{
    [SerializeField] private Vector3 offset = new Vector3(0f, 0f, -10f);

    public event Action OnCameraMoveStarted;
    public event Action OnCameraMoveFinished;

    public void MoveToRoom(Vector3 targetPos, float duration)
    {
        OnCameraMoveStarted?.Invoke();

        targetPos.z = offset.z;

        transform.DOKill();
        transform.DOMove(targetPos, duration)
            .SetEase(Ease.InOutCubic)
            .OnComplete(() =>
            {
                DOVirtual.DelayedCall(0.2f, () =>
                {
                    OnCameraMoveFinished?.Invoke();
                });
            });
    }

    public void MoveToRoom(Vector3 startPos, Vector3 targetPos, float startDuration, float targetDuration)
    {
        OnCameraMoveStarted?.Invoke();

        startPos.z = offset.z;
        targetPos.z = offset.z;

        Sequence sequence = DOTween.Sequence();

        sequence.Append(transform.DOMove(startPos, startDuration)
            .SetEase(Ease.InOutCubic));

        sequence.AppendInterval(0.2f);

        sequence.Append(transform.DOMove(targetPos, targetDuration)
            .SetEase(Ease.InOutCubic));

        sequence.OnComplete(() =>
        {
            OnCameraMoveFinished?.Invoke();
        });
    }
}
