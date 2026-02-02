using System.Collections.Generic;
using UnityEngine;

public sealed class TextureCache
{
    private const int MIN_CAPACITY = 1;
    
    private readonly int _capacity;

    private readonly Dictionary<string, CacheEntry> _entriesByUrl = new Dictionary<string, CacheEntry>();
    private readonly LinkedList<string> _lruUrls = new LinkedList<string>();

    public TextureCache(int capacity)
    {
        _capacity = Mathf.Max(MIN_CAPACITY, capacity);
    }

    public bool TryAcquire(string url, out Sprite sprite)
    {
        sprite = null;

        if (string.IsNullOrEmpty(url))
        {
            return false;
        }

        if (!_entriesByUrl.TryGetValue(url, out CacheEntry entry))
        {
            return false;
        }
        
        entry.ReferenceCount++;
        
        Touch(entry);

        sprite = entry.Sprite;
        
        return sprite != null;
    }

    public void AddOrUpdate(string url, Texture2D texture)
    {
        if (string.IsNullOrEmpty(url) || texture == null)
        {
            return;
        }

        if (_entriesByUrl.TryGetValue(url, out CacheEntry existing))
        {
            ReplaceTexture(existing, texture);
            Touch(existing);
            TryEvictUnused();
            
            return;
        }

        Sprite sprite = CreateSprite(texture);
        
        LinkedListNode<string> node = _lruUrls.AddFirst(url);

        var entry = new CacheEntry(url, texture, sprite, node);
        _entriesByUrl.Add(url, entry);

        TryEvictUnused();
    }

    public void Release(string url)
    {
        if (string.IsNullOrEmpty(url))
        {
            return;
        }
        
        if (!_entriesByUrl.TryGetValue(url, out CacheEntry entry))
        {
            return;
        }
        
        entry.ReferenceCount = Mathf.Max(0, entry.ReferenceCount - 1);
        
        TryEvictUnused();
    }
    
    private void TryEvictUnused()
    {
        while (_entriesByUrl.Count > _capacity)
        {
            LinkedListNode<string> oldestNode = _lruUrls.Last;

            if (oldestNode == null)
            {
                return;
            }

            string oldestUrl = oldestNode.Value;

            if (!_entriesByUrl.TryGetValue(oldestUrl, out CacheEntry entry))
            {
                _lruUrls.RemoveLast();
                
                continue;
            }

            if (entry.ReferenceCount > 0)
            {
                return;
            }

            RemoveEntry(entry);
        }
    }

    private void Touch(CacheEntry entry)
    {
        if (entry.LruNode == null)
        {
            return;
        }

        _lruUrls.Remove(entry.LruNode);
        _lruUrls.AddFirst(entry.LruNode);
    }

    private void RemoveEntry(CacheEntry entry)
    {
        if (entry.LruNode != null)
        {
            _lruUrls.Remove(entry.LruNode);
        }

        _entriesByUrl.Remove(entry.Url);

        if (entry.Texture != null)
        {
            Object.Destroy(entry.Texture);
        }
    }

    private void ReplaceTexture(CacheEntry entry, Texture2D newTexture)
    {
        if (entry.Texture != null)
        {
            Object.Destroy(entry.Texture);
        }

        entry.Texture = newTexture;
        entry.Sprite = CreateSprite(newTexture);
    }

    private Sprite CreateSprite(Texture2D texture)
    {
        Rect rect = new Rect(0f, 0f, texture.width, texture.height);
        Vector2 pivot = new Vector2(0.5f, 0.5f);

        return Sprite.Create(texture, rect, pivot, 100f);
    }
}