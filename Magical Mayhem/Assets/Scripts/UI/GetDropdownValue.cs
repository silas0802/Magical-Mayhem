using TMPro;
using UnityEngine;

public class GetDropdownValue : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField] TMP_Dropdown drpdwn;

    public void GetValue(){
        int Pickedindex = drpdwn.value;
        if(drpdwn.options[0].text == "Map size" ){
            LobbySystem.SetMapSize(Pickedindex);
        }
        else if(drpdwn.options[0].text == "Map type"){
            LobbySystem.SetMapType(Pickedindex);
        }
        else{
            LobbySystem.SetNumOfRounds(Pickedindex + 4);
        }
    }
}
