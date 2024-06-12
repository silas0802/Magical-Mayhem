using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Netcode;
using UnityEngine;

public class LobbyPlayerInfo : NetworkBehaviour
{
    [SerializeField] private TMP_Text userName;
    [SerializeField] private TMP_Text className;
    public string sName;

    

    
    public void Initialize(string userName){
        this.userName.text = userName;

    }

    [ClientRpc]
    public void SetNameClientRPC(string name){
        userName.text = name;
    }
}
