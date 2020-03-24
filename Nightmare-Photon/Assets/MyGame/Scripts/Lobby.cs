using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;

public class Lobby : MonoBehaviour {

    public GameObject panelLogin;
    public GameObject panelLobby;

    public Text lobbyWait;
    public Text lobbyTimeStart;
    public Text playerStatus;

    public InputField playerInputField;
    public string playerName;

	void Start ()
    {
        lobbyTimeStart.gameObject.SetActive(false);
        playerStatus.gameObject.SetActive(false);
        PanelLoginActive();

        playerName = "Player" + Random.Range(1000, 10000);
        playerInputField.text = playerName;
	}

    public void PanelLobbyActive()
    {
        panelLogin.SetActive(false);
        panelLobby.SetActive(true);
    }

    public void PanelLoginActive()
    {
        panelLogin.SetActive(true);
        panelLobby.SetActive(false);
    }
}
