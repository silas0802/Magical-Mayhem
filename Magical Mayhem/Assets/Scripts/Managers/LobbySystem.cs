using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LobbySystem : NetworkBehaviour
{
    public static LobbySystem instance;
    //public static Dictionary<ulong,string> names = new Dictionary<ulong, string>();
    [SerializeField] private NetworkObject playerTemplate;
    [SerializeField] private TMP_Text ipText;
    [SerializeField] private RectTransform playerInfoSpawnPoint;
    [SerializeField] private Button startLobbyButton;
    [SerializeField] private Button leaveLobbyButton;
    [SerializeField] private TMP_Dropdown mapSizeDrop;
    [SerializeField] private TMP_Dropdown mapTypeDrop;
    [SerializeField] private TMP_Dropdown NumOfRoundsDrop;
    [SerializeField] private TMP_Dropdown botDifficultyDrop;
    [SerializeField] private Sprite[] mapSprites;
    [SerializeField] private Image mapImage;
    [SerializeField] private Button addBotButton;
    [SerializeField] private Button removeBotButton;
    [SerializeField] private TMP_Text numberofBots;
    

    [SerializeField] private Toggle buffsToggle;
    //public static List<LobbyPlayerInfo> playerList = new List<LobbyPlayerInfo>();
    private static int mapSize = 0;
    private static int mapType = 0;
    private static bool buffs = true;
    private static int numOfRounds = 3;
    private static int botNumbers = 0;
    private static int botDifficulty = 0;

    private float updateNameTimer;
    
    private void Awake()
    {
        if (instance == null){
            instance = this;
            mapImage.sprite = instance.mapSprites[0];
        }
        else{
            Destroy(gameObject);
        }
        leaveLobbyButton.onClick.AddListener(LeaveButton);
        startLobbyButton.onClick.AddListener(StartLobbyButton);
        addBotButton.onClick.AddListener(BotIncrementer);
        removeBotButton.onClick.AddListener(BotDecrementer);
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
            string name;
            //names.TryGetValue(99,out name);
            //t.GetComponent<LobbyPlayerInfo>().sName = name;
            ipText.text = ConnectionHUD.GetLocalIPv4();
            //playerList = new List<LobbyPlayerInfo>();
            //playerList.Add(t.GetComponent<LobbyPlayerInfo>());
            SetPlayersNames();
        }
        else
        {
            print("Joined successfully");
            startLobbyButton.gameObject.SetActive(false);
            mapSizeDrop.gameObject.SetActive(false);
            mapTypeDrop.gameObject.SetActive(false);
            buffsToggle.gameObject.SetActive(false);
            NumOfRoundsDrop.gameObject.SetActive(false);
            botDifficultyDrop.gameObject.SetActive(false);
            addBotButton.gameObject.SetActive(false);
            removeBotButton.gameObject.SetActive(false);
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
        //string name;
        //names.TryGetValue(clientId,out name);
        player.SpawnAsPlayerObject(clientId, true);
        player.TrySetParent(playerInfoSpawnPoint, false);

        //player.GetComponent<LobbyPlayerInfo>().sName = name;
        //playerList.Add(player.GetComponent<LobbyPlayerInfo>());

        SetPlayersNames();
    }

    void SetPlayersNames()
    {
        // foreach (LobbyPlayerInfo p in playerList)
        // {
        //     p?.SetNameClientRPC(p.sName);
        // }
    }
    /// <summary>
    /// Is called when a client disconnects. It despawns the playerprefab for them and removes the reference to their UnitController. Server Only. - Silas Thule
    /// </summary>
    /// <param name="clientId"></param>
    private void OnClientDisconnectCallback(ulong clientId)
    {
        
        if (!IsServer) return;
        try
        {
            NetworkObject player = NetworkManager.Singleton.ConnectedClients[clientId].PlayerObject;
            //playerList.Remove(player.GetComponent<LobbyPlayerInfo>());
            player.Despawn(true);
            Destroy(player.gameObject);
            // units.Remove(unit);
            // if (units.Count < 2)
            // {
            //     startButton.gameObject.SetActive(false);
            // }
        }
        catch (System.Exception)
        {
          
        }
        
    }
    private void LeaveButton()
    {
        NetworkManager.Singleton.Shutdown();
        SceneManager.LoadScene("Connection Screen");
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

    private void BotIncrementer()
    {
        botNumbers += 1;
        numberofBots.text = botNumbers.ToString();
    }

    private void BotDecrementer()
    {
        botNumbers -= 1;
        numberofBots.text = botNumbers.ToString();
    }

    private void StartGame()
    {
        foreach (NetworkClient player in NetworkManager.Singleton.ConnectedClientsList)
        {
            player.PlayerObject.Despawn();
        }
        NetworkManager.Singleton.SceneManager.LoadScene("GameScreen", UnityEngine.SceneManagement.LoadSceneMode.Single);
    }

    //getters and setters
    public static void SetBuffs(){
        buffs = !buffs;
    }

    public static bool GetBuffs(){
        return buffs;
    }

    public static void SetMapType(int x){
        mapType = x;
        if (instance != null)
        {
            instance.mapImage.sprite = instance.mapSprites[x];
        }
    }

    public static void SetBotDifficulty(int x){
        botDifficulty = x;
    }

    public static int GetBotDifficulty(){
        return botDifficulty;
    }

    public static void SetBotNumber(int x){
        botNumbers = x;
    }

    public static int GetBotNumber(){
        return botNumbers;
    }

    public static int GetMapType(){
        return mapType;
    }

    public static void SetMapSize(int x){
        mapSize = x;
    }

    public static int GetMapSize(){
        return mapSize;
    }

    public static void SetNumOfRounds(int x){
        numOfRounds = x;
    }

    public static int GetNumOfRounds(){
        return numOfRounds;
    }
}


