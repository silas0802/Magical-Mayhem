using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using UnityEngine;
using UnityEngine.Networking;

public class ConnectionHUD : MonoBehaviour
{
    public TMP_InputField inputField;
    const string LOCALHOST = "127.0.0.1"; 
    public void JoinGame(){
        SetConnectionData();
        NetworkManager.Singleton.StartClient();
    }
    public void HostGame(){
        SetConnectionData();
        NetworkManager.Singleton.StartHost();
    }

    public void SetConnectionData()
    {
        string input = inputField.text;
        if (input == null || input == "")
        {
            print("Lockal");
            input = LOCALHOST;
        }
        NetworkManager.Singleton.GetComponent<UnityTransport>().SetConnectionData(
            input,  // The IP address is a string
            12345, // The port number is an unsigned short
            "0.0.0.0" // The server listen address is a string. 0.0.0.0 makes it listen for everything
        );
    }
}
