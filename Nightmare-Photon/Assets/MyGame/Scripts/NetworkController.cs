using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using Photon.Pun.UtilityScripts;

using Hashtable = ExitGames.Client.Photon.Hashtable;

public class NetworkController : MonoBehaviourPunCallbacks {

    public Lobby lobbyScript;
    public byte playerRoomMax = 2;

    public override void OnEnable()
    {
        base.OnEnable();
        CountdownTimer.OnCountdownTimerHasExpired += OnCountDownTimeIsExpired;
    }

    void Start()
    {
        PhotonNetwork.AutomaticallySyncScene = true;
    }

    public override void OnDisable()
    {
        base.OnDisable();
        CountdownTimer.OnCountdownTimerHasExpired -= OnCountDownTimeIsExpired;
    }

    void OnCountDownTimeIsExpired()
    {
        StartGame();
    }

    public override void OnConnected()
    {
        Debug.Log("OnConnected");
    }

    public override void OnConnectedToMaster()
    {
        Debug.Log("OnConnectedToMaster");

        lobbyScript.PanelLobbyActive();

        PhotonNetwork.JoinLobby();
    }

    public override void OnJoinedLobby()
    {
        Debug.Log("OnJoinedToLobby");
        PhotonNetwork.JoinRandomRoom();
    }

    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        Debug.Log("OnJoinRandomFailed");

        string roomName = "Room" + Random.Range(1000, 10000);
        RoomOptions roomOptions = new RoomOptions()
        {
            IsOpen = true,
            IsVisible = true,
            MaxPlayers = playerRoomMax
        };

        PhotonNetwork.CreateRoom(roomName, roomOptions, TypedLobby.Default);
    }

    public override void OnJoinedRoom()
    {
        Debug.Log("OnJoinedRoom"); // For masterclient
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        Debug.Log("OnPlayerEnteredRoom");

        if (PhotonNetwork.CurrentRoom.PlayerCount == playerRoomMax)
        {
            foreach(var itemm in PhotonNetwork.PlayerList)
            {
                if (itemm.IsMasterClient)
                {
                    Hashtable props = new Hashtable
                    {
                        {CountdownTimer.CountdownStartTime, (float) PhotonNetwork.Time}
                    };

                    PhotonNetwork.CurrentRoom.SetCustomProperties(props);
                }
            }
        }
    }

    void StartGame()
    {
        PhotonNetwork.LoadLevel(1);
    }

    public override void OnDisconnected(DisconnectCause cause)
    {
        Debug.Log("OnDisconnected: " + cause.ToString());

        lobbyScript.PanelLoginActive();
    }

    public void CancelButton()
    {
        PhotonNetwork.Disconnect(); // print: DisconnectByClientLogic
        lobbyScript.playerStatus.gameObject.SetActive(false);
    }

    public void LoginButton()
    {
        PhotonNetwork.NickName = lobbyScript.playerInputField.text;
        lobbyScript.playerStatus.gameObject.SetActive(true);
        PhotonNetwork.ConnectUsingSettings();
    }
}
