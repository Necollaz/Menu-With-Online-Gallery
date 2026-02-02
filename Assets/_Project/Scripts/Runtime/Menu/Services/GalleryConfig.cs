public sealed class GalleryConfig
{
    public GalleryConfig(string baseUrl, int imagesCount, int premiumEvery)
    {
        BaseUrl = baseUrl;
        ImagesCount = imagesCount;
        PremiumEvery = premiumEvery;
    }
    
    public string BaseUrl { get; }
    public int ImagesCount { get; }
    public int PremiumEvery { get; }
}