using UnityEngine;
using DG.Tweening;
using System;
using Unity.VisualScripting;

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
}
