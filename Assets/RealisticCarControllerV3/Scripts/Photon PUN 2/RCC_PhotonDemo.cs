//----------------------------------------------
//            Realistic Car Controller
//
// Copyright © 2014 - 2021 BoneCracker Games
// http://www.bonecrackergames.com
// Buğra Özdoğanlar
//
//----------------------------------------------

#if RCC_PHOTON && PHOTON_UNITY_NETWORKING
using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using Photon;
using Photon.Pun;
using Photon.Realtime;

/// <summary>
/// A simple manager script for photon demo scene. It has an array of networked spawnable player vehicles, public methods, restart, and quit application.
/// </summary>
[AddComponentMenu("BoneCracker Games/Realistic Car Controller/Network/Photon/RCC Photon Demo Manager")]
public class RCC_PhotonDemo : Photon.Pun.MonoBehaviourPunCallbacks {

	public bool reconnectIfFails = true;
	private bool connectedWithThis = false;

	internal int selectedCarIndex = 0;
	internal int selectedBehaviorIndex = 0;

	public Transform spawnPoint;
	public GameObject menu;

    private void Start() {

		if (reconnectIfFails && !PhotonNetwork.IsConnectedAndReady)
			ConnectToPhoton();

    }

	void ConnectToPhoton() {

		Debug.Log("Connecting to server");
		connectedWithThis = true;
		RCC_InfoLabel.Instance.ShowInfo("Entering to lobby");
		PhotonNetwork.NickName = "New Player " + Random.Range(0, 99999).ToString();
		PhotonNetwork.ConnectUsingSettings();

	}

    public void Spawn () {

		Vector3 lastKnownPos = Vector3.zero;
		Quaternion lastKnownRot = Quaternion.identity;

		RCC_CarControllerV3 newVehicle;

		if(RCC_SceneManager.Instance.activePlayerVehicle){

			lastKnownPos = RCC_SceneManager.Instance.activePlayerVehicle.transform.position;
			lastKnownRot = RCC_SceneManager.Instance.activePlayerVehicle.transform.rotation;

		}

		if (lastKnownPos == Vector3.zero) {
			
			lastKnownPos = spawnPoint.position;
			lastKnownRot = spawnPoint.rotation;

		}

		lastKnownRot.x = 0f;
		lastKnownRot.z = 0f;

		if(RCC_SceneManager.Instance.activePlayerVehicle)
			PhotonNetwork.Destroy(RCC_SceneManager.Instance.activePlayerVehicle.gameObject);
			
		newVehicle = PhotonNetwork.Instantiate("Photon Vehicles/" + RCC_PhotonDemoVehicles.Instance.vehicles[selectedCarIndex].name, lastKnownPos + (Vector3.up), lastKnownRot, 0).GetComponent<RCC_CarControllerV3>();

		RCC.RegisterPlayerVehicle (newVehicle);
		RCC.SetControl (newVehicle, true);

		if (RCC_SceneManager.Instance.activePlayerCamera)
			RCC_SceneManager.Instance.activePlayerCamera.SetTarget (newVehicle.gameObject);
		
	}

	/// <summary>
	/// Selects the vehicle.
	/// </summary>
	/// <param name="index">Index.</param>
	public void SelectVehicle(int index){

		selectedCarIndex = index;

	}

	// An integer index value used for setting behavior mode.
	public void SetBehavior(int index){

		selectedBehaviorIndex = index;

	}

	// Here we are setting new selected behavior to corresponding one.
	public void InitBehavior(){

		RCC.SetBehavior(selectedBehaviorIndex);

	}

	//	Sets the main controller type.
	public void SetController(int index){

		RCC.SetController (index);

	}

	// Sets the mobile controller type.
	public void SetMobileController(int index){

		switch(index){

		case 0:
			RCC.SetMobileController (RCC_Settings.MobileController.TouchScreen);
			break;
		case 1:
			RCC.SetMobileController (RCC_Settings.MobileController.Gyro);
			break;
		case 2:
			RCC.SetMobileController (RCC_Settings.MobileController.SteeringWheel);
			break;
		case 3:
			RCC.SetMobileController (RCC_Settings.MobileController.Joystick);
			break;

		}

	}

	/// <summary>
	/// Sets the quality.
	/// </summary>
	/// <param name="index">Index.</param>
	public void SetQuality(int index){

		QualitySettings.SetQualityLevel (index);

	}

	// Simply restarting the current scene.
	public void RestartScene(){

		SceneManager.LoadScene (SceneManager.GetActiveScene().buildIndex);

	}

	// Simply quit application. Not working on Editor.
	public void Quit(){

		Application.Quit();

	}

	public override void OnConnectedToMaster() {

		if (!connectedWithThis)
			return;

		Debug.Log("Connected to server");
		Debug.Log("Entering to lobby");
		RCC_InfoLabel.Instance.ShowInfo("Entering to lobby");
		PhotonNetwork.JoinLobby();

	}

	public override void OnJoinedLobby() {

		if (!connectedWithThis)
			return;

		Debug.Log("Entered to lobby");
		RCC_InfoLabel.Instance.ShowInfo("Creating / Joining Random Room");
		PhotonNetwork.JoinRandomRoom();

	}

    public override void OnJoinRandomFailed(short returnCode, string message) {

		if (!connectedWithThis)
			return;

		RoomOptions roomOptions = new RoomOptions();
		roomOptions.IsOpen = true;
		roomOptions.IsVisible = true;
		roomOptions.MaxPlayers = 4;

		PhotonNetwork.JoinOrCreateRoom("New Room " + Random.Range(0, 999), roomOptions, TypedLobby.Default);

	}

    public override void OnJoinedRoom() {

		if (!connectedWithThis)
			return;

		if (menu)
			menu.SetActive(true);

    }

    public override void OnCreatedRoom() {

		if (!connectedWithThis)
			return;

		if (menu)
			menu.SetActive(true);

	}

}
#endif