using UnityEngine;
using UnityEngine.Rendering;

public class YSortOrder : MonoBehaviour
{
    [SerializeField] private SortingGroup sortingGroup;
    [SerializeField] private float sortingScale = 100f;
    [SerializeField] private int sortingOffset = 0;

    private int previousSortingOrder;

    private void Awake()
    {
        if (sortingGroup == null)
        {
            sortingGroup = GetComponent<SortingGroup>();
        }

        UpdateSortingOrder(true);
    }

    private void LateUpdate()
    {
        UpdateSortingOrder(false);
    }

    private void UpdateSortingOrder(bool forceUpdate)
    {
        if (sortingGroup == null)
        {
            return;
        }
        //y값에 따른 Sorting Layer 통제
        int sortingOrder = Mathf.RoundToInt(-transform.position.y * sortingScale) + sortingOffset;

        if (!forceUpdate && previousSortingOrder == sortingOrder)
        {
            return;
        }

        previousSortingOrder = sortingOrder;
        sortingGroup.sortingOrder = sortingOrder;
    }
}