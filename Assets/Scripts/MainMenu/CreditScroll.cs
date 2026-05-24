using UnityEngine;

public class CreditScroll : MonoBehaviour
{
    public float speed = 50f;

    public float startY = -600f;
    public float endY = 800f;

    private RectTransform rect;

    private bool isScrolling = false;

    void Awake()
    {
        rect = GetComponent<RectTransform>();
    }

    void OnEnable()
    {
        ResetPosition();
        StartScroll();
    }

    void Update()
    {
        if (!isScrolling) return;

        rect.anchoredPosition += Vector2.up * speed * Time.deltaTime;

        if (rect.anchoredPosition.y >= endY)
        {
            ResetPosition();
        }
    }

    public void StartScroll()
    {
        isScrolling = true;
    }

    public void ResetPosition()
    {
        rect.anchoredPosition = new Vector2(0, startY);
    }

    public void StopScroll()
    {
        isScrolling = false;
    }
}