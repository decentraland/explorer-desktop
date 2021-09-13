using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace DCL
{
    public static class CommandLineParserUtils
    {
        public static void ParseArguments()
        {
            var debugConfig = GameObject.Find("DebugConfig").GetComponent<DebugConfigComponent>();

            var arguments = System.Environment.GetCommandLineArgs();

            for (var i = 0; i < arguments.Length; ++i)
            {
                var argumentsLeft = arguments.Length - i - 1;
                var argument = arguments[i];

                if (argumentsLeft >= 1) // Arguments with at least 1 parameter
                {
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
}