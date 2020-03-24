using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Lobby : MonoBehaviour {

    public GameObject panelLogin;
    public GameObject panelLobby;

    public Text lobbyWait;
    public Text lobbyTimeStart;

    public string lobbyTimeStartText = "Start Game in {0}...";

	public void Start ()
    {
        lobbyTimeStart.gameObject.SetActive(false);
        PanelLoginActive();
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
