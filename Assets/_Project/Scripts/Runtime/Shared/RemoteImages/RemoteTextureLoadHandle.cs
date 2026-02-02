using UnityEngine;

public readonly struct RemoteTextureLoadHandle
{
    public static RemoteTextureLoadHandle Empty => new RemoteTextureLoadHandle(string.Empty, null, null);

    private readonly string _url;
    private readonly Coroutine _coroutine;
    private readonly TextureCache _textureCache;

    public RemoteTextureLoadHandle(string url, Coroutine coroutine, TextureCache textureCache)
    {
        _url = url;
        _coroutine = coroutine;
        _textureCache = textureCache;
    }
    
    public bool IsEmpty => string.IsNullOrEmpty(_url) && _coroutine == null && _textureCache == null;

    public void Cancel(ICoroutineRunner coroutineRunner)
    {
        if (_coroutine != null && coroutineRunner != null)
        {
            coroutineRunner.StopRunning(_coroutine);
        }

        if (!string.IsNullOrEmpty(_url) && _textureCache != null)
        {
            _textureCache.Release(_url);
        }
    }

    public void ReleaseOnly()
    {
        if (!string.IsNullOrEmpty(_url) && _textureCache != null)
        {
            _textureCache.Release(_url);
        }
    }
}