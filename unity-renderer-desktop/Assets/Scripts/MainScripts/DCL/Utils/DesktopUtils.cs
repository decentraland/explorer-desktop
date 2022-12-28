using System;
using UnityEngine;
using Object = UnityEngine.Object;

namespace MainScripts.DCL.Utils
{
    public static class DesktopUtils
    {
        public static void Quit()
        {
            Debug.Log("Disposing");
            bool disposed = false;
            foreach (var mb in Object.FindObjectsOfType<MonoBehaviour>())
            {
                Debug.Log(mb.name);

                if (mb is IDisposable disposable)
                {
                    disposable.Dispose();
                    disposed = true;
                }
            }

            if(disposed)
                return;
            
            Debug.Log("Destroying");
            foreach (var mb in Object.FindObjectsOfType<MonoBehaviour>())
            {
                Debug.Log(mb.name);
                Object.Destroy(mb);
            }
// #if UNITY_EDITOR
//             // Application.Quit() does not work in the editor so
//             // UnityEditor.EditorApplication.isPlaying need to be set to false to end the game
//             UnityEditor.EditorApplication.isPlaying = false;
// #else
//             Application.Quit();
// #endif
        }
    }
}