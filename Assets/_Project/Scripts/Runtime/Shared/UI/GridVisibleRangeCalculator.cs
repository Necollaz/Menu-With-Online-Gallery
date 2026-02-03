using UnityEngine;
using UnityEngine.UI;

namespace MenuWithOnlineGallery.Gallery.Layout
{
    public sealed class GridVisibleRangeCalculator
    {
        private const float MIN_VALID_SIZE = 0.01f;
        private const float ZERO = 0f;

        public VisibleIndexRange Calculate(RectTransform viewportRectTransform, RectTransform contentRectTransform,
            GridLayoutGroup gridLayoutGroup, int totalItemsCount, int bufferRows)
        {
            if (viewportRectTransform == null || contentRectTransform == null || gridLayoutGroup == null)
                return new VisibleIndexRange(-1, -1);

            if (totalItemsCount <= 0)
                return new VisibleIndexRange(-1, -1);

            int colums = Mathf.Max(1, gridLayoutGroup.constraintCount);
            float cellHeight = gridLayoutGroup.cellSize.y;
            float spacingY = gridLayoutGroup.spacing.y;
            float rowHeight = cellHeight + spacingY;

            if (rowHeight <= MIN_VALID_SIZE)
                return new VisibleIndexRange(0, totalItemsCount - 1);

            float viewportHeight = viewportRectTransform.rect.height;

            if (viewportHeight <= MIN_VALID_SIZE)
                return new VisibleIndexRange(0, totalItemsCount - 1);

            float contentTopPadding = gridLayoutGroup.padding.top;
            float contentOffsetY = Mathf.Max(ZERO, contentRectTransform.anchoredPosition.y);

            float visibleTopY = contentOffsetY - contentTopPadding;
            float visibleBottomY = visibleTopY + viewportHeight;

            int firstVisibleRow = Mathf.FloorToInt(visibleTopY / rowHeight);
            int lastVisibleRow = Mathf.FloorToInt(visibleBottomY / rowHeight);

            int bufferedFirstRow = Mathf.Max(0, firstVisibleRow - Mathf.Max(0, bufferRows));
            int bufferedLastRow = Mathf.Max(0, lastVisibleRow + Mathf.Max(0, bufferRows));

            int startIndex = bufferedFirstRow * colums;
            int endIndex = ((bufferedLastRow + 1) * colums) - 1;

            startIndex = Mathf.Clamp(startIndex, 0, totalItemsCount - 1);
            endIndex = Mathf.Clamp(endIndex, 0, totalItemsCount - 1);

            if (endIndex < startIndex)
                return new VisibleIndexRange(-1, -1);

            return new VisibleIndexRange(startIndex, endIndex);
        }
    }
}