using UnityEngine;

namespace MenuWithOnlineGallery.BannerCarousel
{
    public sealed class BannerSlidesResizer
    {
        private const float MIN_VALID_SIZE = 0.01f;
        private const float SIZE_COMPARE_EPSILON = 0.1f;
    
        private readonly BannerCarouselReferences _references;
    
        private float _lastViewportWidth = -1f;
        private float _lastViewportHeight = -1f;
        private int _lastChildCount = -1;
    
        public BannerSlidesResizer(BannerCarouselReferences references)
        {
            _references = references;
        }
    
        public void ResizeToViewport()
        {
            if (_references == null)
                return;
    
            RectTransform contentRectTransform = _references.ContentRectTransform;
            RectTransform viewportRectTransform = _references.ViewportRectTransform;
    
            if (contentRectTransform == null || viewportRectTransform == null)
                return;
    
            float viewportWidth = viewportRectTransform.rect.width;
            float viewportHeight = viewportRectTransform.rect.height;
    
            if (viewportWidth <= MIN_VALID_SIZE || viewportHeight <= MIN_VALID_SIZE)
                return;
    
            int childCount = contentRectTransform.childCount;
    
            bool sameSize = Mathf.Abs(viewportWidth - _lastViewportWidth) < SIZE_COMPARE_EPSILON &&
                            Mathf.Abs(viewportHeight - _lastViewportHeight) < SIZE_COMPARE_EPSILON;
    
            if (sameSize && childCount == _lastChildCount)
                return;
    
            for (int i = 0; i < childCount; i++)
            {
                if (contentRectTransform.GetChild(i) is RectTransform childRect)
                {
                    childRect.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, viewportWidth);
                    childRect.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, viewportHeight);
                }
            }
    
            _lastViewportWidth = viewportWidth;
            _lastViewportHeight = viewportHeight;
            _lastChildCount = childCount;
        }
    }
}