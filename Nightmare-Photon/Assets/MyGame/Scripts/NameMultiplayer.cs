using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;

public class NameMultiplayer : MonoBehaviour {

    PhotonView photonView;

    public Text playerNameText;
    public GameObject playerCanvas;
	
	void Start () {
        photonView = GetComponent<PhotonView>();
        playerNameText.text = photonView.Owner.NickName;
	}

    private void Update()
    {
        playerCanvas.transform.LookAt(Camera.main.transform);
    }
}
