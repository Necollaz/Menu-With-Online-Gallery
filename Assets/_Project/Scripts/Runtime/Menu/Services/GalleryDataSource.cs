using System.Collections.Generic;

namespace MenuWithOnlineGallery.Gallery
{
    public sealed class GalleryDataSource
    {
        private readonly GalleryConfig _config;

        public GalleryDataSource(GalleryConfig config)
        {
            _config = config;
        }

        public IReadOnlyList<GalleryImageModel> CreateAll()
        {
            var items = new List<GalleryImageModel>(_config.ImagesCount);

            for (int id = 1; id <= _config.ImagesCount; id++)
            {
                items.Add(CreateModel(id));
            }

            return items;
        }

        private GalleryImageModel CreateModel(int id)
        {
            string url = $"{_config.BaseUrl}{id}.jpg";
            bool isPremium = id % _config.PremiumEvery == 0;

            return new GalleryImageModel(id, url, isPremium);
        }
    }
}