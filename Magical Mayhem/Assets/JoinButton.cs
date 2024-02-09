using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Networking;

public class JoinButton : MonoBehaviour
{

    public void JoinGame(){
        NetworkManager.Singleton.StartClient();
    }
}
