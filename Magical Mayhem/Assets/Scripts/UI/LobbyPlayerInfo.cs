using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class LobbyPlayerInfo : MonoBehaviour
{
    [SerializeField] private TMP_Text userName;
    [SerializeField] private TMP_Text className;

    private UnitClass unitClass;
    private ulong clientId;

    public void Initialize(UnitClass unitClass, string userName){
        this.unitClass = unitClass;
        this.userName.text = userName;
        this.className.text = unitClass.name;

    }
}
