using System.Collections.Generic;
using UnityEngine;

public sealed class DotsIndicator
{
    private const int FIRST_INDEX = 0;

    private readonly RectTransform _container;
    private readonly DotView _dotPrefab;

    private readonly List<DotView> _dots = new List<DotView>();

    public DotsIndicator(RectTransform container, DotView dotPrefab)
    {
        _container = container;
        _dotPrefab = dotPrefab;
    }

    public void Rebuild(int count)
    {
        if (_container == null || _dotPrefab == null)
        {
            return;
        }

        int requiredCount = Mathf.Max(0, count);

        EnsureDotsCount(requiredCount);
        SetDotsVisibleCount(requiredCount);
        SetActiveIndex(FIRST_INDEX);
    }
    
    public void SetActiveIndex(int activeIndex)
    {
        if (_dots.Count == 0)
        {
            return;
        }

        activeIndex = Mathf.Clamp(activeIndex, 0, _dots.Count - 1);

        for (int i = 0; i < _dots.Count; i++)
        {
            DotView dotView = _dots[i];
            
            if (dotView == null || !dotView.gameObject.activeSelf)
            {
                continue;
            }

            dotView.SetActive(i == activeIndex);
        }
    }

    public void Dispose()
    {
        DestroyAllDots();
    }

    private void EnsureDotsCount(int requiredCount)
    {
        for (int i = _dots.Count; i < requiredCount; i++)
        {
            DotView dot = Object.Instantiate(_dotPrefab, _container);
            dot.SetActive(false);
            _dots.Add(dot);
        }
    }

    private void SetDotsVisibleCount(int visibleCount)
    {
        for (int i = 0; i < _dots.Count; i++)
        {
            DotView dotView = _dots[i];
            
            if (dotView == null)
            {
                continue;
            }

            bool shouldBeVisible = i < visibleCount;
            
            if (dotView.gameObject.activeSelf != shouldBeVisible)
            {
                dotView.gameObject.SetActive(shouldBeVisible);
            }

            if (shouldBeVisible)
            {
                dotView.SetActive(false);
            }
        }
    }

    private void DestroyAllDots()
    {
        for (int i = 0; i < _dots.Count; i++)
        {
            DotView dotView = _dots[i];
            
            if (dotView != null)
            {
                Object.Destroy(dotView.gameObject);
            }
        }

        _dots.Clear();
    }
}