using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace DCL
{
    public class CommandLineParserDesktop : MonoBehaviour
    {
        // Start is called before the first frame update
        private void Awake()
        {
            var debugConfig = GameObject.Find("DebugConfig").GetComponent<DebugConfigComponent>();

            var arguments = System.Environment.GetCommandLineArgs();

            for (var i = 0; i < arguments.Length; ++i)
            {
                var argument = arguments[i];
                switch (argument)
                {
                    case "--url-params":
                        i++; // shift
                        debugConfig.baseUrlCustom += arguments[i] + "&";
                        break;
                    case "--browser":
                        i++; // shift
                        debugConfig.openBrowserWhenStart = arguments[i] == "true";
                        break;
                }
            }
        }
    }
}