using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using MLAPI;

public class ButtonsForTuning : NetworkBehaviour
{

    public TuningTrigger playerCar;
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
        var progress = SceneManager.LoadSceneAsync("Tuning", LoadSceneMode.Additive);

        while (!progress.isDone)
        {
            yield return null;
        }
        if (progress.isDone)
        {
            playerCar.OnLoadTuningServerRpc(NetworkManager.Singleton.LocalClientId);
            gameObject.SetActive(false);
        }
    }

    public void OnClickeYes()
    {
        if (playerCar != null)
        {
            loadLevel = true;
        }

    }
    public void OnClickeNo()
    {
        Destroy(this.gameObject);
    }
}
