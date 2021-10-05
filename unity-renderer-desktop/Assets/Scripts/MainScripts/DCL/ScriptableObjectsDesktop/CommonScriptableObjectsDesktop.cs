using UnityEngine;

namespace MainScripts.DCL.ScriptableObjectsDesktop
{
    public class CommonScriptableObjectsDesktop
    {
        private static BooleanVariable disableVSyncValue;
        public static BooleanVariable disableVSync => GetOrLoad(ref disableVSyncValue, "ScriptableObjectsDesktop/DisableVSync");
        
        public static T GetOrLoad<T>(ref T variable, string path) where T : Object
        {
            if (variable == null)
            {
                variable = Resources.Load<T>(path);
            }

            return variable;
        }
    }
}