using System;
using UnityEngine;
using Object = UnityEngine.Object;

namespace MainScripts.DCL.Utils
{
    public static class DesktopUtils
    {
        public static void Quit()
        {
            var disposed = false;
            
            Debug.Log("Disposing");
            foreach (var mb in Object.FindObjectsOfType<MonoBehaviour>())
            {
                Debug.Log(mb.name);

                if (mb is not IDisposable disposable) continue;
                
                disposable.Dispose();
                disposed = true;
            }
            if(disposed) return;
            
            Debug.Log("Destroying");
            foreach (var mb in Object.FindObjectsOfType<MonoBehaviour>())
            {
                Debug.Log(mb.name);
                Object.Destroy(mb);
                
                disposed = true;
            }
            if(disposed) return;
            
#if UNITY_EDITOR
            // Application.Quit() does not work in the editor so
            // UnityEditor.EditorApplication.isPlaying need to be set to false to end the game
            UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
        }
    }
}