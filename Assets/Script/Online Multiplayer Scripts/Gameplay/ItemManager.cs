using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.UI;

using Photon.Realtime;
using Photon.Pun;
using Photon.Pun.UtilityScripts;
using ExitGames.Client.Photon;

public class ItemManager : MonoBehaviourPunCallbacks
{
    CardManager cardManager;
    GameplayManager gameplayManager;

    void Start() {
        gameplayManager = GameObject.Find("GameManager").GetComponent<GameplayManager>();
        cardManager = GameObject.Find("CardManager").GetComponent<CardManager>();
    }

    void Update() {
        
    }
}
