using System;
using System.Collections.Generic;

namespace MenuWithOnlineGallery.Gallery
{
    public sealed class GalleryItemViewsCollection
    {
        private readonly List<GalleryItemView> _spawned = new List<GalleryItemView>();
        private readonly Dictionary<int, GalleryItemView> _viewsById = new Dictionary<int, GalleryItemView>();

        public int Count => _spawned.Count;

        public GalleryItemView TryGetByIndex(int index)
        {
            if (index < 0 || index >= _spawned.Count)
                return null;

            return _spawned[index];
        }

        public GalleryItemView TryGetById(int id)
        {
            if (id < 0)
                return null;

            if (_viewsById.TryGetValue(id, out GalleryItemView itemView))
                return itemView;

            return null;
        }

        public void Rebuild(IReadOnlyList<GalleryImageModel> items, Func<int, GalleryItemView> createItemViewByIndex,
            Action<GalleryItemView> subscribeToItemView, Action<GalleryItemView, GalleryImageModel> bindItemView)
        {
            if (items == null || createItemViewByIndex == null)
                return;

            for (int i = 0; i < items.Count; i++)
            {
                GalleryItemView itemView = createItemViewByIndex.Invoke(i);

                if (itemView == null)
                    continue;

                bindItemView?.Invoke(itemView, items[i]);
                subscribeToItemView?.Invoke(itemView);

                _spawned.Add(itemView);
                _viewsById[itemView.Model.Id] = itemView;
            }
        }

        public void Clear(Action<GalleryItemView> unsubscribeFromItemView, Action<GalleryItemView> releaseItemView)
        {
            for (int i = 0; i < _spawned.Count; i++)
            {
                GalleryItemView itemView = _spawned[i];

                if (itemView == null)
                    continue;
                
                unsubscribeFromItemView?.Invoke(itemView);
                
                itemView.Unbind();

                releaseItemView?.Invoke(itemView);
            }

            _spawned.Clear();
            _viewsById.Clear();
        }
    }
}