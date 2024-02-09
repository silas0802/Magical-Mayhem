using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using UnityEngine;
using UnityEngine.Networking;

public class JoinButton : MonoBehaviour
{
    const string LOCALHOST = "127.0.0.1"; 
    const string IP = "192.168.3.71";
    public void JoinGame(){
        NetworkManager.Singleton.GetComponent<UnityTransport>().SetConnectionData(
            IP,  // The IP address is a string
            (ushort)12345, // The port number is an unsigned short
            "0.0.0.0" // The server listen address is a string. 0.0.0.0 makes it listen for everything
        );
        NetworkManager.Singleton.StartClient();
    }
    public void HostGame(){
        NetworkManager.Singleton.GetComponent<UnityTransport>().SetConnectionData(
            IP,  // The IP address is a string
            (ushort)12345, // The port number is an unsigned short
            "0.0.0.0" // The server listen address is a string. 0.0.0.0 makes it listen for everything
        );
        NetworkManager.Singleton.StartHost();
    }
}
