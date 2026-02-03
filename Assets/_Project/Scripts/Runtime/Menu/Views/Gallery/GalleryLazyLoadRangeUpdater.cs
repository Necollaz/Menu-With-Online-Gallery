using System;
using UnityEngine;
using UnityEngine.UI;
using MenuWithOnlineGallery.Gallery.Layout;
using MenuWithOnlineGallery.MenuScreen;

namespace MenuWithOnlineGallery.Gallery
{
    public sealed class GalleryLazyLoadRangeUpdater
    {
        private const int INVALID_INDEX = -1;

        private readonly GridVisibleRangeCalculator _visibleRangeCalculator;
        private readonly MenuScreenConfigAsset _configAsset;
        private readonly RectTransform _viewport;
        private readonly RectTransform _content;
        private readonly GridLayoutGroup _gridLayoutGroup;

        private readonly Func<int> _getTotalItemsCount;
        private readonly Action<int, bool, bool> _setItemLoadingEnabledByIndex;

        private VisibleIndexRange _lastRange = new VisibleIndexRange(INVALID_INDEX, INVALID_INDEX);

        public GalleryLazyLoadRangeUpdater(
            GridVisibleRangeCalculator visibleRangeCalculator,
            MenuScreenConfigAsset configAsset, 
            RectTransform viewport,
            RectTransform content,
            GridLayoutGroup gridLayoutGroup,
            Func<int> getTotalItemsCount,
            Action<int, bool, bool> setItemLoadingEnabledByIndex)
        {
            _visibleRangeCalculator = visibleRangeCalculator;
            _configAsset = configAsset;
            _viewport = viewport;
            _content = content;
            _gridLayoutGroup = gridLayoutGroup;
            _getTotalItemsCount = getTotalItemsCount;
            _setItemLoadingEnabledByIndex = setItemLoadingEnabledByIndex;
        }

        public void Reset()
        {
            _lastRange = new VisibleIndexRange(INVALID_INDEX, INVALID_INDEX);
        }

        public void ForceUpdate()
        {
            _lastRange = new VisibleIndexRange(INVALID_INDEX, INVALID_INDEX);

            UpdateIfNeeded();
        }

        public void UpdateIfNeeded()
        {
            if (_viewport == null || _content == null || _gridLayoutGroup == null)
                return;

            if (_visibleRangeCalculator == null || _getTotalItemsCount == null || _setItemLoadingEnabledByIndex == null)
                return;

            int totalItemsCount = _getTotalItemsCount.Invoke();
            
            if (totalItemsCount <= 0)
                return;

            int bufferRows = _configAsset != null ? _configAsset.LazyLoadBufferRows : 0;

            VisibleIndexRange range = _visibleRangeCalculator.Calculate(_viewport, _content, _gridLayoutGroup,
                totalItemsCount, bufferRows);

            if (!range.IsValid)
                return;

            if (_lastRange.IsValid && _lastRange.StartIndex == range.StartIndex &&
                _lastRange.EndIndex == range.EndIndex)
            {
                return;
            }

            _lastRange = range;

            bool cancelOutsideRange = _configAsset != null && _configAsset.CancelLoadOutsideRange;

            for (int i = 0; i < totalItemsCount; i++)
            {
                bool shouldLoad = range.Contains(i);
                _setItemLoadingEnabledByIndex.Invoke(i, shouldLoad, cancelOutsideRange);
            }
        }
    }
}