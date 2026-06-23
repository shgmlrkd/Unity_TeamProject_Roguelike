using UnityEngine;
using UnityEngine.UI;

public class EquipmentImageManager : MonoBehaviour
{
    [SerializeField]
    private Image equipmentImagePrefab;

    private const int poolSize = 10;

    private void Awake()
    {
        PoolManager.Instance.SetCreatePool(transform, true);
        PoolManager.Instance.PreloadPool(equipmentImagePrefab, poolSize);
    }

    public Image EquipmentImagePop()
    {
        return PoolManager.Instance.GetPool(equipmentImagePrefab);
    }

    public void ReturnEquipmentImage(Image image)
    {
        PoolManager.Instance.ReturnPool(image);
    }
}