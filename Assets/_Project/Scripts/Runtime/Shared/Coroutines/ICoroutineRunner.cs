using System.Collections;
using UnityEngine;

namespace MenuWithOnlineGallery.Common
{
    public interface ICoroutineRunner
    {
        public Coroutine RunCoroutine(IEnumerator routine);
    
        public void StopRunning(Coroutine coroutine);
    }
}