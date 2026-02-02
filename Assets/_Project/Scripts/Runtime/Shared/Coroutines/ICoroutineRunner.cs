using System.Collections;
using UnityEngine;

public interface ICoroutineRunner
{
    public Coroutine RunCoroutine(IEnumerator routine);
    
    public void StopRunning(Coroutine coroutine);
}