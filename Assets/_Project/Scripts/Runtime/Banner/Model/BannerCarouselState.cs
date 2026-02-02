using UnityEngine;

public sealed class BannerCarouselState
{
    private const int MIN_SLIDES_FOR_SCROLL = 2;
    private const int FIRST_INDEX = 0;

    private const float ZERO = 0f;
    private const float ONE = 1f;

    private const float DEFAULT_THRESHOLD_FALLBACK = 1f;
    private const float THRESHOLD_STEP_FACTOR = 0.25f;

    private int _slidesCount;
    private int _currentIndex;

    private float _nextAutoScrollTime;
    private float _dragStartNormalized;
    private bool _isDragging;

    private float _autoScrollIntervalSeconds;
    private float _autoScrollResumeDelaySeconds;

    public int CurrentIndex => _currentIndex;
    public int SlidesCount => _slidesCount;

    public void Configure(float autoScrollIntervalSeconds, float autoScrollResumeDelaySeconds)
    {
        _autoScrollIntervalSeconds = Mathf.Max(ZERO, autoScrollIntervalSeconds);
        _autoScrollResumeDelaySeconds = Mathf.Max(ZERO, autoScrollResumeDelaySeconds);
    }

    public void SetSlidesCount(int slidesCount)
    {
        _slidesCount = Mathf.Max(0, slidesCount);

        if (_slidesCount == 0)
        {
            _currentIndex = FIRST_INDEX;
            
            return;
        }

        if (_currentIndex >= _slidesCount)
        {
            _currentIndex = Mathf.Max(FIRST_INDEX, _slidesCount - 1);
        }
    }

    public void ResetIndex()
    {
        _currentIndex = FIRST_INDEX;
    }

    public void ScheduleNextAutoScroll(float nowUnscaled)
    {
        _nextAutoScrollTime = nowUnscaled + _autoScrollIntervalSeconds;
    }

    public void DelayAutoScroll(float nowUnscaled)
    {
        _nextAutoScrollTime = nowUnscaled + _autoScrollResumeDelaySeconds;
    }

    public bool CanAutoScroll(float nowUnscaled)
    {
        if (_slidesCount < MIN_SLIDES_FOR_SCROLL)
        {
            return false;
        }

        if (_isDragging)
        {
            return false;
        }

        return nowUnscaled >= _nextAutoScrollTime;
    }

    public int GetNextAutoIndex()
    {
        if (_slidesCount < MIN_SLIDES_FOR_SCROLL)
        {
            return FIRST_INDEX;
        }

        return (_currentIndex + 1) % _slidesCount;
    }

    public void BeginDrag(float startNormalized)
    {
        _isDragging = true;
        _dragStartNormalized = startNormalized;
    }

    public void EndDrag()
    {
        _isDragging = false;
    }

    public int ResolveTargetIndexAfterDrag(float endNormalized)
    {
        if (_slidesCount < MIN_SLIDES_FOR_SCROLL)
        {
            return FIRST_INDEX;
        }
        
        float delta = endNormalized - _dragStartNormalized;
        float threshold = GetSwitchThresholdNormalized();

        int nearestIndex = GetNearestIndex(endNormalized);
        int targetIndex = nearestIndex;

        if (Mathf.Abs(delta) >= threshold)
        {
            if (delta > ZERO)
            {
                targetIndex = Mathf.Min(nearestIndex + 1, _slidesCount - 1);
            }
            else
            {
                targetIndex = Mathf.Max(nearestIndex - 1, FIRST_INDEX);
            }
        }

        return targetIndex;
    }

    public void SetCurrentIndex(int index)
    {
        if (_slidesCount == 0)
        {
            _currentIndex = FIRST_INDEX;
            
            return;
        }

        _currentIndex = Mathf.Clamp(index, FIRST_INDEX, _slidesCount - 1);
    }

    public float GetNormalizedPositionForIndex(int index)
    {
        if (_slidesCount < MIN_SLIDES_FOR_SCROLL)
        {
            return ZERO;
        }

        index = Mathf.Clamp(index, FIRST_INDEX, _slidesCount - 1);

        return (float)index / (_slidesCount - 1);
    }

    private int GetNearestIndex(float normalizedPosition)
    {
        if (_slidesCount < MIN_SLIDES_FOR_SCROLL)
        {
            return FIRST_INDEX;
        }

        float step = ONE / (_slidesCount - 1);
        int index = Mathf.RoundToInt(normalizedPosition / step);

        return Mathf.Clamp(index, FIRST_INDEX, _slidesCount - 1);
    }
    
    private float GetSwitchThresholdNormalized()
    {
        if (_slidesCount < MIN_SLIDES_FOR_SCROLL)
        {
            return DEFAULT_THRESHOLD_FALLBACK;
        }

        float step = ONE / (_slidesCount - 1);
        
        return step * THRESHOLD_STEP_FACTOR;
    }
}