using MLAPI.Serialization;
using UnityEngine;

public struct PlayerState : INetworkSerializable
{
    public bool Light;
    public PlayerState( bool light)
    {
        Light = light;
    }

    public void NetworkSerialize(NetworkSerializer serializer)
    {
        bool IsOnLight;
        if (serializer.IsReading)
        {
            IsOnLight = Light;
        }
        serializer.Serialize(ref Light);
    }

}
