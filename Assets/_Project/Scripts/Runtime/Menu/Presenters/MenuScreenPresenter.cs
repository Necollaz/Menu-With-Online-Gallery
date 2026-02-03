using System;
using System.Collections.Generic;
using UnityEngine;
using MenuWithOnlineGallery.Gallery;
using MenuWithOnlineGallery.Popups;

namespace MenuWithOnlineGallery.MenuScreen
{
    public sealed class MenuScreenPresenter : IDisposable
    {
        private readonly TabBarView _tabBarView;
        private readonly GalleryView _galleryView;
        private readonly GalleryDataSource _dataSource;
        private readonly GalleryFilter _filter;

        private readonly ImagePopupView _imagePopupView;
        private readonly PremiumPopupView _premiumPopupView;

        private GalleryTabType _activeTab;

        private IReadOnlyList<GalleryImageModel> _allItems;

        public MenuScreenPresenter(
            TabBarView tabBarView,
            GalleryView galleryView,
            GalleryDataSource dataSource,
            GalleryFilter filter,
            ImagePopupView imagePopupView,
            PremiumPopupView premiumPopupView)
        {
            _tabBarView = tabBarView;
            _galleryView = galleryView;
            _dataSource = dataSource;
            _filter = filter;
            _imagePopupView = imagePopupView;
            _premiumPopupView = premiumPopupView;
        }

        public void Dispose()
        {
            if (_tabBarView != null)
                _tabBarView.TabSelected -= OnTabSelected;

            if (_galleryView != null)
                _galleryView.ItemClicked -= OnGalleryItemClicked;

            if (_imagePopupView != null)
                _imagePopupView.CloseRequested -= HideAllPopups;

            if (_premiumPopupView != null)
                _premiumPopupView.CloseRequested -= HideAllPopups;
        }

        public void Initialize()
        {
            HideAllPopups();

            _allItems = _dataSource.CreateAll();
            _activeTab = GalleryTabType.All;

            if (_tabBarView == null || _galleryView == null || _imagePopupView == null ||
                _premiumPopupView == null)
            {
                return;
            }
            
            if (_tabBarView != null)
                _tabBarView.TabSelected += OnTabSelected;
            
            if (_galleryView != null) 
                _galleryView.ItemClicked += OnGalleryItemClicked;
            
            if (_imagePopupView != null) 
                _imagePopupView.CloseRequested += HideAllPopups;
            
            if (_premiumPopupView != null) 
                _premiumPopupView.CloseRequested += HideAllPopups;

            ApplyTab(_activeTab);
        }

        private void OnTabSelected(GalleryTabType tabType)
        {
            if (_activeTab == tabType)
                return;

            ApplyTab(tabType);
        }

        private void ApplyTab(GalleryTabType tabType)
        {
            _activeTab = tabType;

            _tabBarView?.TrySetActiveTab(tabType);

            IReadOnlyList<GalleryImageModel> filtered = _filter.TryApply(tabType, _allItems);

            _galleryView?.TrySetItems(filtered);
        }

        private void OnGalleryItemClicked(GalleryImageModel model)
        {
            HideAllPopups();

            if (model.IsPremium)
            {
                _premiumPopupView?.Show();

                return;
            }

            if (_galleryView == null || _imagePopupView == null)
                return;

            GalleryItemView itemView = _galleryView.TryGetItemView(model);

            if (itemView == null)
                return;

            Sprite sprite = itemView.CurrentSprite;

            if (sprite == null)
                return;

            _imagePopupView.Show(sprite);
        }

        private void HideAllPopups()
        {
            _imagePopupView?.Hide();
            _premiumPopupView?.Hide();
        }
    }
}