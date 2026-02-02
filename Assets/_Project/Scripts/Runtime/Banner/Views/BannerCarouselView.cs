using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[RequireComponent(typeof(ScrollRect))]
public sealed class BannerCarouselView : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IPointerDownHandler
{
    [Header("Dots")]
    [SerializeField] private RectTransform _dotsContainer;
    [SerializeField] private DotView _dotPrefab;
    
    [Header("References")] 
    [SerializeField] private ScrollRect _scrollRect;
    [SerializeField] private RectTransform _viewportRectTransform;

    [Header("Settings")] 
    [SerializeField] private float _autoScrollIntervalSeconds = 5f;
    [SerializeField] private float _snapDurationSeconds = 0.25f;
    [SerializeField] private float _autoScrollResumeDelaySeconds = 1.0f;

    private readonly BannerCarouselState _state = new BannerCarouselState();
    
    private BannerCarouselReferences _references;
    private BannerSlidesResizer _slidesResizer;
    private BannerCarouselSnapper _snapper;
    private BannerCarouselAutoScroller _autoScroller;
    
    private DotsIndicator _dotsIndicator;
    
    private void Awake()
    {
        EnsureInitialized();

        _state.ResetIndex();
        
        SetNormalizedPosition(_state.GetNormalizedPositionForIndex(_state.CurrentIndex));

        _dotsIndicator = new DotsIndicator(_dotsContainer, _dotPrefab);
        _dotsIndicator.Rebuild(_state.SlidesCount);
        _dotsIndicator.SetActiveIndex(_state.CurrentIndex);
        
        _state.ScheduleNextAutoScroll(Time.unscaledTime);
    }

    private void OnEnable()
    {
        EnsureInitialized();
        RefreshSlidesCount();
        
        _slidesResizer.ResizeToViewport();
        _autoScroller.Start();
    }

    private void OnDisable()
    {
        _autoScroller?.Stop();
        _snapper?.Stop();
    }

    private void OnDestroy()
    {
        if (_dotsIndicator != null)
        {
            _dotsIndicator.Dispose();
            _dotsIndicator = null;
        }
    }

    void IPointerDownHandler.OnPointerDown(PointerEventData eventData)
    {
        _state.DelayAutoScroll(Time.unscaledTime);
    }

    void IBeginDragHandler.OnBeginDrag(PointerEventData eventData)
    {
        EnsureInitialized();

        _state.BeginDrag(_scrollRect.horizontalNormalizedPosition);
        _snapper.Stop();
    }

    void IDragHandler.OnDrag(PointerEventData eventData)
    {
        _state.DelayAutoScroll(Time.unscaledTime);
    }

    void IEndDragHandler.OnEndDrag(PointerEventData eventData)
    {
        _state.EndDrag();

        float endNormalized = _scrollRect.horizontalNormalizedPosition;
        int targetIndex = _state.ResolveTargetIndexAfterDrag(endNormalized);

        ScrollToIndex(targetIndex);
        
        _state.DelayAutoScroll(Time.unscaledTime);
    }

    private void OnRectTransformDimensionsChange()
    {
        _slidesResizer?.ResizeToViewport();
    }

    private void EnsureInitialized()
    {
        if (_references == null)
        {
            _references = new BannerCarouselReferences(this, ref _scrollRect, ref _viewportRectTransform);
        }

        _references.TryBind();

        if (_slidesResizer == null)
        {
            _slidesResizer = new BannerSlidesResizer(_references);
        }

        ConfigureState();
        RefreshSlidesCount();
        
        _slidesResizer.ResizeToViewport();

        if (_snapper == null)
        {
            _snapper = new BannerCarouselSnapper(this, _scrollRect, _snapDurationSeconds);
        }

        if (_autoScroller == null)
        {
            _autoScroller = new BannerCarouselAutoScroller(this, _state, ScrollToIndex);
        }
    }

    private void ConfigureState()
    {
        _state.Configure(_autoScrollIntervalSeconds, _autoScrollResumeDelaySeconds);
    }

    private void RefreshSlidesCount()
    {
        int slidesCount = _references.ContentRectTransform != null ? _references.ContentRectTransform.childCount : 0;
        _state.SetSlidesCount(slidesCount);

        if (_dotsIndicator != null)
        {
            _dotsIndicator.Rebuild(_state.SlidesCount);
            _dotsIndicator.SetActiveIndex(_state.CurrentIndex);
        }
    }

    private void ScrollToIndex(int index)
    {
        _state.SetCurrentIndex(index);

        if (_dotsIndicator != null)
        {
            _dotsIndicator.SetActiveIndex(_state.CurrentIndex);
        }

        float targetNormalized = _state.GetNormalizedPositionForIndex(_state.CurrentIndex);
        
        _snapper.Stop();
        _snapper.SnapTo(targetNormalized);
    }

    private void SetNormalizedPosition(float normalized)
    {
        if (_scrollRect == null)
        {
            return;
        }

        _scrollRect.horizontalNormalizedPosition = normalized;
    }
}