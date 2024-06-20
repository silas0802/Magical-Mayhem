using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class LobbyPlayerInfo : NetworkBehaviour
{
    [SerializeField] private TMP_Text userName;
    [SerializeField] private TMP_Text className;
    [SerializeField] private Button removeBotButton;

    public string sName;

    public void Initialize(string userName)
    {
        this.userName.text = userName;
    }

    [ClientRpc]
    public void SetNameClientRPC(string name)
    {
        if (userName != null)
        {
            userName.text = name;
        }
    }
}
