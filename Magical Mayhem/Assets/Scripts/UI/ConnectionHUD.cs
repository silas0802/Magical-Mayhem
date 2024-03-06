using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using TMPro;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ConnectionHUD : MonoBehaviour
{
    [SerializeField] private Button joinServer;
    [SerializeField] private Button host;
    [SerializeField] private Button join;
    [SerializeField] private TMP_InputField ipField;

    const string LOCALHOST = "127.0.0.1";

    private void Awake()
    {
        joinServer.onClick.AddListener(JoinServer);
        host.onClick.AddListener(HostGame);
        join.onClick.AddListener(JoinGame);
    }

    public void SetConnectionData(string IP)
    {
        string input = ipField.text;
        if (input == null || input == "")
        {
            input = GetLocalIPv4();
        }
        NetworkManager.Singleton.GetComponent<UnityTransport>().SetConnectionData(
            input,  // The IP address is a string
            12345, // The port number is an unsigned short
            "0.0.0.0" // The server listen address is a string. 0.0.0.0 makes it listen for everything
        );
    }
    private void JoinServer()
    {
        SetConnectionData(LOCALHOST);
        print(NetworkManager.Singleton.StartClient());
        if (NetworkManager.Singleton.IsConnectedClient)
        {
            SceneManager.LoadScene("Lobby Screen");
        }
        else
        {
            Debug.Log("Cant join");
            NetworkManager.Singleton.Shutdown();
        }
    }
    private void JoinGame()
    {
        SetConnectionData(ipField.text);
        NetworkManager.Singleton.StartClient();
        SceneManager.LoadScene("Lobby Screen");
        
    }
    private void HostGame()
    {
        print(GetLocalIPv4());
        SetConnectionData(LOCALHOST);
        NetworkManager.Singleton.StartHost();
        NetworkManager.Singleton.SceneManager.LoadScene("Lobby Screen",UnityEngine.SceneManagement.LoadSceneMode.Single);
    }
    public string GetLocalIPv4()// Taken from https://discussions.unity.com/t/get-the-device-ip-address-from-unity/235351
    {
        return Dns.GetHostEntry(Dns.GetHostName())
        .AddressList.First(
        f => f.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
        .ToString();
    }
}
