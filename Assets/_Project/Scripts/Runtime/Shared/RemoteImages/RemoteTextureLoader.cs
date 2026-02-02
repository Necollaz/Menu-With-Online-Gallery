using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

public sealed class RemoteTextureLoader
{
    private const bool NON_READABLE_TEXTURE = false;

    private readonly ICoroutineRunner _coroutineRunner;
    private readonly TextureCache _textureCache;
    private readonly int _requestTimeoutSeconds;

    public RemoteTextureLoader(ICoroutineRunner coroutineRunner, TextureCache textureCache, int requestTimeoutSeconds)
    {
        _coroutineRunner = coroutineRunner;
        _textureCache = textureCache;
        _requestTimeoutSeconds = Mathf.Max(1, requestTimeoutSeconds);
    }

    public RemoteTextureLoadHandle TryLoadSprite(string url, Action<Sprite> onCompleted, Action<string> onFailed)
    {
        if (string.IsNullOrEmpty(url))
        {
            onFailed?.Invoke("URL is null or empty.");
            
            return RemoteTextureLoadHandle.Empty;
        }

        if (_textureCache.TryAcquire(url, out Sprite cachedSprite))
        {
            onCompleted?.Invoke(cachedSprite);
            
            return new RemoteTextureLoadHandle(url, null, _textureCache);
        }

        if (_coroutineRunner == null)
        {
            onFailed?.Invoke($"[RemoteTextureLoader] CoroutineRunner is null. url={url}");
            
            return RemoteTextureLoadHandle.Empty;
        }

        Coroutine coroutine = _coroutineRunner.RunCoroutine(DownloadCoroutine(url, onCompleted, onFailed));
        
        return new RemoteTextureLoadHandle(url, coroutine, _textureCache);
    }

    private IEnumerator DownloadCoroutine(string url, Action<Sprite> onCompleted, Action<string> onFailed)
    {
        using UnityWebRequest request = UnityWebRequestTexture.GetTexture(url, NON_READABLE_TEXTURE);
        request.timeout = _requestTimeoutSeconds;

        UnityWebRequestAsyncOperation operation = request.SendWebRequest();

        while (!operation.isDone)
        {
            yield return null;
        }

        if (request.result != UnityWebRequest.Result.Success)
        {
            onFailed?.Invoke($"Load failed: result={request.result}, code={request.responseCode}," +
                             $" error={request.error}, url={url}");
            yield break;
        }

        Texture2D texture = DownloadHandlerTexture.GetContent(request);
        
        if (texture == null)
        {
            onFailed?.Invoke($"Downloaded texture is null. url={url}");
            
            yield break;
        }

        _textureCache.AddOrUpdate(url, texture);

        if (_textureCache.TryAcquire(url, out Sprite sprite))
        {
            onCompleted?.Invoke(sprite);
            
            yield break;
        }

        onFailed?.Invoke($"Texture cached but sprite not acquired. url={url}");
    }
}