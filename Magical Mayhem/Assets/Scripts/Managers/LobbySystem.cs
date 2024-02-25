using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class LobbySystem : NetworkBehaviour
{
    [SerializeField] private NetworkObject playerTemplate;
    [SerializeField] private TMP_Text ipText;
    [SerializeField] private Transform playerInfoSpawnPoint;
    [SerializeField] private Button startLobbyButton;
    [SerializeField] private Button leaveLobbyButton;
    // Start is called before the first frame update
    void Start()
    {
        
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
        NetworkObject player = Instantiate(playerTemplate,playerInfoSpawnPoint);
        player.SpawnAsPlayerObject(clientId, true);
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
}
