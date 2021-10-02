using System.Collections;
using System.Collections.Generic;
using MLAPI;
using MLAPI.NetworkVariable;
using MLAPI.Messaging;
using UnityEngine;
using UnityEngine.Networking;

public class SpawnCars : NetworkBehaviour
{
    [SerializeField] public CarsList carsList;
    [SerializeField] public Canvas showroomMenu;
    [SerializeField]private bool spawn = false;
    private int currentCarIndex = 0;

    public NetworkVariableInt carIndexSpawn = new NetworkVariableInt(0);

    string rootURL = "http://192.168.0.168/";

    public override void NetworkStart()
    {
        if (spawn)
        {
            spawnCerStartGameServerRpc(PlayerInfo.currentCar);
        }
    }

    public void SpawnCar()
    {
        spawnCarServerRpc();
    }

    [ServerRpc]
    private void spawnCarServerRpc()
    {
        GameObject spawnedCar = Instantiate(carsList.vehicles[carIndexSpawn.Value], new Vector3(), new Quaternion());
        spawnedCar.GetComponent<NetworkObject>().SpawnAsPlayerObject(OwnerClientId);
        NetworkObject.Despawn(true);
    }

    [ServerRpc]
    public void spawnCerStartGameServerRpc(int indexCar)
    {
        GameObject spawnedCar = Instantiate(carsList.vehicles[indexCar], new Vector3(), new Quaternion());
        spawnedCar.GetComponent<NetworkObject>().SpawnAsPlayerObject(OwnerClientId);
        NetworkObject.Despawn(true);
    }

    [ServerRpc]
    private void spawnShowroomMenuServerRpc()
    {
        Canvas spawnedMenu = Instantiate(showroomMenu);
        spawnedMenu.GetComponent<NetworkObject>().SpawnAsPlayerObject(OwnerClientId);
    }

    [ServerRpc]

    public void setNewCarServerRpc(int carIndex)
    {
        StartCoroutine(setNewCar(carIndex));
    }

    private IEnumerator setNewCar(int indexCar)
    {
        using (UnityWebRequest www = UnityWebRequest.Get(rootURL + "online/" + PlayerInfo.nicName + "&" + PlayerInfo.password + "&" + indexCar))
        {
            yield return www.SendWebRequest();
            if (www.result == UnityWebRequest.Result.Success)
            {
                carIndexSpawn.Value = indexCar;
            }
        }
    }

}

