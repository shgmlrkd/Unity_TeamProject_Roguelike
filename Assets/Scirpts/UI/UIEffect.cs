using UnityEngine;

public class UIEffect : MonoBehaviour
{
    [Header("설정")]
    [SerializeField] private float minScale = 0.01f;
    [SerializeField] private float maxScale = 0.015f;
    [SerializeField] private float speed = 2.0f;

    private RectTransform rectTransform;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
    }
    void Update()
    {
        float scale = Mathf.Lerp(minScale, maxScale, (Mathf.Sin(Time.time * speed) + 1f) / 2f);

        rectTransform.localScale = new Vector3(scale, scale, 1f);
    }
}
