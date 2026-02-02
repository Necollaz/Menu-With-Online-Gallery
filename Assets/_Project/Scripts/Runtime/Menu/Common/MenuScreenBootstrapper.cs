using UnityEngine;

public sealed class MenuScreenBootstrapper : MonoBehaviour
{
    [SerializeField] private MenuScreenConfigAsset _configAsset;
    [SerializeField] private TabBarView _tabBarView;
    [SerializeField] private GalleryView _galleryView;
    
    [Header("Popups")]
    [SerializeField] private ImagePopupView _imagePopupView;
    [SerializeField] private PremiumPopupView _premiumPopupView;
    
    private MenuScreenPresenter _presenter;

    private void Awake()
    {
        GalleryConfig galleryConfig = new GalleryConfig(_configAsset.BaseUrl, _configAsset.ImagesCount, 
            _configAsset.PremiumEvery);
        GalleryDataSource dataSource = new GalleryDataSource(galleryConfig);
        GalleryFilter filter = new GalleryFilter();
        _presenter = new MenuScreenPresenter(_tabBarView, _galleryView, dataSource, filter, _imagePopupView,
            _premiumPopupView);
    }

    private void OnEnable()
    {
        _presenter.Initialize();
    }

    private void OnDisable()
    {
        _presenter.Dispose();
    }
}