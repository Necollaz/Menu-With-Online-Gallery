using System;
using UnityEngine;
using UnityEngine.UI;

namespace MenuWithOnlineGallery.Popups
{
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
            _root?.gameObject.SetActive(true);
            
            EnsureInputBlockingEnabled(true);
        }
    
        public void Hide()
        {
            EnsureInputBlockingEnabled(false);
            
            _root?.gameObject.SetActive(false);
        }
    
        protected void RequestClose()
        {
            CloseRequested?.Invoke();
        }
        
        private void EnsureInputBlockingEnabled(bool isEnabled)
        {
            if (_inputBlockerImage == null)
                return;
    
            _inputBlockerImage.raycastTarget = isEnabled;
            _inputBlockerImage.enabled = isEnabled;
        }
    }
}