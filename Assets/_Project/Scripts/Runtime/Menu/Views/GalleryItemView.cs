using System;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public sealed class GalleryItemView : MonoBehaviour
{
    private const bool PRESERVE_ASPECT = true;
    
    public event Action<GalleryImageModel> Clicked; 
    
    [Header("References")]
    [SerializeField] private Button _button;
    [SerializeField] private Image _premiumBadgeImage;
    [SerializeField] private Image _previewImage;

    [Header("Placeholder")]
    [SerializeField] private Sprite _placeholderSprite;

    private RemoteTextureLoader _remoteTextureLoader;
    private ICoroutineRunner _coroutineRunner;
    
    private RemoteTextureLoadHandle _loadHandle;
    private GalleryImageModel _model;
    
    private string _requestedUrl;
    private bool _isBound;
    private bool _isLoadingEnabled;

    public Sprite CurrentSprite => _previewImage != null ? _previewImage.sprite : null;
    public GalleryImageModel Model => _model;
    
    private void Awake()
    {
        _button = GetComponent<Button>();
    }

    private void OnEnable()
    {
        if (_button != null)
        {
            _button.onClick.AddListener(OnClicked);
        }
    }

    private void OnDisable()
    {
        if (_button != null)
        {
            _button.onClick.RemoveListener(OnClicked);
        }
        
        CancelLoading();
    }

    public void TryBindModel(GalleryImageModel model, RemoteTextureLoader remoteTextureLoader,
        ICoroutineRunner coroutineRunner)
    {
        _model = model;
        _remoteTextureLoader = remoteTextureLoader;
        _coroutineRunner = coroutineRunner;
        
        _isBound = true;
        
        CancelLoading();
        SetPremiumBadgeVisible(model.IsPremium);
        SetPlaceholder();
        
        _isLoadingEnabled = true;
        _requestedUrl = null;
    }

    public void Unbind()
    {
        _isBound = false;
        _isLoadingEnabled = false;
        
        CancelLoading();
        SetPremiumBadgeVisible(false);
        SetPlaceholder();
    }
    
    public void SetLoadingEnabled(bool isEnabled, bool cancelIfDisabled)
    {
        if (!_isBound)
        {
            return;
        }

        if (isEnabled)
        {
            _isLoadingEnabled = true;
            
            StartLoadingIfNeeded(_model.Url);
            
            return;
        }

        _isLoadingEnabled = false;


        if (cancelIfDisabled)
        {
            CancelLoading();
            SetPlaceholder();
        }
    }
    
    private void StartLoadingIfNeeded(string url)
    {
        if (_remoteTextureLoader == null)
        {
            return;
        }

        if (string.IsNullOrEmpty(url))
        {
            return;
        }

        bool isSameUrlRequested = _requestedUrl == url;
        bool hasActiveHandle = !_loadHandle.IsEmpty;

        if (isSameUrlRequested && hasActiveHandle)
        {
            return;
        }

        CancelLoading();

        _requestedUrl = url;
        _loadHandle = _remoteTextureLoader.TryLoadSprite(url, OnSpriteLoaded, OnSpriteLoadFailed);
    }

    private void CancelLoading()
    {
        if (_coroutineRunner != null)
        {
            _loadHandle.Cancel(_coroutineRunner);
        }
        else
        {
            _loadHandle.ReleaseOnly();
        }

        _loadHandle = RemoteTextureLoadHandle.Empty;
        _requestedUrl = null;
    }

    private void SetPremiumBadgeVisible(bool isVisible)
    {
        if (_premiumBadgeImage == null)
        {
            return;
        }

        _premiumBadgeImage.gameObject.SetActive(isVisible);
    }

    private void SetPlaceholder()
    {
        if (_previewImage == null)
        {
            return;
        }

        if (_placeholderSprite == null)
        {
            return;
        }

        _previewImage.sprite = _placeholderSprite;
        _previewImage.preserveAspect = PRESERVE_ASPECT;
    }
    
    private void OnSpriteLoaded(Sprite sprite)
    {
        if (!_isBound || !_isLoadingEnabled)
        {
            _loadHandle.ReleaseOnly();
            
            return;
        }

        if (_model.Url != _requestedUrl)
        {
            _loadHandle.ReleaseOnly();
            
            return;
        }

        if (_previewImage == null)
        {
            _loadHandle.ReleaseOnly();
            
            return;
        }

        _previewImage.sprite = sprite;
        _previewImage.preserveAspect = PRESERVE_ASPECT;
    }
    
    private void OnSpriteLoadFailed(string error)
    {
        if (!_isBound || !_isLoadingEnabled)
        {
            return;
        }
        
        SetPlaceholder();
    }

    private void OnClicked()
    {
        if (!_isBound)
        {
            return;
        }

        Clicked?.Invoke(_model);
    }
}