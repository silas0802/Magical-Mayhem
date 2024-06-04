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
    [SerializeField] private TMP_InputField userNameField;

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
        NetworkManager.Singleton.NetworkConfig.ConnectionData = System.Text.ASCIIEncoding.ASCII.GetBytes(userNameField.text);
        NetworkManager.Singleton.StartClient();
        SceneManager.LoadScene("Lobby Screen");
        
    }
    private void HostGame()
    {
        print(GetLocalIPv4());
        SetConnectionData(LOCALHOST);
        NetworkManager.Singleton.ConnectionApprovalCallback = ApprovalCheck;
        NetworkManager.Singleton.StartHost();
        LobbySystem.names.Add(99,userNameField.text);
        NetworkManager.Singleton.SceneManager.LoadScene("Lobby Screen",UnityEngine.SceneManagement.LoadSceneMode.Single);
    }
    private void ApprovalCheck(NetworkManager.ConnectionApprovalRequest request, NetworkManager.ConnectionApprovalResponse response)
    {
        // The client identifier to be authenticated
        ulong clientId = request.ClientNetworkId;

        // Additional connection data defined by user code
        string connectionData = System.Text.Encoding.UTF8.GetString(request.Payload);
        LobbySystem.names.Add(clientId,connectionData);

        // Your approval logic determines the following values
        response.Approved = true;
        response.CreatePlayerObject = false;

        // The Prefab hash value of the NetworkPrefab, if null the default NetworkManager player Prefab is used
        response.PlayerPrefabHash = null;

        // Position to spawn the player object (if null it uses default of Vector3.zero)
        response.Position = Vector3.zero;

        // Rotation to spawn the player object (if null it uses the default of Quaternion.identity)
        response.Rotation = Quaternion.identity;

        // If response.Approved is false, you can provide a message that explains the reason why via ConnectionApprovalResponse.Reason
        // On the client-side, NetworkManager.DisconnectReason will be populated with this message via DisconnectReasonMessage
        response.Reason = "Some reason for not approving the client";

        // If additional approval steps are needed, set this to true until the additional steps are complete
        // once it transitions from true to false the connection approval response will be processed.
        response.Pending = false;
    }
    public static string GetLocalIPv4()// Taken from https://discussions.unity.com/t/get-the-device-ip-address-from-unity/235351
    {
        return Dns.GetHostEntry(Dns.GetHostName())
        .AddressList.First(
        f => f.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
        .ToString();
    }
}
