using MLAPI;
using MLAPI.Messaging;
using UnityEngine;

public class StartShowroom : NetworkBehaviour
{
    [SerializeField] public Canvas showroomMenu;

    private void Start()
    {
        Canvas spawnedMenu = Instantiate(showroomMenu);
    }
}
