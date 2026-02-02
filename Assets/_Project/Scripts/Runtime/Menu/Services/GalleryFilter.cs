using System.Collections.Generic;

public sealed class GalleryFilter
{
    public IReadOnlyList<GalleryImageModel> TryApply(GalleryTabType tabType, IReadOnlyList<GalleryImageModel> allItems)
    {
        if (allItems == null || allItems.Count == 0)
        {
            return allItems;
        }

        if (tabType == GalleryTabType.All)
        {
            return allItems;
        }

        var result = new List<GalleryImageModel>(allItems.Count);

        for (int i = 0; i < allItems.Count; i++)
        {
            GalleryImageModel item = allItems[i];

            if (tabType == GalleryTabType.Odd && (item.Id & 1) == 1)
            {
                result.Add(item);
                continue;
            }

            if (tabType == GalleryTabType.Even && (item.Id & 1) == 0)
            {
                result.Add(item);
            }
        }

        return result;
    }
}