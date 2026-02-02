using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public sealed class TabBarView : MonoBehaviour
{
    public event Action<GalleryTabType> TabSelected;

    [Header("Buttons")]
    [SerializeField] private Button _allButton;
    [SerializeField] private Button _oddButton;
    [SerializeField] private Button _evenButton;

    [Header("Labels")]
    [SerializeField] private TextMeshProUGUI _allLabel;
    [SerializeField] private TextMeshProUGUI _oddLabel;
    [SerializeField] private TextMeshProUGUI _evenLabel;

    [Header("Style")]
    [SerializeField] private float _inactiveAlpha = 0.6f;
    [SerializeField] private float _activeAlpha = 1.0f;

    private bool _isInitialized;
    
    private void Awake()
    {
        if (_isInitialized)
        {
            return;
        }

        _isInitialized = true;

        if (_allButton != null)
        {
            _allButton.onClick.AddListener(OnAllClicked);
        }
        
        if (_oddButton != null)
        {
            _oddButton.onClick.AddListener(OnOddClicked);
        }
        
        if (_evenButton != null)
        {
            _evenButton.onClick.AddListener(OnEvenClicked);
        }
    }

    private void OnDestroy()
    {
        if (_allButton != null)
        {
            _allButton.onClick.RemoveListener(OnAllClicked);
        }
        
        if (_oddButton != null)
        {
            _oddButton.onClick.RemoveListener(OnOddClicked);
        }
        
        if (_evenButton != null)
        {
            _evenButton.onClick.RemoveListener(OnEvenClicked);
        }
    }

    public void TrySetActiveTab(GalleryTabType tabType)
    {
        TrySetLabelAlpha(_allLabel, tabType == GalleryTabType.All ? _activeAlpha : _inactiveAlpha);
        TrySetLabelAlpha(_oddLabel, tabType == GalleryTabType.Odd ? _activeAlpha : _inactiveAlpha);
        TrySetLabelAlpha(_evenLabel, tabType == GalleryTabType.Even ? _activeAlpha : _inactiveAlpha);

        if (_allButton != null)
        {
            _allButton.interactable = tabType != GalleryTabType.All;
        }
        
        if (_oddButton != null)
        {
            _oddButton.interactable = tabType != GalleryTabType.Odd;
        }
        
        if (_evenButton != null)
        {
            _evenButton.interactable = tabType != GalleryTabType.Even;
        }
    }

    private void TrySetLabelAlpha(TextMeshProUGUI label, float alpha)
    {
        if (label == null)
        {
            return;
        }
        
        Color color = label.color;
        color.a = alpha;
        label.color = color;
    }
    
    private void OnAllClicked() => TabSelected?.Invoke(GalleryTabType.All);
    
    private void OnOddClicked() => TabSelected?.Invoke(GalleryTabType.Odd);
    
    private void OnEvenClicked() => TabSelected?.Invoke(GalleryTabType.Even);
}