using MLAPI;
using MLAPI.Messaging;
using UnityEngine;

public class SpawnCars : NetworkBehaviour
{
    [SerializeField] public GameObject[] carsList;
    [SerializeField] public Canvas showroomMenu;
    [SerializeField]private bool spawn = false;
    private int currentCarIndex = 0;
   
    public override void NetworkStart()
    {
        if (spawn)
        {
            spawnCarServerRpc(currentCarIndex);
        }
    }

    public void SpawnCar(int carIndex)
    {
        spawnCarServerRpc(carIndex);
    }

    [ServerRpc]
    private void spawnCarServerRpc(int carIndex)
    {
        GameObject spawnedCar = Instantiate(carsList[carIndex], new Vector3(), new Quaternion());
        spawnedCar.GetComponent<NetworkObject>().SpawnAsPlayerObject(OwnerClientId);
        NetworkObject.Despawn(true);
    }

    [ServerRpc]
    private void spawnShowroomMenuServerRpc()
    {
        Canvas spawnedMenu = Instantiate(showroomMenu);
        spawnedMenu.GetComponent<NetworkObject>().SpawnAsPlayerObject(OwnerClientId);
    }
}
