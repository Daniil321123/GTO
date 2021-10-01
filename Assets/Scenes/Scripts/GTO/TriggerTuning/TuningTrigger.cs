using UnityEngine;
using MLAPI;
using MLAPI.Messaging;

public class TuningTrigger : NetworkBehaviour
{
    [SerializeField] private Canvas tuningSelected;
    [SerializeField] private Canvas showroomSelected;
    [SerializeField] private Camera cameraShowRoom;
    private NetworkObject networkObject;
    private Canvas instanceMenuShowroom;
    private Canvas instanceMenuTuning;

    public Camera newCamera;

    private Vector3 positionCarForTuning = new Vector3(9.4f, 1963f, 8.1f);
    private Vector3 positionCarForTuningDespawn = new Vector3(-29f, 4.4f, -27.4f);

    private void Start()
    {
        networkObject = gameObject.GetComponent<NetworkObject>();
    }

    private void OnTriggerEnter(Collider collider)
    {
        if (IsLocalPlayer && collider.isTrigger && collider.gameObject.name == "Showroom")
        {
            instanceMenuShowroom = Instantiate(showroomSelected);
            instanceMenuShowroom.GetComponent<ButtonsForTuningSelected>().playerCar = this;
        }
        else if (IsLocalPlayer && collider.isTrigger && collider.gameObject.name == "Tuning")
        {
            instanceMenuTuning = Instantiate(tuningSelected);
            instanceMenuTuning.GetComponent<ButtonsForTuning>().playerCar = this;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (IsLocalPlayer && other.isTrigger && other.gameObject.name == "Showroom")
        {
            Destroy(instanceMenuShowroom.gameObject);
        }
        else if (IsLocalPlayer && other.isTrigger && other.gameObject.name == "Tuning")
        {
            Destroy(instanceMenuTuning.gameObject);
        }
    }

    [ServerRpc]
    public void OnLoadShowroomServerRpc(ulong clientID)
    {
        newCamera = Instantiate(cameraShowRoom, new Vector3(0f, 25.9f, -4.6f), Quaternion.identity);
        if (newCamera != null)
        {
            newCamera.GetComponent<NetworkObject>().SpawnWithOwnership(clientID);
            OnLoadShowroomClientRpc();
        }
    }

    [ClientRpc]
    public void OnLoadShowroomClientRpc()
    {
        if (gameObject.GetComponent<HelloWorldPlayer>().cameraInst != null && gameObject.GetComponent<HelloWorldPlayer>().CanvasInst != null)
        {
            Destroy(gameObject.GetComponent<HelloWorldPlayer>().cameraInst.gameObject);
            Destroy(gameObject.GetComponent<HelloWorldPlayer>().CanvasInst.gameObject);
        }
    }

    [ServerRpc]
    public void OnLoadTuningServerRpc(ulong clientID)
    {
        gameObject.GetComponent<Transform>().position = positionCarForTuning;
        gameObject.GetComponent<NetworkObject>().gameObject.SetActive(false); //отключение синхры
        OnLoadTuningClientRpc();
    }

    [ClientRpc]
    public void OnLoadTuningClientRpc()
    {
        gameObject.GetComponent<Transform>().position = positionCarForTuning;
        if (gameObject.GetComponent<HelloWorldPlayer>().cameraInst != null && gameObject.GetComponent<HelloWorldPlayer>().CanvasInst != null)
        {
            gameObject.GetComponent<HelloWorldPlayer>().cameraInst.gameObject.SetActive(false);
            gameObject.GetComponent<HelloWorldPlayer>().CanvasInst.gameObject.SetActive(false);
            gameObject.GetComponent<TuningDetails>().isTuningModeOn = true;
        }
    }

    [ServerRpc]
    public void UnloadTuningServerRpc()
    {
        gameObject.GetComponent<Transform>().position = positionCarForTuningDespawn;
        gameObject.GetComponent<NetworkObject>().gameObject.SetActive(true); //включение синхры
        UnloadTuningClientRpc();
    }

    [ClientRpc]
    public void UnloadTuningClientRpc()
    {
        gameObject.GetComponent<Transform>().position = positionCarForTuningDespawn;
        if (gameObject.GetComponent<HelloWorldPlayer>().cameraInst != null && gameObject.GetComponent<HelloWorldPlayer>().CanvasInst != null)
        {
            gameObject.GetComponent<HelloWorldPlayer>().cameraInst.gameObject.SetActive(true);
            gameObject.GetComponent<HelloWorldPlayer>().CanvasInst.gameObject.SetActive(true);
            gameObject.GetComponent<TuningDetails>().isTuningModeOn = false;
        }
    }

    [ServerRpc]
    public void destroyCameraServerRpc()
    {
        if (newCamera != null)
        {
            Destroy(newCamera.gameObject);
            destroyCameraClientRpc();
        }
    }

    [ClientRpc]
    public void destroyCameraClientRpc()
    {
        if (newCamera != null)
        {
            Destroy(newCamera.gameObject);
        }
    }
}
