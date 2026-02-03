using System;
using System.Collections;
using UnityEngine;

namespace MenuWithOnlineGallery.BannerCarousel
{
    public sealed class BannerCarouselAutoScroller
    {
        private readonly MonoBehaviour _coroutineOwner;
        private readonly BannerCarouselState _state;
        
        private readonly Action<int> _scrollToIndex;
    
        private Coroutine _autoScrollCoroutine;
    
        public BannerCarouselAutoScroller(
            MonoBehaviour coroutineOwner,
            BannerCarouselState state,
            Action<int> scrollToIndex)
        {
            _coroutineOwner = coroutineOwner;
            _state = state;
            _scrollToIndex = scrollToIndex;
        }
    
        public void Start()
        {
            if (_coroutineOwner == null || _state == null || _scrollToIndex == null)
                return;
    
            Stop();
            
            _autoScrollCoroutine = _coroutineOwner.StartCoroutine(AutoScrollLoop());
        }
    
        public void Stop()
        {
            if (_coroutineOwner == null)
            {
                _autoScrollCoroutine = null;
                
                return;
            }
    
            if (_autoScrollCoroutine == null)
                return;
    
            _coroutineOwner.StopCoroutine(_autoScrollCoroutine);
            _autoScrollCoroutine = null;
        }
    
        private IEnumerator AutoScrollLoop()
        {
            while (true)
            {
                yield return null;
    
                if (!_state.CanAutoScroll(Time.unscaledTime))
                    continue;
    
                int nextIndex = _state.GetNextAutoIndex();
                _scrollToIndex.Invoke(nextIndex);
                _state.ScheduleNextAutoScroll(Time.unscaledTime);
            }
        }
    }
}