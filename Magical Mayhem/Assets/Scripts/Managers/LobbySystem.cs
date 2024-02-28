using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class LobbySystem : NetworkBehaviour
{
    [SerializeField] private NetworkObject playerTemplate;
    [SerializeField] private TMP_Text ipText;
    [SerializeField] private RectTransform playerInfoSpawnPoint;
    [SerializeField] private Button startLobbyButton;
    [SerializeField] private Button leaveLobbyButton;
    private void Awake()
    {
        leaveLobbyButton.onClick.AddListener(LeaveButton);
        startLobbyButton.onClick.AddListener(StartLobbyButton);

    }
    void Start()
    {
        NetworkManager.Singleton.OnClientConnectedCallback += OnClientConnectedCallback;
        NetworkManager.Singleton.OnClientDisconnectCallback += OnClientDisconnectCallback;
        if (IsHost)
        {
            print("Hosting");
            NetworkObject t = Instantiate(playerTemplate);
            t.SpawnAsPlayerObject(0, true);
            t.TrySetParent(playerInfoSpawnPoint, false);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    /// <summary>
    /// Is called when a client connects. It spawns the playerprefab for them and gets the reference to their UnitController. Server Only. - Silas Thule
    /// </summary>
    /// <param name="clientId"></param>
    private void OnClientConnectedCallback(ulong clientId)
    {
        if (!IsServer) return;
        Debug.Log("PlayerId: " + clientId + " has joined");
        NetworkObject player = Instantiate(playerTemplate,playerInfoSpawnPoint);
        player.SpawnAsPlayerObject(clientId, true);
        player.TrySetParent(playerInfoSpawnPoint, false);
        // units.Add(unit);
        // if (units.Count > 1)
        // {
        //     startButton.gameObject.SetActive(true);
        // }
    }
    /// <summary>
    /// Is called when a client disconnects. It despawns the playerprefab for them and removes the reference to their UnitController. Server Only. - Silas Thule
    /// </summary>
    /// <param name="clientId"></param>
    private void OnClientDisconnectCallback(ulong clientId)
    {
        
        if (!IsServer) return;
        NetworkObject player = NetworkManager.Singleton.ConnectedClients[clientId].PlayerObject;
        player.Despawn(true);
        Destroy(player.gameObject);
        // units.Remove(unit);
        // if (units.Count < 2)
        // {
        //     startButton.gameObject.SetActive(false);
        // }
    }
    private void LeaveButton()
    {
        NetworkManager.Singleton.Shutdown();
        SceneHelper.instance.LoadScene("Connection Screen");
    }
    private void StartLobbyButton()
    {
        if (NetworkManager.Singleton.ConnectedClientsList.Count>0)
        {
            StartGame();
        }
        else
        {
            Debug.Log("Not enough players");
        }
    }
    private void StartGame()
    {
        foreach (NetworkClient player in NetworkManager.Singleton.ConnectedClientsList)
        {
            player.PlayerObject.Despawn();
        }
        NetworkManager.Singleton.SceneManager.LoadScene("GameScreen", UnityEngine.SceneManagement.LoadSceneMode.Single);
    }
}
