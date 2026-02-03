using UnityEngine;
using UnityEngine.UI;

namespace MenuWithOnlineGallery.Popups
{
    public sealed class PremiumPopupView : PopupView
    {
        [SerializeField] private Button _buyButton;
        [SerializeField] private Button _closeButton;

        private void OnEnable()
        {
            _buyButton?.onClick.AddListener(OnBuyClicked);
            _closeButton?.onClick.AddListener(OnCloseClicked);
        }

        private void OnDisable()
        {
            _buyButton?.onClick.RemoveListener(OnBuyClicked);
            _closeButton?.onClick.RemoveListener(OnCloseClicked);
        }

        private void OnBuyClicked()
        {
            Debug.Log("Buy Premium clicked");
        }

        private void OnCloseClicked()
        {
            Hide();
            RequestClose();
        }
    }
}