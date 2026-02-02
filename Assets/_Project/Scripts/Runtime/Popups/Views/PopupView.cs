using System;
using UnityEngine;
using UnityEngine.UI;

public abstract class PopupView : MonoBehaviour
{
    public event Action CloseRequested;

    [Header("Root")]
    [SerializeField] private RectTransform _root;
    
    [Header("Input Blocker (optional)")]
    [SerializeField] private Image _inputBlockerImage;

    protected virtual void Awake()
    {
        Hide();
    }

    public void Show()
    {
        if (_root == null)
        {
            return;
        }

        _root.gameObject.SetActive(true);
        
        EnsureInputBlockingEnabled(true);
    }

    public void Hide()
    {
        if (_root == null)
        {
            return;
        }

        EnsureInputBlockingEnabled(false);
        
        _root.gameObject.SetActive(false);
    }

    protected void RequestClose()
    {
        CloseRequested?.Invoke();
    }
    
    private void EnsureInputBlockingEnabled(bool isEnabled)
    {
        if (_inputBlockerImage == null)
        {
            return;
        }

        _inputBlockerImage.raycastTarget = isEnabled;
        _inputBlockerImage.enabled = isEnabled;
    }
}