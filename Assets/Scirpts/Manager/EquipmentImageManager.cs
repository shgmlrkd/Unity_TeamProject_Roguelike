using UnityEngine;
using UnityEngine.UI;

public class EquipmentImageManager : MonoBehaviour
{
    [SerializeField]
    private Image equipmentImage;

    private const int poolSize = 10;

    private void Awake()
    {
        PoolManager.Instance.SetCreatePool(transform, true);
        PoolManager.Instance.PreloadPool(equipmentImage, poolSize);
    }

    public Image EquipmentImagePop()
    {
        return PoolManager.Instance.GetPool(equipmentImage);
    }
}