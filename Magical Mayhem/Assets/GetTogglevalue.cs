using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class GetTogglevalue : MonoBehaviour
{
    [SerializeField] Toggle toggle; 
    // Start is called before the first frame update
    public void getToggleValue(){
        LobbySystem.buffs = toggle.isOn;
    }
}
