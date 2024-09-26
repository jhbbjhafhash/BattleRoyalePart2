using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Realtime;
using Photon.Pun;
using System.Linq;

public class GameManager : MonoBehaviourPun
{
     public float postGameTime;

   [Header("Players")]
   public string playerPrefabLocation;
   public PlayerController[] players;
   public Transform[] spawnPoints;
   public int alivePlayers;

   private int playersInGame;

   public static GameManager instance;

   void Awake()
   {
        instance = this;
   }

   void Start()
   {
        players = new PlayerController[PhotonNetwork.PlayerList.Length];
        alivePlayers = players.Length;

        photonView.RPC("ImInGame", RpcTarget.AllBuffered);
   }
    
    [PunRPC]
   void ImInGame()
   {
        playersInGame++;

        if(PhotonNetwork.IsMasterClient && playersInGame == PhotonNetwork.PlayerList.Length)
            photonView.RPC("SpawnPlayer", RpcTarget.All);

   }

    [PunRPC]
   void SpawnPlayer()
   {
        GameObject playerObj = PhotonNetwork.Instantiate(playerPrefabLocation, spawnPoints[Random.Range(0, spawnPoints.Length)].position, Quaternion.identity);
        playerObj.GetComponent<PlayerController>().photonView.RPC("Initialize", RpcTarget.All, PhotonNetwork.LocalPlayer);
   }

   public PlayerController GetPlayer (int playerId)
   {
      return players.First(x => x.id == playerId);
   }

   public PlayerController GetPlayer (GameObject playerObject)
   {
      return players.First(x => x.gameObject == playerObject);
   }

   public void CheckWinCondition ()
   {
     if(alivePlayers == 1)
          photonView.RPC("WinGame", RpcTarget.All, players.First(x => !x.dead));

   }

     [PunRPC]
   void WinGame(int winningPlayer)
   {
     GameUI.instance.SetWinText(GetPlayer(winningPlayer).photonPlayer.NickName);

     Invoke("GoBackToMenu", postGameTime);

   }

   void GoBackToMenu ()
   {
     NetworkManager.instance.ChangeScene("Menu");
   }
}
