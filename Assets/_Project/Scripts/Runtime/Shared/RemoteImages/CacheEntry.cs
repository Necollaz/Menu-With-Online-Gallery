using System.Collections.Generic;
using UnityEngine;

namespace MenuWithOnlineGallery.RemoteImages
{
    public sealed class CacheEntry
    {
        public CacheEntry(string url, Texture2D texture, Sprite sprite, LinkedListNode<string> lruNode)
        {
            Url = url;
            Texture = texture;
            Sprite = sprite;
            LruNode = lruNode;
        }

        public string Url { get; }
        public Texture2D Texture { get; set; }
        public Sprite Sprite { get; set; }
        public LinkedListNode<string> LruNode { get; }
        public int ReferenceCount { get; set; }
    }
}