using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GetDropdownValue : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField] TMP_Dropdown drpdwn;

    public void GetValue(){
        int Pickedindex = drpdwn.value;
        if(drpdwn.options[0].text == "Map size" ){
            LobbySystem.mapSize = Pickedindex;
        }
        else{
            LobbySystem.mapType = Pickedindex;
        }
    }
}
