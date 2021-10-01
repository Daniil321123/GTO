using System.Collections.Generic;
using MLAPI;
using UnityEngine;

public class NetworkCommandLine : MonoBehaviour
{
    private NetworkManager netManager;

    private void Start()
    {
        netManager = GetComponentInParent<NetworkManager>();

        if (Application.isEditor) return;

        var args = GetCommandLineArgs();

        if (args.TryGetValue("-mlapi", out string mlapivalue))
        {
            switch (mlapivalue)
            {
                case "server":
                    netManager.StartServer();
                    break;
                case "client":
                    netManager.StartClient();
                    break;
                case "host":
                    netManager.StartHost();
                    break;
            }
        }
    }

    private Dictionary<string, string> GetCommandLineArgs()
    {
        Dictionary<string, string> argsDictonary = new Dictionary<string, string>();

        var args = System.Environment.GetCommandLineArgs();

        for (int i = 0; i < args.Length; ++i)
        {
            var arg = args[i].ToLower();
            if(arg.StartsWith("-"))
            {
                var value = i < args.Length - 1 ? args[i + 1].ToLower() : null;
                value = (value?.StartsWith("-") ?? false) ? null : value;

                argsDictonary.Add(arg, value);
            }
        }

        return argsDictonary;
    }
}
