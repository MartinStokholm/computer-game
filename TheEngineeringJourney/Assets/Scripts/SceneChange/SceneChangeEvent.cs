using System;
using UnityEngine;

public class SceneChangeEvent : MonoBehaviour
{
    public int sceneBuildIndex;

     private void OnTriggerEnter2D(Collider2D other)
     {
         //if (other.tag == "player")
         {
             StaticEventHandler.CallEnterLevelEvent(sceneBuildIndex);
         }
     }
}

public static class StaticEventHandler
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
