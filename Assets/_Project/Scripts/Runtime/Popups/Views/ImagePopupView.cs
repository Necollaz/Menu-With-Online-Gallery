using UnityEngine;
using UnityEngine.UI;

public sealed class ImagePopupView : PopupView
{
    private const bool PRESERVE_ASPECT = true;
    
    [SerializeField] private Image _image;
    [SerializeField] private Button _closeButton;

    private void OnEnable()
    {
        if (_closeButton != null)
        {
            _closeButton.onClick.AddListener(OnCloseClicked);
        }
    }

    private void OnDisable()
    {
        if (_closeButton != null)
        {
            _closeButton.onClick.RemoveListener(OnCloseClicked);
        }
    }

    public void Show(Sprite sprite)
    {
        if (_image != null)
        {
            _image.sprite = sprite;
            _image.preserveAspect = PRESERVE_ASPECT;
        }

        base.Show();
    }

    private void OnCloseClicked()
    {
        Hide();
        RequestClose();
    }
}