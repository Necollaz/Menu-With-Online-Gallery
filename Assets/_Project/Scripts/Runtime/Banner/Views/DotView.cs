using UnityEngine;
using UnityEngine.UI;

public sealed class DotView : MonoBehaviour
{
    [SerializeField] private Image _activeImage;
    [SerializeField] private Image _inactiveImage;

    public void SetActive(bool isActive)
    {
        if (_activeImage != null)
        {
            _activeImage.gameObject.SetActive(isActive);
        }

        if (_inactiveImage != null)
        {
            _inactiveImage.gameObject.SetActive(!isActive);
        }
    }
}