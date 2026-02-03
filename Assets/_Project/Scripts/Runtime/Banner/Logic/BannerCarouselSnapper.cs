using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace MenuWithOnlineGallery.BannerCarousel
{
    public sealed class BannerCarouselSnapper
    {
        private const float ZERO = 0f;
        private const float EASE_SMOOTHSTEP_A = 3f;
        private const float EASE_SMOOTHSTEP_B = 2f;
        private const float SNAP_T_COMPLETE = 1f;

        private readonly MonoBehaviour _coroutineOwner;
        private readonly ScrollRect _scrollRect;

        private readonly float _snapDurationSeconds;

        private Coroutine _snapCoroutine;

        public BannerCarouselSnapper(MonoBehaviour coroutineOwner, ScrollRect scrollRect, float snapDurationSeconds)
        {
            _coroutineOwner = coroutineOwner;
            _scrollRect = scrollRect;
            _snapDurationSeconds = snapDurationSeconds;
        }

        public void SnapTo(float targetNormalizedPosition)
        {
            if (_coroutineOwner == null || _scrollRect == null)
                return;

            Stop();

            _snapCoroutine = _coroutineOwner.StartCoroutine(SnapCoroutine(targetNormalizedPosition,
                _snapDurationSeconds));
        }

        public void Stop()
        {
            if (_coroutineOwner == null)
            {
                _snapCoroutine = null;
                
                return;
            }

            if (_snapCoroutine == null)
                return;

            _coroutineOwner.StopCoroutine(_snapCoroutine);
            _snapCoroutine = null;
        }

        private IEnumerator SnapCoroutine(float targetNormalizedPosition, float snapDurationSeconds)
        {
            float startNormalizedPosition = _scrollRect.horizontalNormalizedPosition;
            float elapsedUnscaledSeconds = ZERO;
            float durationSeconds = Mathf.Max(ZERO, snapDurationSeconds);

            while (elapsedUnscaledSeconds < durationSeconds)
            {
                elapsedUnscaledSeconds += Time.unscaledDeltaTime;

                float linear01 = durationSeconds <= ZERO ? SNAP_T_COMPLETE : Mathf.Clamp01(elapsedUnscaledSeconds / 
                    durationSeconds);
                float eased01 = EvaluateSmoothStep01(linear01);
                float currentNormalizedPosition = Mathf.Lerp(startNormalizedPosition, targetNormalizedPosition, 
                    eased01);

                _scrollRect.horizontalNormalizedPosition = currentNormalizedPosition;

                yield return null;
            }

            _scrollRect.horizontalNormalizedPosition = targetNormalizedPosition;
            _snapCoroutine = null;
        }

        private float EvaluateSmoothStep01(float normalizedProgress)
        {
            float clampedProgress01 = Mathf.Clamp01(normalizedProgress);

            return clampedProgress01 * clampedProgress01 * (EASE_SMOOTHSTEP_A - EASE_SMOOTHSTEP_B * clampedProgress01);
        }
    }
}