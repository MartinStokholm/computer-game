using System;
using UnityEngine;

public class SceneChangeEvent : MonoBehaviour
{
    public int sceneBuildIndex;

     private void OnTriggerEnter2D(Collider2D other)
     {
         if (other.CompareTag(Settings.PlayerTag))
         {
             StaticSceneChangeEvent.CallEnterLevelEvent(sceneBuildIndex);
         }
     }
}

public static class StaticSceneChangeEvent
{
    public static event Action<SceneChangeArgs> OnEnterLevel;

    public static void CallEnterLevelEvent(int level)
    {
        OnEnterLevel?.Invoke(new SceneChangeArgs() { Level = level});
    }
}

public class SceneChangeArgs : EventArgs
{
    public int Level;
}
