using UnityEngine;
using UnityEngine.UI;

public sealed class GalleryGridLayoutApplier
{
    private const float WIDTH_COMPARE_EPSILON = 0.1f;
    private const float MIN_VALID_WIDTH = 0.01f;
    private const float MIN_CELL_SIZE = 0f;
    private const int INVALID_INDEX = -1;

    private readonly MenuScreenConfigAsset _configAsset;
    private readonly RectTransform _viewport;
    private readonly RectTransform _content;
    private readonly GridLayoutGroup _gridLayoutGroup;

    private int _lastColumns = INVALID_INDEX;
    private float _lastWidth = -1f;

    public GalleryGridLayoutApplier(MenuScreenConfigAsset configAsset, RectTransform viewport, RectTransform content,
        GridLayoutGroup gridLayoutGroup)
    {
        _configAsset = configAsset;
        _viewport = viewport;
        _content = content;
        _gridLayoutGroup = gridLayoutGroup;
    }

    public void ApplyIfNeeded()
    {
        if (_gridLayoutGroup == null || _configAsset == null)
        {
            return;
        }

        float width = GetCurrentWidth();
        int columns = GetColumns();

        if (columns == _lastColumns && Mathf.Abs(width - _lastWidth) < WIDTH_COMPARE_EPSILON)
        {
            return;
        }

        Apply();
    }

    public void Apply()
    {
        if (_gridLayoutGroup == null || _configAsset == null)
        {
            return;
        }

        float width = GetCurrentWidth();
        
        if (width <= MIN_VALID_WIDTH)
        {
            return;
        }

        int columns = GetColumns();

        _gridLayoutGroup.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
        _gridLayoutGroup.constraintCount = columns;

        float available = width - _gridLayoutGroup.padding.left - _gridLayoutGroup.padding.right 
                          - _gridLayoutGroup.spacing.x * (columns - 1);

        float cell = columns <= 0 ? MIN_CELL_SIZE : available / columns;
        
        if (cell < MIN_CELL_SIZE)
        {
            cell = MIN_CELL_SIZE;
        }

        _gridLayoutGroup.cellSize = new Vector2(cell, cell);
        _lastColumns = columns;
        _lastWidth = width;
    }

    private float GetCurrentWidth()
    {
        if (_viewport != null)
        {
            return _viewport.rect.width;
        }

        if (_content != null)
        {
            return _content.rect.width;
        }

        return 0f;
    }

    private int GetColumns()
    {
        float minSide = Mathf.Min(Screen.width, Screen.height);
        bool isTablet = minSide >= _configAsset.TabletMinSidePixels;

        if (isTablet)
        {
            return _configAsset.TabletColumns;
        }

        bool isLandscape = Screen.width > Screen.height;
        
        return isLandscape ? _configAsset.TabletColumns : _configAsset.PhoneColumns;
    }
}