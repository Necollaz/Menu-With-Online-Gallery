namespace MenuWithOnlineGallery.Gallery
{
    public readonly struct GalleryImageModel
    {
        public GalleryImageModel(int id, string url, bool isPremium)
        {
            Id = id;
            Url = url;
            IsPremium = isPremium;
        }
    
        public int Id { get; }
        public string Url { get; }
        public bool IsPremium { get; }
    }
}