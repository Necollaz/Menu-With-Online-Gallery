using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using MenuWithOnlineGallery.Common;
using MenuWithOnlineGallery.Gallery.Layout;
using MenuWithOnlineGallery.MenuScreen;
using MenuWithOnlineGallery.RemoteImages;

namespace MenuWithOnlineGallery.Gallery
{
    public sealed class GalleryView : MonoBehaviour, ICoroutineRunner
    {
        public event Action<GalleryImageModel> ItemClicked;

        [Header("References")] 
        [SerializeField] private MenuScreenConfigAsset _configAsset;
        [SerializeField] private ScrollRect _scrollRect;
        [SerializeField] private RectTransform _viewport;
        [SerializeField] private RectTransform _content;
        [SerializeField] private GridLayoutGroup _gridLayoutGroup;
        [SerializeField] private GalleryItemView _itemPrefab;

        private readonly GridVisibleRangeCalculator _visibleRangeCalculator = new GridVisibleRangeCalculator();
        private readonly GalleryItemViewsCollection _itemViewsCollection = new GalleryItemViewsCollection();
        private readonly GalleryItemViewPool _itemViewPool = new GalleryItemViewPool();

        private TextureCache _textureCache;
        private RemoteTextureLoader _remoteTextureLoader;

        private GalleryGridLayoutApplier _gridLayoutApplier;
        private GalleryLazyLoadRangeUpdater _lazyLoadRangeUpdater;

        private void Awake()
        {
            EnsureInitialized();
            _gridLayoutApplier?.Apply();
        }

        private void OnEnable()
        {
            EnsureInitialized();

            _scrollRect?.onValueChanged.AddListener(OnScrollValueChanged);
            _lazyLoadRangeUpdater?.ForceUpdate();
        }

        private void OnDisable()
        {
            _scrollRect?.onValueChanged.RemoveListener(OnScrollValueChanged);
            _lazyLoadRangeUpdater?.Reset();
        }

        Coroutine ICoroutineRunner.RunCoroutine(IEnumerator routine) => StartCoroutine(routine);

        void ICoroutineRunner.StopRunning(Coroutine coroutine)
        {
            if (coroutine == null)
                return;
            
            StopCoroutine(coroutine);
        }

        public GalleryItemView TryGetItemView(GalleryImageModel model)
        {
            if (model.Id < 0)
                return null;

            return _itemViewsCollection.TryGetById(model.Id);
        }

        public void TrySetItems(IReadOnlyList<GalleryImageModel> items)
        {
            EnsureInitialized();

            _itemViewsCollection.Clear(UnsubscribeFromItemView, ReleaseItemViewToPool);
            _itemViewsCollection.Rebuild(items, CreateItemViewByIndex, SubscribeToItemView, BindItemView);

            _gridLayoutApplier.Apply();
            _lazyLoadRangeUpdater.ForceUpdate();
        }

        private GalleryItemView CreateItemViewByIndex(int index)
        {
            if (_itemPrefab == null || _content == null)
                return null;

            GalleryItemView itemView = _itemViewPool.Acquire(_itemPrefab, _content);

            if (itemView == null)
                return null;

            itemView.transform.SetSiblingIndex(index);

            return itemView;
        }

        private int GetSpawnedCount() => _itemViewsCollection.Count;

        private void EnsureInitialized()
        {
            EnsureRemoteLoadingInitialized();

            _gridLayoutApplier ??= new GalleryGridLayoutApplier(_configAsset, _viewport, _content, _gridLayoutGroup);
            _lazyLoadRangeUpdater ??= new GalleryLazyLoadRangeUpdater(_visibleRangeCalculator, _configAsset,
                _viewport, _content, _gridLayoutGroup, GetSpawnedCount, SetItemLoadingEnabledByIndex);
        }

        private void EnsureRemoteLoadingInitialized()
        {
            if (_configAsset == null)
                return;

            _textureCache ??= new TextureCache(_configAsset.TextureCacheCapacity);
            _remoteTextureLoader ??= new RemoteTextureLoader(this, _textureCache, _configAsset.RequestTimeoutSeconds);
        }

        private void OnRectTransformDimensionsChange()
        {
            _gridLayoutApplier?.ApplyIfNeeded();
            _lazyLoadRangeUpdater?.ForceUpdate();
        }

        private void OnScrollValueChanged(Vector2 scrollNormalizedPosition)
        {
            _lazyLoadRangeUpdater.UpdateIfNeeded();
        }

        private void OnItemClicked(GalleryImageModel model)
        {
            ItemClicked?.Invoke(model);
        }

        private void SetItemLoadingEnabledByIndex(int index, bool shouldLoad, bool cancelOutsideRange)
        {
            GalleryItemView itemView = _itemViewsCollection.TryGetByIndex(index);

            if (itemView == null)
                return;

            itemView.SetLoadingEnabled(shouldLoad, cancelOutsideRange);
        }

        private void ReleaseItemViewToPool(GalleryItemView itemView)
        {
            if (itemView == null)
                return;

            _itemViewPool.Release(itemView);
        }

        private void BindItemView(GalleryItemView itemView, GalleryImageModel model)
        {
            if (itemView == null)
                return;

            itemView.TryBindModel(model, _remoteTextureLoader, this);
        }

        private void SubscribeToItemView(GalleryItemView itemView)
        {
            if (itemView == null)
                return;

            itemView.Clicked += OnItemClicked;
        }

        private void UnsubscribeFromItemView(GalleryItemView itemView)
        {
            if (itemView == null)
                return;

            itemView.Clicked -= OnItemClicked;
        }
    }
}