using UnityEngine;
using System.Collections;
using MLAPI;
using MLAPI.Messaging;
using MLAPI.NetworkVariable;

public class SetTuning : NetworkBehaviour
{
    const int FRONTBUMPER = 0;
    const int REARBUMPER = 1;
    const int EXHAUSTS = 2;
    const int EXTS = 3;
    const int FENDERS = 4;
    const int ROOFS = 5;
    const int SPOILERS = 6;

    private NetworkVariableInt m_currentFronBumper = new NetworkVariableInt(0);
    private NetworkVariableInt m_currentRearBumper = new NetworkVariableInt(0);
    private NetworkVariableInt m_currentExhaust = new NetworkVariableInt(0);
    private NetworkVariableInt m_currentExt = new NetworkVariableInt(0);
    private NetworkVariableInt m_currentFender = new NetworkVariableInt(0);
    private NetworkVariableInt m_currentRoof = new NetworkVariableInt(0);
    private NetworkVariableInt m_currentSpoiler = new NetworkVariableInt(0);

    private NetworkVariableBool m_online = new NetworkVariableBool(false);

    public NetworkVariableInt CurrentFrontBumper => m_currentFronBumper;
    public NetworkVariableInt CurrentRearBumper => m_currentRearBumper;
    public NetworkVariableInt CurrentExhaust => m_currentExhaust;
    public NetworkVariableInt CurrentExt => m_currentExt;
    public NetworkVariableInt CurrentFender => m_currentFender;
    public NetworkVariableInt CurrentRoof => m_currentRoof;
    public NetworkVariableInt CurrentSpoiler => m_currentSpoiler;
    public NetworkVariableBool Online => m_online;

    private TuningDetails tuningDetails;

    private void Start()
    {
        tuningDetails = gameObject.GetComponent<TuningDetails>();
    }

    public override void NetworkStart()
    {
        base.NetworkStart();

        NetworkManager.Singleton.OnClientConnectedCallback += Singleton_OnClientConnectedCallback;
        NetworkManager.Singleton.OnClientDisconnectCallback += Singleton_OnClientDisconnectCallback;
    }

    private void OnDestroy()
    {
        NetworkManager.Singleton.OnClientConnectedCallback -= Singleton_OnClientConnectedCallback;
        NetworkManager.Singleton.OnClientDisconnectCallback -= Singleton_OnClientDisconnectCallback;
    }

    private void Singleton_OnClientDisconnectCallback(ulong obj)
    {
        setOnlineServerRpc(false);
    }

    private void Singleton_OnClientConnectedCallback(ulong obj)
    {
        setOnlineServerRpc(true);
    }

    private void Update()
    {
        //if (Input.GetKeyDown(KeyCode.P))
        //{
        //    if (IsOwner && IsClient)
        //    {
        //        changeDetailServerRpc();
        //    }
        //}
    }

    private void OnEnable()
    {
        CurrentFrontBumper.OnValueChanged += OnCurrentFrontBumperChanged;
        CurrentRearBumper.OnValueChanged += OnCurrentFrontBumperChanged;
        CurrentExhaust.OnValueChanged += OnCurrentFrontBumperChanged;
        CurrentExt.OnValueChanged += OnCurrentFrontBumperChanged;
        CurrentFender.OnValueChanged += OnCurrentFrontBumperChanged;
        CurrentRoof.OnValueChanged += OnCurrentFrontBumperChanged;
        CurrentSpoiler.OnValueChanged += OnCurrentFrontBumperChanged;

        Online.OnValueChanged += OnOnlineChanged;
    }

    private void OnDisable()
    {
        CurrentFrontBumper.OnValueChanged += OnCurrentFrontBumperChanged;
        CurrentRearBumper.OnValueChanged += OnCurrentFrontBumperChanged;
        CurrentExhaust.OnValueChanged += OnCurrentFrontBumperChanged;
        CurrentExt.OnValueChanged += OnCurrentFrontBumperChanged;
        CurrentFender.OnValueChanged += OnCurrentFrontBumperChanged;
        CurrentRoof.OnValueChanged += OnCurrentFrontBumperChanged;
        CurrentSpoiler.OnValueChanged += OnCurrentFrontBumperChanged;

        Online.OnValueChanged += OnOnlineChanged;
    }

    private void OnCurrentFrontBumperChanged(int previousValue, int newValue)
    {
        if (tuningDetails != null)
        {
            if (!IsOwner)
            {
                tuningDetails.setDetails(m_currentFronBumper.Value, FRONTBUMPER);
                tuningDetails.setDetails(m_currentRearBumper.Value, REARBUMPER);
                tuningDetails.setDetails(m_currentExhaust.Value, EXHAUSTS);
                tuningDetails.setDetails(m_currentExt.Value, EXTS);
                tuningDetails.setDetails(m_currentFender.Value, FENDERS);
                tuningDetails.setDetails(m_currentRoof.Value, ROOFS);
                tuningDetails.setDetails(m_currentSpoiler.Value, SPOILERS);
            }
            else if(IsOwner && IsClient)
            {
                tuningDetails.setDetails(m_currentFronBumper.Value, FRONTBUMPER);
                tuningDetails.setDetails(m_currentRearBumper.Value, REARBUMPER);
                tuningDetails.setDetails(m_currentExhaust.Value, EXHAUSTS);
                tuningDetails.setDetails(m_currentExt.Value, EXTS);
                tuningDetails.setDetails(m_currentFender.Value, FENDERS);
                tuningDetails.setDetails(m_currentRoof.Value, ROOFS);
                tuningDetails.setDetails(m_currentSpoiler.Value, SPOILERS);
            }
        }
    }

    private IEnumerator startOnline()
    {
        yield return new WaitForSeconds(2f);
        if (!IsOwner)
        {
            if (tuningDetails != null)
            {
                tuningDetails.setDetails(m_currentFronBumper.Value, FRONTBUMPER);
                tuningDetails.setDetails(m_currentRearBumper.Value, REARBUMPER);
                tuningDetails.setDetails(m_currentExhaust.Value, EXHAUSTS);
                tuningDetails.setDetails(m_currentExt.Value, EXTS);
                tuningDetails.setDetails(m_currentFender.Value, FENDERS);
                tuningDetails.setDetails(m_currentRoof.Value, ROOFS);
                tuningDetails.setDetails(m_currentSpoiler.Value, SPOILERS);
            }
        }
    }

    private void OnOnlineChanged(bool previousValue, bool newValue)
    {
        StartCoroutine(startOnline());
    }

    [ServerRpc]
    public void changeDetailServerRpc(int[] details)
    {
        for (int detailIndex = 0; detailIndex < details.Length; detailIndex++)
        {
            switch (detailIndex)
            {
                case FRONTBUMPER:
                    CurrentFrontBumper.Value = details[detailIndex];
                    break;
                case REARBUMPER:
                    CurrentRearBumper.Value = details[detailIndex];
                    break;
                case EXHAUSTS:
                    CurrentExhaust.Value = details[detailIndex];
                    break;
                case EXTS:
                    CurrentExt.Value = details[detailIndex];
                    break;
                case FENDERS:
                    CurrentFender.Value = details[detailIndex];
                    break;
                case ROOFS:
                    CurrentRoof.Value = details[detailIndex];
                    break;
                case SPOILERS:
                    CurrentSpoiler.Value = details[detailIndex];
                    break;
            }
        }
    }

    [ServerRpc]
    public void setOnlineServerRpc(bool online)
    {
        Online.Value = online;
    }
}
