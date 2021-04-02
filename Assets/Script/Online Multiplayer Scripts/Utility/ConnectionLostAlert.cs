using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

using Photon.Realtime;
using Photon.Pun;
using ExitGames.Client.Photon;

public class ConnectionLostAlert : MonoBehaviourPunCallbacks
{
    public GameObject ConnLostAlertOnly, ConnLostAlertWithReconnButton, ReconnectAlert;
    public Text ErrorMessageTextReconAlert, ErrorMessageTextAlertOnly, WarningMessageTextAlertOnly, ReconnectResultText;
    public Button OkButton;
    public static string DisconnectedFromScene;
    public static string DisconnectCauses;

    bool isReconnected = false;

    void Start()
    {
        if(DisconnectedFromScene == "Login" || DisconnectedFromScene == "Mainmenu" || DisconnectedFromScene == "Tutorial" || DisconnectedFromScene == "SelectMode")
        {
            if(DisconnectCauses == "Create or Join Room Failed") {
                ConnLostAlertOnly.SetActive(true);
                WarningMessageTextAlertOnly.text = "Gagal masuk ke dalam room!\n Silahkan periksa koneksi anda.";
                ErrorMessageTextAlertOnly.text = "Err Message : " + DisconnectCauses;
            }
            else
            {
                ConnLostAlertWithReconnButton.SetActive(true);
                ErrorMessageTextReconAlert.text = "Err Message : " + DisconnectCauses;
                Debug.Log("Connected status : " + PhotonNetwork.IsConnectedAndReady);
            }
        } else
        {
            ConnLostAlertOnly.SetActive(true);

            if (DisconnectCauses == "Other player disconnected")
            {
                WarningMessageTextAlertOnly.text = "Jaringan pemain lain terputus\n Permainan dihentikan!";
            }
            else if (DisconnectCauses == "Other player leave the room")
            {
                WarningMessageTextAlertOnly.text = "Pemain lain keluar dari room\n Permainan dihentikan!";
            }
            else if (DisconnectCauses == "Connection lost" && DisconnectedFromScene == "Gameplay")
            {
                WarningMessageTextAlertOnly.text = "Terjadi Kesalahan Jaringan\n Permainan dihentikan!";
            }

            ErrorMessageTextAlertOnly.text = "Err Message : " + DisconnectCauses;
            Debug.Log("Connected status : " + PhotonNetwork.IsConnectedAndReady);
        }
    }

    public void OKButton()
    {
        Rooms.expectedMaxPlayer = 0;
        Rooms.isRoomFull = false;
        Gameover.WinnerNickname = null;
        Constant.FirstTurnPlayerNickname = "Player 1";
        Constant.SecondTurnPlayerNickname = "Player 2";
        Constant.ThirdTurnPlayerNickname = "Player 3";
        Constant.FourthTurnPlayerNickname = "Player 4";

        if(DisconnectCauses == "Create or Join Room Failed")
        {
            SceneManager.LoadScene("Mainmenu");
        }
        else if(PhotonNetwork.IsConnectedAndReady)
        {
            StartCoroutine(leaveRoom());
        } 
        else
        {
            SceneManager.LoadScene("Mainmenu");
            StartCoroutine(reconnectToPhotonServer());
        }
    }

    IEnumerator leaveRoom() {
        PhotonNetwork.LeaveRoom();
        if(PhotonNetwork.CurrentRoom.Name != null)
            yield return null;
        SceneManager.LoadScene("Mainmenu");
    }

    public void ReconnectButton()
    {
        ConnLostAlertWithReconnButton.SetActive(false);
        StartCoroutine(reconnectToPhotonServer());
    }

    public void ExitButton()
    {
        Application.Quit();
    }

    public void BackButton()
    {
        SceneManager.LoadScene(DisconnectedFromScene);
    }

    IEnumerator reconnectToPhotonServer() {
        PhotonNetwork.Reconnect();
        if(!PhotonNetwork.IsConnectedAndReady)
            yield return null;
        if(ConnLostAlertOnly.activeSelf == false)
        {
            ConnLostAlertWithReconnButton.SetActive(false);
        } 
        else
        {
            ConnLostAlertOnly.SetActive(false);
        }
        ReconnectAlert.SetActive(true);
        ReconnectResultText.text = "Sukses terhubung kembali!";
    }
}
