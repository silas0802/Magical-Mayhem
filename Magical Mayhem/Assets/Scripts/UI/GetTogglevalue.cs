using UnityEngine;
using UnityEngine.UI;

public class GetTogglevalue : MonoBehaviour
{
    [SerializeField] Toggle toggle; 
    public void getToggleValue(){
        LobbySystem.SetBuffs();
    }
}
