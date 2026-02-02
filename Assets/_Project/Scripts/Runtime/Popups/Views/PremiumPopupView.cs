using UnityEngine;
using UnityEngine.UI;

public sealed class PremiumPopupView : PopupView
{
    [SerializeField] private Button _buyButton;
    [SerializeField] private Button _closeButton;

    private void OnEnable()
    {
        if (_buyButton != null)
        {
            _buyButton.onClick.AddListener(OnBuyClicked);
        }

        if (_closeButton != null)
        {
            _closeButton.onClick.AddListener(OnCloseClicked);
        }
    }

    private void OnDisable()
    {
        if (_buyButton != null)
        {
            _buyButton.onClick.RemoveListener(OnBuyClicked);
        }

        if (_closeButton != null)
        {
            _closeButton.onClick.RemoveListener(OnCloseClicked);
        }
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