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

    // 장비 이미지 꺼내기
    public Image EquipmentImagePop()
    {
        return PoolManager.Instance.GetPool(equipmentImagePrefab);
    }

    // 장비 이미지 다시 넣기
    public void ReturnEquipmentImage(Image image)
    {
        PoolManager.Instance.ReturnPool(image);
    }
}