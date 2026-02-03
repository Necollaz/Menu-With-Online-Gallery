using UnityEngine;
using UnityEngine.UI;

namespace MenuWithOnlineGallery.BannerCarousel.Dots
{
    public sealed class DotView : MonoBehaviour
    {
        [SerializeField] private Image _activeImage;
        [SerializeField] private Image _inactiveImage;

        public void SetActive(bool isActive)
        {
            _activeImage?.gameObject.SetActive(isActive);
            _inactiveImage?.gameObject.SetActive(!isActive);
        }
    }
}