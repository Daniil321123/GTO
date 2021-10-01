//----------------------------------------------
//            Realistic Car Controller
//
// Copyright © 2014 - 2021 BoneCracker Games
// http://www.bonecrackergames.com
// Buğra Özdoğanlar
//
//----------------------------------------------

#if RCC_PHOTON && PHOTON_UNITY_NETWORKING
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;

public class RCC_PhotonManager : MonoBehaviourPunCallbacks{

    public static RCC_PhotonManager Instance;

    const string version = "1.0";
    public string gameplaySceneName = "Gameplay Scene Name";

    [Header("UI Menus")]
    public InputField nickPanel;
    public GameObject browseRoomsPanel;
    public GameObject roomsContent;
    public GameObject chatLinesPanel;
    public GameObject chatLinesContent;
    public GameObject noRoomsYet;
    public GameObject createRoomButton;
    public GameObject exitRoomButton;

    [Header("UI Texts")]
    public Text status;
    public Text totalOnlinePlayers;
    public Text totalRooms;
    public Text region;
    public Text informer;

    [Header("UI Prefabs")]
    public RCC_PhotonUIRoom roomPrefab;
    public RCC_PhotonUIChatLine chatLinePrefab;

    private List<RCC_PhotonUIRoom> UIRoomButtons = new List<RCC_PhotonUIRoom>();

    private void Awake() {

        if (Instance == null) {

            Instance = this;
            DontDestroyOnLoad(transform.root.gameObject);

        } else {

            Destroy(gameObject);
            return;
        
        }

    }

    private void Start() {

        status.text = "Ready to connect";
        nickPanel.text = "New Player " + Random.Range(0, 99999).ToString();

    }

    private void Update() {

        if (PhotonNetwork.IsConnected && PhotonNetwork.InLobby) {

            totalOnlinePlayers.text = "Total Online Players: " + PhotonNetwork.CountOfPlayers.ToString();
            totalRooms.text = "Total Online Rooms: " + PhotonNetwork.CountOfRooms.ToString();
            region.text = "Region: " + PhotonNetwork.CloudRegion.ToString();

        }

    }

    public void Connect(){

        if (!PhotonNetwork.IsConnected) {

            PhotonNetwork.NickName = nickPanel.text;
            ConnectToServer();

        } else {

            nickPanel.gameObject.SetActive(false);

        }
        
    }

    private void ConnectToServer(){

        informer.text = status.text = "Connecting to server";
        Debug.Log(status.text);
        PhotonNetwork.ConnectUsingSettings();
        nickPanel.gameObject.SetActive(false);

        if(RCC_InfoLabel.Instance)
            RCC_InfoLabel.Instance.ShowInfo("Connecting to server");

    }

    public override void OnConnectedToMaster() {

        informer.text = "";
        Debug.Log("Connected to server");
        Debug.Log("Entering to lobby");
        status.text = "Entering to lobby";
        PhotonNetwork.JoinLobby();

        if (RCC_InfoLabel.Instance)
            RCC_InfoLabel.Instance.ShowInfo("Connected to server, Entering to lobby");

    }

    public override void OnJoinedLobby() {

        Debug.Log("Entered to lobby");
        status.text = "Entered to lobby";
        nickPanel.gameObject.SetActive(false);
        browseRoomsPanel.SetActive(true);
        createRoomButton.SetActive(true);
        exitRoomButton.SetActive(false);

        if (RCC_InfoLabel.Instance)
            RCC_InfoLabel.Instance.ShowInfo("Entering to lobby");

    }

    public override void OnJoinedRoom() {

        Debug.Log("Joined room");
        status.text = "Joined room";
        nickPanel.gameObject.SetActive(false);
        browseRoomsPanel.SetActive(false);
        createRoomButton.SetActive(false);
        exitRoomButton.SetActive(true);
        chatLinesPanel.gameObject.SetActive(true);
        UIRoomButtons.Clear();
        LoadLevel(gameplaySceneName);

        if (RCC_InfoLabel.Instance)
            RCC_InfoLabel.Instance.ShowInfo("Joined room, You can spawn your vehicle from 'Options' menu");

    }

    public override void OnCreatedRoom() {

        Debug.Log("Created room");
        status.text = "Created room";
        nickPanel.gameObject.SetActive(false);
        browseRoomsPanel.SetActive(false);
        createRoomButton.SetActive(false);
        exitRoomButton.SetActive(true);
        chatLinesPanel.gameObject.SetActive(true);
        print(SceneManager.GetSceneByName(gameplaySceneName));
        UIRoomButtons.Clear();
        LoadLevel(gameplaySceneName);

        if (RCC_InfoLabel.Instance)
            RCC_InfoLabel.Instance.ShowInfo("Created room, You can spawn your vehicle from 'Options' menu");

    }

    public override void OnRoomListUpdate(List <RoomInfo> roomInfo) {

        Debug.Log("Rooms found: " + roomInfo.Count);

        bool roomFound = false;

        foreach (RoomInfo room in roomInfo) {

            if (room.RemovedFromList) {

                int index = UIRoomButtons.FindIndex(x => x.roomNameString == room.Name);
                
                if (index != -1) {

                    Destroy(UIRoomButtons[index].gameObject);
                    UIRoomButtons.RemoveAt(index);
                
                }

            } else {

                if (room.IsOpen && room.IsVisible && room.PlayerCount > 0) {

                    RCC_PhotonUIRoom roomButton = GameObject.Instantiate(roomPrefab.gameObject, roomsContent.transform).GetComponent<RCC_PhotonUIRoom>();
                    UIRoomButtons.Add(roomButton);
                    roomButton.Check(room.Name, (room.PlayerCount + "/" + room.MaxPlayers).ToString());

                }

            }

        }

        if (UIRoomButtons.Count > 0)
            roomFound = true;

        noRoomsYet.SetActive(!roomFound);

    }

    public void CreateRoom() {

        RoomOptions roomOptions = new RoomOptions();
        roomOptions.IsOpen = true;
        roomOptions.IsVisible = true;
        roomOptions.MaxPlayers = 4;

        PhotonNetwork.JoinOrCreateRoom("New Room " + Random.Range(0, 999), roomOptions, TypedLobby.Default);
        
    }

    public void JoinSelectedRoom(RCC_PhotonUIRoom room) {

        PhotonNetwork.JoinRoom(room.roomName.text);
    
    }

    public void Chat(InputField inputField) {
        
        photonView.RPC("RPCChat", RpcTarget.AllBuffered, PhotonNetwork.NickName, inputField.text);
    
    }

    [PunRPC]
    public void RPCChat(string nickName, string text) {

        RCC_PhotonUIChatLine newChatLine = GameObject.Instantiate(chatLinePrefab.gameObject, chatLinesContent.transform).GetComponent<RCC_PhotonUIChatLine>();
        newChatLine.Line(nickName + " : " + text);

        RCC_PhotonUIChatLine[] chatLines = chatLinesContent.gameObject.GetComponentsInChildren<RCC_PhotonUIChatLine>();

        if (chatLines.Length > 7)
            Destroy(chatLines[0].gameObject);

    }

    public void ExitRoom() {

        PhotonNetwork.LeaveRoom();
    
    }

    public override void OnLeftRoom() {

        Debug.Log("Exited room");
        status.text = "Exited room";
        nickPanel.gameObject.SetActive(false);
        browseRoomsPanel.SetActive(true);
        createRoomButton.SetActive(true);
        exitRoomButton.SetActive(false);
        chatLinesPanel.gameObject.SetActive(false);

    }

    public void ExitLobby() {

        PhotonNetwork.LeaveLobby();
    
    }

    public override void OnLeftLobby() {

        Debug.Log("Exited to lobby");
        status.text = "Exited to lobby";
        nickPanel.gameObject.SetActive(true);
        browseRoomsPanel.SetActive(false);
        createRoomButton.SetActive(false);
        exitRoomButton.SetActive(false);
        chatLinesPanel.gameObject.SetActive(false);

    }

    public void LoadLevel(string level) {

        PhotonNetwork.LoadLevel(level);

    }

}
#endif