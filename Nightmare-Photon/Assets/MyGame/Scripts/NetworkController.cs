using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class NetworkController : MonoBehaviourPunCallbacks {

    public Lobby lobbyScript;

    public override void OnConnected()
    {
        Debug.Log("OnConnected");
    }

    public override void OnConnectedToMaster()
    {
        Debug.Log("OnConnectedToMaster");

        lobbyScript.PanelLobbyActive();
    }

    public override void OnDisconnected(DisconnectCause cause)
    {
        Debug.Log("OnDisconnected: " + cause.ToString());

        lobbyScript.PanelLoginActive();
    }

    public void CancelButton()
    {
        PhotonNetwork.Disconnect(); // print: DisconnectByClientLogic
    }

    public void LoginButton()
    {
        PhotonNetwork.ConnectUsingSettings();
    }
}
