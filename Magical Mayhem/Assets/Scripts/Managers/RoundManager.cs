using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
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
    public bool isDebugging = false;
    [SerializeField] private Brain botBrain;
    [SerializeField] private int roundNumber = 0;
    [SerializeField] private int shoppingTime = 60;

    public bool roundIsOngoing { get; private set; }
    [SerializeField] private List<UnitController> units = new List<UnitController>();
    [SerializeField] private List<KillData> kills = new List<KillData>();
    [Header("References")]
    [SerializeField] private NetworkObject playerPrefab;
    
    public List<UnitController> GetUnits => units;
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            
            if (UnitController.OnUnitDeath == null)
            {
                UnitController.OnUnitDeath = new KillEvent();
            }
        }
        else
        {
            Destroy(gameObject);
        }
       
    }
    void Start()
    {
        //Subscribes to Connection Events
        if (NetworkManager.Singleton == null) return;
        NetworkManager.Singleton.OnClientConnectedCallback += OnClientConnectedCallback;
        NetworkManager.Singleton.OnClientDisconnectCallback += OnClientDisconnectCallback;
        UnitController.OnUnitDeath.AddListener(OnUnitDeath);
        OnStart();
        StartCoroutine(BeforeShopPhase());
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
        foreach (UnitController unit in units)
        {
            unit.ConnectUnitToShopClientRPC();
        }
        roundIsOngoing = false;
        OpenPlayerShopsClientRPC();
        StartCoroutine(ShoppingPhaseCoroutine());
    }
    /// <summary>
    /// Resets the health of units and their position. Also enables damage.
    /// </summary>
    private void StartNewRound()
    {

        if (!IsServer) return;
        MapGenerator.instance.ResetMap();
        kills.Clear();
        roundNumber++;
        foreach (UnitController unit in units)
        {
            unit.ResetHealth();
        }
        MapGenerator.instance.GenerateMap(1,1,"Medium");
        PlaceUnits();
        roundIsOngoing = true;

    }
    /// <summary>
    /// Places all the players on the map
    /// </summary>
    private void PlaceUnits()
    {
        if (!IsServer) return;
        
        foreach (UnitController unit in units)
        {

            Vector3 radius = new Vector3(UnityEngine.Random.Range(-1.0f, 1.0f),0, UnityEngine.Random.Range(-1.0f, 1.0f));
            radius = radius.normalized * 10;
            unit.transform.position = radius;
            unit.GetComponent<Rigidbody>().velocity = Vector3.zero;
            unit.unitMover.SetTargetPosition(radius);
            unit.GetComponent<UnitController>().unitMover.canMove = true;

        }
        //throw new NotImplementedException();
    }
    /// <summary>
    /// Opens the shop for all players
    /// </summary>
    [ClientRpc]
    private void OpenPlayerShopsClientRPC()
    {
        SpellShop.instance.SetTimer(shoppingTime);
        SpellShop.instance.gameObject.SetActive(true);
    }
    [ClientRpc]
    private void ClosePlayerShopsClientRPC(){
        SpellShop.instance.gameObject.SetActive(false);
    }
    /// <summary>
    /// Waits for some time then starts a new round
    /// </summary>
    /// <returns></returns>
    private IEnumerator ShoppingPhaseCoroutine()
    {

        yield return new WaitForSeconds(shoppingTime);
        ClosePlayerShopsClientRPC();
        StartNewRound();
        
    }
    /// <summary>
    /// Waits for some time then starts the shopping phase
    /// </summary>
    /// <returns></returns>
    private IEnumerator BeforeShopPhase()
    {
        yield return new WaitForSeconds(2);
        StartShoppingPhase();
    }
    /// <summary>
    /// Event that is called on every kill. Handles ending the round.
    /// </summary>
    /// <param name="kill">The information about the kill</param>
    public void OnUnitDeath(KillData kill)
    {
        if (!IsServer) return;
        kills.Add(kill);

        if (units.Count-kills.Count<=1) // If there is only one unit left
        {
            StartCoroutine(BeforeShopPhase());
        }

    }
    /// <summary>
    /// Adds a bot with a given brain to the game
    /// </summary>
    /// <param name="brain"></param>
    public void AddBot(Brain brain)
    {
        if (!IsServer) return;
        NetworkObject bot = Instantiate(playerPrefab,new Vector3 (0,0,0),Quaternion.identity);
        bot.Spawn();
        bot.GetComponent<UnitController>().InitializeBot(brain);
        units.Add(bot.GetComponent<UnitController>());
    }
    /// <summary>
    /// Initializes all player Units
    /// </summary>
    private void OnStart()
    {
        if (!IsServer) return;
        foreach (ulong player in NetworkManager.Singleton.ConnectedClientsIds) //Spawn player objects
        {
            NetworkObject prefab = Instantiate(playerPrefab,new Vector3 (10f,0,10f),Quaternion.identity);
            prefab.SpawnAsPlayerObject(player, true);
            UnitController unit = prefab.GetComponent<UnitController>();
            units.Add(unit);
            unit.unitMover.canMove = false;
        }

        AddBot(botBrain);
    }

}
