using System.Collections.Generic;
using UnityEngine;

namespace MenuWithOnlineGallery.Gallery
{
    public sealed class GalleryItemViewPool
    {
        private readonly Stack<GalleryItemView> _pool = new Stack<GalleryItemView>();

        public GalleryItemView Acquire(GalleryItemView prefab, RectTransform parent)
        {
            if (prefab == null || parent == null)
                return null;

            GalleryItemView itemView;

            if (_pool.Count > 0)
            {
                itemView = _pool.Pop();
            
                if (itemView == null)
                    return Object.Instantiate(prefab, parent);

                Transform viewTransform = itemView.transform;
                viewTransform.SetParent(parent, false);
                itemView.gameObject.SetActive(true);

                return itemView;
            }

            return Object.Instantiate(prefab, parent);
        }

        public void Release(GalleryItemView itemView)
        {
            if (itemView == null)
                return;

            itemView.gameObject.SetActive(false);
            _pool.Push(itemView);
        }
    }
}