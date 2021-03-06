using UnityEngine;
using MLAPI;
using MLAPI.NetworkVariable;

public class NetworkVariableTest : NetworkBehaviour
{
    private NetworkVariable<float> ServerNetworkVariable = new NetworkVariable<float>();
    private NetworkVariable<float> ClientNetworkVariable = new NetworkVariable<float>();
    private float last_t = 0.0f;

    private void Start()
    {
        ClientNetworkVariable.Settings.WritePermission = NetworkVariablePermission.OwnerOnly;   
        ClientNetworkVariable.Settings.ReadPermission = NetworkVariablePermission.ServerOnly;

        if (IsServer)
        {
            ServerNetworkVariable.Value = 0.0f;
            Debug.Log("Server's var initialized to: " + ServerNetworkVariable.Value);
        }
        else if(IsClient) 
        {
            ClientNetworkVariable.Value = 0.0f;
            Debug.Log("Client's var initialized to: " + ClientNetworkVariable.Value);
        }
    }

    private void Update()
    {
        var t_bow = Time.time;
        if (IsServer)
        {
            ServerNetworkVariable.Value = ServerNetworkVariable.Value + 0.1f;
            if (t_bow - last_t > 0.5f)
            {
                last_t = t_bow;
                Debug.Log("Server set its var to: " + ServerNetworkVariable.Value + ", has client var at: " + ClientNetworkVariable.Value);
            }
        }
        else if (IsClient)
        {
            ClientNetworkVariable.Value = ClientNetworkVariable.Value + 0.1f;
            if (t_bow - last_t > 0.5f)
            {
                last_t = t_bow;
                Debug.Log("Client set its var to: " + ClientNetworkVariable.Value + ", has server var at: " + ServerNetworkVariable.Value);
            }
        }
    }
}
