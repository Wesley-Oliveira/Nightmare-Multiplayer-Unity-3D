using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class GameControllerGamePlay : MonoBehaviourPunCallbacks {

    public GameObject myPlayer;
    public Transform[] spawnPLayer;

    private void Start()
    {
        int position = Random.Range(0, spawnPLayer.Length);
        GameObject playerTemp = PhotonNetwork.Instantiate(myPlayer.name, spawnPLayer[position].position, spawnPLayer[position].rotation, 0);

        if (playerTemp.GetComponent<PhotonView>().Owner.IsMasterClient)
        {
            ExitGames.Client.Photon.Hashtable props = new ExitGames.Client.Photon.Hashtable
            {
                {CountDownEndGame.CountdownStartTime, (float) PhotonNetwork.Time}
            };
            PhotonNetwork.CurrentRoom.SetCustomProperties(props);
        }
    }
}
