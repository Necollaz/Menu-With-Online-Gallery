using UnityEngine;
using UnityEngine.UI;

public sealed class BannerCarouselReferences
{
    private readonly MonoBehaviour _owner;

    private ScrollRect _scrollRect;
    private RectTransform _viewportRectTransform;

    public BannerCarouselReferences(MonoBehaviour owner, ref ScrollRect scrollRect, ref RectTransform viewportRectTransform)
    {
        _owner = owner;
        _scrollRect = scrollRect;
        _viewportRectTransform = viewportRectTransform;
    }
    
    public RectTransform ContentRectTransform { get; private set; }
    public RectTransform ViewportRectTransform => _viewportRectTransform;

    public void TryBind()
    {
        if (_scrollRect == null)
        {
            _scrollRect = _owner.GetComponent<ScrollRect>();
        }

        ContentRectTransform = _scrollRect != null ? _scrollRect.content : null;

        if (_viewportRectTransform == null && _scrollRect != null)
        {
            _viewportRectTransform = _scrollRect.viewport;
        }
    }
}