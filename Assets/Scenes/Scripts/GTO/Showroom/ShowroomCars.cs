using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using MLAPI;
using MLAPI.NetworkVariable;
using MLAPI.Messaging;
using MLAPI.Transports.UNET;
using UnityEngine.SceneManagement;

public class ShowroomCars : NetworkBehaviour
{
    [SerializeField] public GameObject[] cars;

    private NetworkVariableInt currentCarPlayer = new NetworkVariableInt(0);

    private GameObject car;
    private int currentcar = 0;
    private int countCar;
    private Quaternion positionRot = new Quaternion(0f, 140, 0f, 50f);
    private Vector3 positionTransform = new Vector3(0f, 26f, 0f);

    private bool unloadLevel = false;

    private void Start()
    {
        currentCarPlayer.Settings.WritePermission = NetworkVariablePermission.OwnerOnly;
        currentCarPlayer.Settings.ReadPermission = NetworkVariablePermission.OwnerOnly;

        countCar = cars.Length;
        car = Instantiate(cars[currentcar]);
        car.transform.position = positionTransform;
        car.transform.rotation = positionRot;
    }

    public void nexCar()
    {
        if (car != null)
        {
            currentcar++;
            if (currentcar < countCar)
            {
                Destroy(car.gameObject);
                car = Instantiate(cars[currentcar]);
                car.transform.position = positionTransform;
                car.transform.rotation = positionRot;
            }
            else
            {
                currentcar = 0;
                Destroy(car.gameObject);
                car = Instantiate(cars[currentcar]);
                car.transform.position = positionTransform;
                car.transform.rotation = positionRot;
            }
        }
    }

    public void prevCar()
    {
        if (car != null)
        {
            currentcar--;
            if (currentcar < 0)
            {
                currentcar = countCar - 1;
                Destroy(car.gameObject);
                car = Instantiate(cars[currentcar]);
                car.transform.position = positionTransform;
                car.transform.rotation = positionRot;
            }
            else 
            {
                Destroy(car.gameObject);
                car = Instantiate(cars[currentcar]);
                car.transform.position = positionTransform;
                car.transform.rotation = positionRot;
            }
        }
    }

    private void Update()
    {
        if (unloadLevel)
        {
            unloadLevel = false;
            StartCoroutine(UnloadLevelAsync());
        }
    }


    private IEnumerator UnloadLevelAsync()
    {
        var progress = SceneManager.UnloadSceneAsync("ShowRoomScene", UnloadSceneOptions.UnloadAllEmbeddedSceneObjects);

        while (!progress.isDone)
        {
            yield return null;
        }
        if (progress.isDone)
        {
            var playerCar = NetworkManager.Singleton.ConnectedClients[NetworkManager.Singleton.LocalClientId].PlayerObject;
            var playerData = playerCar.GetComponent<TuningTrigger>();
            if (playerCar != null)
            {
                playerData.destroyCameraServerRpc();
                playerCar.GetComponent<SpawnCars>().SpawnCar(currentcar);
                Destroy(gameObject);
            }

        }
    }

    public void OnBack()
    {
        currentCarPlayer.Value = currentcar;
        Destroy(car.gameObject);
        unloadLevel = true;
    }

}
