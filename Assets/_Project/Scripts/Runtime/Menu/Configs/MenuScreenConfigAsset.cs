using UnityEngine;

[CreateAssetMenu(menuName = "Game/Menu Screen Config")]
public sealed class MenuScreenConfigAsset : ScriptableObject
{
    [Header("Gallery")]
    [SerializeField] private string _baseUrl = "https://data.ikppbb.com/test-task-unity-data/pics/";
    [SerializeField] private int _imagesCount = 66;
    [SerializeField] private int _premiumEvery = 4;
    
    [Header("Grid")]
    [SerializeField] private int _phoneColumns = 2;
    [SerializeField] private int _tabletColumns = 3;
    [SerializeField] private float _tabletMinSidePixels = 1200f;
    
    [Header("Remote Images")]
    [SerializeField] private int _textureCacheCapacity = 24;
    [SerializeField] private int _requestTimeoutSeconds = 30;

    [Header("Lazy Load")]
    [SerializeField] private int _lazyLoadBufferRows = 2;
    [SerializeField] private bool _cancelLoadOutsideRange = true;

    public string BaseUrl => _baseUrl;
    public int ImagesCount => _imagesCount;
    public int PremiumEvery => _premiumEvery;
    
    public int PhoneColumns => _phoneColumns;
    public int TabletColumns => _tabletColumns;
    public float TabletMinSidePixels => _tabletMinSidePixels;
    
    public int TextureCacheCapacity => _textureCacheCapacity;
    public int RequestTimeoutSeconds => _requestTimeoutSeconds;
    
    public int LazyLoadBufferRows => _lazyLoadBufferRows;
    public bool CancelLoadOutsideRange => _cancelLoadOutsideRange;
}