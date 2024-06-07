using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Netcode;
using UnityEngine;

public class LobbyPlayerInfo : NetworkBehaviour
{
    [SerializeField] private TMP_Text userName;
    public NetworkVariable<string> uName = new NetworkVariable<string>();
    [SerializeField] private TMP_Text className;

    private UnitClass unitClass;
    private ulong clientId;

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        uName.OnValueChanged += SetName;
    }
    public override void OnNetworkDespawn()
    {
        base.OnNetworkDespawn();
    }
    public void Initialize(UnitClass unitClass, string userName){
        this.unitClass = unitClass;
        this.userName.text = userName;
        this.className.text = unitClass.name;

    }
    public void SetName(string before, string after){
        userName.text = after;
    }
}
