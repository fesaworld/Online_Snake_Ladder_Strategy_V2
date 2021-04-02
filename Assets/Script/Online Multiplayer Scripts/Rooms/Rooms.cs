using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

using Photon.Pun;
using Photon.Realtime;
using Photon.Pun.UtilityScripts;
using ExitGames.Client.Photon;

public class Rooms : MonoBehaviourPunCallbacks
{
    public GameObject player1SpawnLocation, player2SpawnLocation, player3SpawnLocation, player4SpawnLocation;
    public Button RollButton, PlayButton, LeaveRoomButton;
    public static bool isRoomFull = false;
    public static int expectedMaxPlayer = 0;

    GameObject player1Avatar, player2Avatar, player3Avatar, player4Avatar;
    SceneLoader sceneLoader;

    void Start()
    {
        sceneLoader = GameObject.Find("SceneLoader").GetComponent<SceneLoader>();

        if(PhotonNetwork.IsMasterClient) {
            RollButton.gameObject.SetActive(true);
            RollButton.interactable = false;
            PlayButton.gameObject.SetActive(true);
            PlayButton.interactable = false;
        }

        spawnPlayerInRoomLobby();
        StartCoroutine(activateLeaveRoomButton());
    }
    
    void Update() {
        if(!PhotonNetwork.IsConnected) {
            onPlayerDisconnected("Connection lost");
        }
        
        if(PhotonNetwork.CurrentRoom.PlayerCount != Rooms.expectedMaxPlayer && isRoomFull) {
            onPlayerDisconnected("Other player leave the room");
        }

        if(RollButton.interactable == true)
        {
            LeaveRoomButton.interactable = false;
        }

        checkPlayerInRoom();
        activatePlayButton();
    }

    void spawnPlayerInRoomLobby() {
        if(PhotonNetwork.CurrentRoom.PlayerCount.Equals(1)) {
            player1Avatar = PhotonNetwork.Instantiate("Player/Player1Avatar", player1SpawnLocation.transform.position, player1SpawnLocation.transform.rotation, 0);
            PlayerScript.PlayerNumber = 1;
        } else if(PhotonNetwork.CurrentRoom.PlayerCount.Equals(2)) {
            player2Avatar = PhotonNetwork.Instantiate("Player/Player2Avatar", player2SpawnLocation.transform.position, player2SpawnLocation.transform.rotation, 0);
            PlayerScript.PlayerNumber = 2;
        } else if(PhotonNetwork.CurrentRoom.PlayerCount.Equals(3)) {
            player3Avatar = PhotonNetwork.Instantiate("Player/Player3Avatar", player3SpawnLocation.transform.position, player3SpawnLocation.transform.rotation, 0);
            PlayerScript.PlayerNumber = 3;
        } else if(PhotonNetwork.CurrentRoom.PlayerCount.Equals(4)) {
            player4Avatar = PhotonNetwork.Instantiate("Player/Player4Avatar", player4SpawnLocation.transform.position, player4SpawnLocation.transform.rotation, 0);
            PlayerScript.PlayerNumber = 4;
        }
    }

    public void OnClick_PlayButton() {
        //SoundManager.PlaySoundEffect("ButtonClick");
        string sceneName = "Gameplay";
        object[] sceneNameData = new object[] { sceneName };
        PhotonNetwork.RaiseEvent(RaiseEventCode.ENTER_THE_GAME, sceneNameData, RaiseEventOptions.Default, SendOptions.SendReliable);

        StartGame(sceneName);
    }

    public void OnClick_LeaveRoomButton() {
        //SoundManager.PlaySoundEffect("ButtonClick");
        StartCoroutine(leaveRoom());
    }

    IEnumerator leaveRoom() {
        PhotonNetwork.LeaveRoom();
        if(PhotonNetwork.CurrentRoom.Name != null)
            yield return null;
        sceneLoader.LoadScene("Mainmenu", SoundManager.sfxLength);
        // SceneManager.LoadScene("Mainmenu");
    }

    public void StartGame(string sceneName) {
        RollPhase.isRollPhaseDone = false;
        sceneLoader.LoadScene(sceneName, SoundManager.sfxLength);
    }

    void checkPlayerInRoom() {
        if(PhotonNetwork.CurrentRoom.PlayerCount == expectedMaxPlayer && !isRoomFull) {
            isRoomFull = true;
            object[] roomFullStateData = new object[] { isRoomFull };
            PhotonNetwork.RaiseEvent(RaiseEventCode.ALL_PLAYERS_IN_ROOM, roomFullStateData, RaiseEventOptions.Default, SendOptions.SendReliable);

            allPlayersInRoom(isRoomFull);
        }
    }

    void activatePlayButton() {
        if(PhotonNetwork.IsMasterClient) {
            if(RollPhase.isRollPhaseDone == true) {
                RollButton.interactable = false;
                PlayButton.interactable = true;
            } else {
                PlayButton.interactable = false;
            }
        }
    }

    IEnumerator activateLeaveRoomButton() {
        yield return new WaitForSeconds(10f);
        LeaveRoomButton.interactable = true;
    }

    void allPlayersInRoom(bool isRoomFull) {
        if(isRoomFull) {
            if(PhotonNetwork.IsMasterClient) {
                RollButton.interactable = true;
            }
        }
    }

    public override void OnEnable() {
        PhotonNetwork.NetworkingClient.EventReceived += NetworkingClient_EventReceived;
    }

    public override void OnDisable() {
        PhotonNetwork.NetworkingClient.EventReceived -= NetworkingClient_EventReceived;
    }

    private void NetworkingClient_EventReceived(EventData obj) {
        if(obj.Code == RaiseEventCode.ALL_PLAYERS_IN_ROOM) {
            object[] playerStatusData = (object[])obj.CustomData;
            bool isRoomFull = (bool)playerStatusData[0];

            allPlayersInRoom(isRoomFull);
        } 
        else if(obj.Code == RaiseEventCode.ENTER_THE_GAME) {
            object[] sceneNameData = (object[])obj.CustomData;
            string sceneName = (string)sceneNameData[0];

            StartGame(sceneName);
        }
    }

    void onPlayerDisconnected(string message) {
        ConnectionLostAlert.DisconnectedFromScene = SceneManager.GetActiveScene().name;
        ConnectionLostAlert.DisconnectCauses = message;
        SceneManager.LoadScene("ConnLostAlert");
    }
}
