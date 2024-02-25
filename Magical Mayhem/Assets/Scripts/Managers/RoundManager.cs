using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
/// <summary>
/// A Singleton class that controls the game flow. - Silas Thule
/// </summary>
public class RoundManager : NetworkBehaviour
{
    public static RoundManager instance;

    [SerializeField] private Brain botBrain;
    [SerializeField] private int roundNumber = 0;
    [SerializeField] private int shoppingTime = 60;

    public bool roundIsOngoing { get; private set; }
    [SerializeField] private List<UnitController> units = new List<UnitController>();
    [SerializeField] private List<KillData> kills = new List<KillData>();
    [Header("References")]
    [SerializeField] private NetworkObject playerPrefab;
    [SerializeField] private Button startButton;
    
    public List<UnitController> GetUnits => units;
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);

            //Setup the start lobby button
            startButton.onClick.AddListener(() =>
            {
                if (units.Count > 1)
                {
                    StartShoppingPhase();
                    AddBot();
                    startButton.gameObject.SetActive(false);
                }
                else
                {
                    throw new NotSupportedException("At least 2 players needed in lobby to start game.");
                }
            });

            startButton.gameObject.SetActive(false);
            if (UnitController.OnUnitDeath == null)
            {
                UnitController.OnUnitDeath = new KillEvent();
            }
        }
       
    }
    void Start()
    {
        //Subscribes to Connection Events
        NetworkManager.Singleton.OnClientConnectedCallback += OnClientConnectedCallback;
        NetworkManager.Singleton.OnClientDisconnectCallback += OnClientDisconnectCallback;
        UnitController.OnUnitDeath.AddListener(OnUnitDeath);
    }
    
    /// <summary>
    /// Is called when a client connects. It spawns the playerprefab for them and gets the reference to their UnitController. Server Only. - Silas Thule
    /// </summary>
    /// <param name="clientId"></param>
    private void OnClientConnectedCallback(ulong clientId)
    {
        if (!IsServer) return;
        
        NetworkObject player = Instantiate(playerPrefab);
        player.SpawnAsPlayerObject(clientId, false);
        UnitController unit = player.GetComponent<UnitController>();
        units.Add(unit);
        if (units.Count > 1)
        {
            startButton.gameObject.SetActive(true);
        }
    }
    /// <summary>
    /// Is called when a client disconnects. It despawns the playerprefab for them and removes the reference to their UnitController. Server Only. - Silas Thule
    /// </summary>
    /// <param name="clientId"></param>
    private void OnClientDisconnectCallback(ulong clientId)
    {
        
        if (!IsServer) return;
        NetworkObject player = NetworkManager.Singleton.ConnectedClients[clientId].PlayerObject;
        UnitController unit = player.GetComponent<UnitController>();
        units.Remove(unit);
        player.Despawn(true);
        Destroy(player.gameObject);
        if (units.Count < 2)
        {
            startButton.gameObject.SetActive(false);
        }
    }

    /// <summary>
    /// Finds nearest unit that is not self. - Silas Thule
    /// </summary>
    /// <param name="position"></param>
    /// <param name="self"></param>
    /// <returns></returns>
    public UnitController FindNearestUnit(Vector3 position, UnitController self)
    {
        UnitController closest = null;
        float distance = 1000;
        foreach (UnitController unit in units)
        {
            if (unit != self)
            {
                if ((unit.transform.position - position).magnitude < distance)
                {
                    closest = unit;
                    distance = (unit.transform.position - position).magnitude;
                }
            }
        }
        return closest;
    }
    /// <summary>
    /// Opens the shop for all players for a given time and waits for some time before closing the shop and starting the next round.  - Silas Thule
    /// </summary>
    public void StartShoppingPhase()
    {
        if (!IsServer) return;
        roundIsOngoing = false;
        OpenPlayerShopsClientRPC();
        StartCoroutine(ShoppingPhaseCoroutine());
    }
    /// <summary>
    /// Resets the health of units and their position. Also enables damage.
    /// </summary>
    public void StartNewRound()
    {
        if (!IsServer) return;
        kills.Clear();
        roundNumber++;
        foreach (UnitController unit in units)
        {
            unit.ResetHealth();
        }
        //mapgen.instence.Genmap
        //PlaceUnits();
        //Call Map Generator function via MapGenerator.instance.GenerateMap();
        roundIsOngoing = true;
        Debug.Log("New round has started");

    }
    /// <summary>
    /// Places all the players on the map
    /// </summary>
    private void PlaceUnits()
    {
        if (!IsServer) return;
        throw new NotImplementedException();
    }
    /// <summary>
    /// Opens the shop for all players
    /// </summary>
    [ClientRpc]
    private void OpenPlayerShopsClientRPC()
    {
        Debug.Log("Shopping Phase started");
        SpellShop.instance.SetTimer(shoppingTime);
        SpellShop.instance.gameObject.SetActive(true);
    }
    /// <summary>
    /// Waits for some time then starts a new round
    /// </summary>
    /// <returns></returns>
    IEnumerator ShoppingPhaseCoroutine()
    {

        yield return new WaitForSeconds(shoppingTime);
        StartNewRound();
        
    }
    IEnumerator BeforeShopPhase()
    {
        yield return new WaitForSeconds(2);
        StartShoppingPhase();
    }
    /// <summary>
    /// Event that is called on every kill. Handles ending the round.
    /// </summary>
    /// <param name="deadUnit"></param>
    /// <param name="killer"></param>
    public void OnUnitDeath(KillData kill)
    {
        if (!IsServer) return;
        kills.Add(kill);

        if (units.Count-kills.Count<=1) // If there is only one unit left
        {
            StartCoroutine(BeforeShopPhase());
        }

    }
    public void AddBot()
    {
        if (!IsServer) return;
        NetworkObject bot = Instantiate(playerPrefab,new Vector3 (0,0,0),Quaternion.identity);
        bot.Spawn();
        bot.GetComponent<UnitController>().InitializeBot(botBrain);
    }

}
