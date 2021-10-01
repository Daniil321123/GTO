using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using MLAPI;

public class ButtonsForTuningSelected : NetworkBehaviour
{

    public TuningTrigger playerCar;
    public NetworkObject carSpawn;

    public bool loadLevel = false;

    private void Update()
    {
        if (loadLevel)
        {
            loadLevel = false;
            StartCoroutine(LoadLevelAsync());
        }
    }

    private IEnumerator LoadLevelAsync()
    {
        var progress = SceneManager.LoadSceneAsync("ShowRoomScene", LoadSceneMode.Additive);

        while (!progress.isDone)
        {
            yield return null;
        }
        if (progress.isDone)
        {
            playerCar.OnLoadShowroomServerRpc(NetworkManager.Singleton.LocalClientId);
            gameObject.SetActive(false);
        }
    }

    private IEnumerator UnloadLevelAsync()
    {
        var progress = SceneManager.UnloadSceneAsync("GoldenPath");

        while (!progress.isDone)
        {
            yield return null;
        }
    }

    public void OnClickeYes()
    {
        if (playerCar != null)
        {
            loadLevel = true;
            Debug.Log("Yes");
        }

    }
    public void OnClickeNo()
    {
        Destroy(this.gameObject);
    }
}
