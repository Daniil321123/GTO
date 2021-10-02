using MLAPI;
using MLAPI.NetworkVariable;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class ButtonForTuningMenu : NetworkBehaviour
{
    const int FRONTBUMPER = 0;
    const int REARBUMPER = 1;
    const int EXHAUSTS = 2;
    const int EXTS = 3;
    const int FENDERS = 4;
    const int ROOFS = 5;
    const int SPOILERS = 6;

    private int currentFrontBumper = 0;
    private int currentRearBumper = 0;
    private int currentExhaust = 0;
    private int currentExt = 0;
    private int currentFender = 0;
    private int currentRoof = 0;
    private int currentSpoiler = 0;

    private int[] detailsSet = new int[7];

    public TuningDetails carDetails;
    private HelloWorldPlayer car;
    private SetTuning setDetails;

    public int currentCategory = 0;
    public bool unloadLevel = false;

    private void Start()
    {
        if (carDetails != null)
        {
            car = carDetails.GetComponent<HelloWorldPlayer>();
            setDetails = carDetails.GetComponent<SetTuning>();
        } 
    }

    private void Update()
    {
        if (unloadLevel)
        {
            StartCoroutine(UnloadLevelAsync());
        }
    }

    public void changeCategoryTuning(int categoryNumber)
    {
        currentCategory = categoryNumber;
    }

    public void nextDetail()
    {
        if (carDetails != null)
        {
            switch (currentCategory)
            {
                case FRONTBUMPER:
                    currentFrontBumper = changeNextDetail(currentFrontBumper, carDetails.frontBumpers);
                    detailsSet[0] = currentFrontBumper;
                    break;
                case REARBUMPER:
                    currentRearBumper = changeNextDetail(currentRearBumper, carDetails.rearBumpers);
                    detailsSet[1] = currentRearBumper;
                    break;
                case EXHAUSTS:
                    currentExhaust = changeNextDetail(currentExhaust, carDetails.exhausts);
                    detailsSet[2] = currentExhaust;
                    break;
                case EXTS:
                    currentExt = changeNextDetail(currentExt, carDetails.exts);
                    detailsSet[3] = currentExt;
                    break;
                case FENDERS:
                    currentFender = changeNextDetail(currentFender, carDetails.fenders);
                    detailsSet[4] = currentFender;
                    break;
                case ROOFS:
                    currentRoof = changeNextDetail(currentRoof, carDetails.roofs);
                    detailsSet[5] = currentRoof;
                    break;
                case SPOILERS:
                    currentSpoiler = changeNextDetail(currentSpoiler, carDetails.spoilers);
                    detailsSet[6] = currentSpoiler;
                    break;
            }
        }
    }

    public void prevDetail()
    {
        if (carDetails != null)
        {
            switch (currentCategory)
            {
                case FRONTBUMPER:
                    currentFrontBumper = changePrevDetail(currentFrontBumper, carDetails.frontBumpers);
                    detailsSet[0] = currentFrontBumper;
                    break;
                case REARBUMPER:
                    currentRearBumper = changePrevDetail(currentRearBumper, carDetails.rearBumpers);
                    detailsSet[1] = currentRearBumper;
                    break;
                case EXHAUSTS:
                    currentExhaust = changePrevDetail(currentExhaust, carDetails.exhausts);
                    detailsSet[2] = currentExhaust;
                    break;
                case EXTS:
                    currentExt = changePrevDetail(currentExt, carDetails.exts);
                    detailsSet[3] = currentExt;
                    break;
                case FENDERS:
                    currentFender = changePrevDetail(currentFender, carDetails.fenders);
                    detailsSet[4] = currentFender;
                    break;
                case ROOFS:
                    currentRoof = changePrevDetail(currentRoof, carDetails.roofs);
                    detailsSet[5] = currentRoof;
                    break;
                case SPOILERS:
                    currentSpoiler = changePrevDetail(currentSpoiler, carDetails.spoilers);
                    detailsSet[6] = currentSpoiler;
                    break;
            }
        }
    }



    private int changeNextDetail(int currentDetail, GameObject[] arrayDetails)
    {
        var countBumpers = arrayDetails.Length;
        currentDetail++;
        if (currentDetail >= countBumpers)
        {
            currentDetail = 0;
            for (int i = 0; i < countBumpers; i++)
            {
                arrayDetails[i].gameObject.SetActive(false);
            }
            arrayDetails[currentDetail].gameObject.SetActive(true);
        }
        else
        {
            for (int i = 0; i < countBumpers; i++)
            {
                arrayDetails[i].gameObject.SetActive(false);
            }
            arrayDetails[currentDetail].gameObject.SetActive(true);
        }
        return currentDetail;
    }


    private int changePrevDetail(int currentDetail, GameObject[] arrayDetails)
    {
        var countBumpers = arrayDetails.Length;
        currentDetail--;
        if (currentDetail < 0)
        {
            currentDetail = countBumpers - 1;
            for (int i = 0; i < countBumpers; i++)
            {
                arrayDetails[i].gameObject.SetActive(false);
            }
            arrayDetails[currentDetail].gameObject.SetActive(true);
        }
        else
        {
            for (int i = 0; i < countBumpers; i++)
            {
                arrayDetails[i].gameObject.SetActive(false);
            }
            arrayDetails[currentDetail].gameObject.SetActive(true);
        }
        return currentDetail;
    }


    private IEnumerator UnloadLevelAsync()
    {
        var progress = SceneManager.UnloadSceneAsync("Scenes/Tuning");

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
                playerData.UnloadTuningServerRpc();
                Destroy(gameObject);
            }
        }
    }

    public void OnBack()
    {
        if (setDetails != null)
        {
            setDetails.changeDetailServerRpc(detailsSet);
        }
        unloadLevel = true;
    }

    public void OnConfirm()
    {
        if (setDetails != null)
        {
            setDetails.changeDetailServerRpc(detailsSet);
        }
    }
}
