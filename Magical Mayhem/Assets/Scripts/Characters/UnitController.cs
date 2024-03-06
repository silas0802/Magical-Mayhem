using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
/// <summary>
/// Controls all the behaviour of a unit. - Silas Thule
/// </summary>
[RequireComponent(typeof(UnitCaster), typeof(UnitMover))]
public class UnitController : NetworkBehaviour, IDamagable
{
    #region Fields
    [SerializeField] private int health;
    [SerializeField] private int arcaneMultiplier =0;
    [SerializeField] private int fireDamageMultiplier=0;
    [SerializeField] private int frostDamageMultiplier=0;
    [SerializeField, Tooltip("The AI brain that will control the units behaviour")]
    private Brain brain;
    [SerializeField]
    public UnitClass unitClass;

    [SerializeField]
    public Inventory inventory;

    [HideInInspector]
    public UnitCaster unitCaster;

    [HideInInspector]
    public Animator animator;

    [HideInInspector]
    public UnitMover unitMover;

    public static KillEvent OnUnitDeath;
    public bool isDead { get; private set; }

    public int GetHealth()
    {
        return health;
    }
    public int GetArcaneDamageMultiplier()
    {
        return arcaneMultiplier;
    }
    #endregion


    #region Awake, Start and Update
    void Awake()
    {
        unitCaster = GetComponent<UnitCaster>();
        unitMover = GetComponent<UnitMover>();
        animator = GetComponentInChildren<Animator>();
        health = unitClass.maxHealth;
        inventory = new Inventory();
    }

    void Start()
    {
        
        if (IsLocalPlayer)
        {
            SpellShop.instance.ConnectPlayer();
        }
    }


    void Update()
    {
        if (!IsServer) return;
        
            
        
        brain?.HandleActions(this);
    }
    #endregion
    
    

    #region Movement Inputs
    /// <summary>
    /// What happens when right clicking. Sets units target position. - Silas Thule
    /// </summary>
    void OnRightClick()
    {
        if (!IsLocalPlayer || brain) return;
        bool validClickPosition;
        Vector3 target = HelperClass.GetMousePosInWorld(out validClickPosition); //gets mouse pos
        if (validClickPosition)
        {
            target = new Vector3(target.x, 0, target.z);
            //Debug.Log(target);
            unitMover.SetTargetPositionServerRPC(target); //sets target pos to mouse pos
        }
    }

    void OnLeftClick()
    {
        if (!IsLocalPlayer || brain) return;
    }
    void OnStop()
    {
        if (!IsLocalPlayer || brain) return;
    }
    #endregion


    #region Spell Inputs
    void OnSpell1()
    {
        CastSpell(0);
    }
    void OnSpell2()
    {
        CastSpell(1);
    }
    void OnSpell3()
    {
        CastSpell(2);
    }
    void OnSpell4()
    {
        CastSpell(3);
    }
    void OnSpell5()
    {
        CastSpell(4);
    }
    void OnSpell6()
    {
        CastSpell(5);
    }
    #endregion

    
    #region Other Inputs

    void OnPause()
    {
        PauseMenu.instance.OpenPauseMenu();
    }

    #endregion

    /// <summary>
    /// Handles input. Tries to cast a spell with given index at the mousePosition in a server authoritative way. - Silas Thule
    /// </summary>
    /// <param name="index"></param>
    void CastSpell(int index)
    {
        if (!IsLocalPlayer || brain) return;
        bool validTarget;
        Vector3 pos = HelperClass.GetMousePosInWorld(out validTarget);
        if (validTarget)
        {
            unitCaster.TryCastSpell(index, pos);
        }
        
    }

    public void TryPlaceBuyable(int itemId,int index){
        if (IsServer)
        {
            PlaceBuyable(itemId,index);
        }else
        {
            PlaceBuyableServerRpc(itemId,index);
        }
    }
    [ServerRpc]

    void PlaceBuyableServerRpc(int itemId, int index){
        
        PlaceBuyable(itemId, index);
    }

    void PlaceBuyable(int itemId,int index){
        Buyable buyable = SpellShop.instance.buyableIDs[itemId];

        if (buyable is Item)
        {
            Item item = buyable as Item;
            inventory.items[index]=item;
            inventory.gold = inventory.gold-item.price;
            health = health+item.health;
            if (item.itemElement is SpellElementType.Frost)
            {
                frostDamageMultiplier +=item.elementBoostPercent;      
            }else if(item.itemElement is SpellElementType.Arcane)
            {
                arcaneMultiplier +=item.elementBoostPercent;
            }else if(item.itemElement is SpellElementType.Fire){
                fireDamageMultiplier += item.elementBoostPercent;
            }
        }else
        {
            Spell spell = buyable as Spell;
            inventory.spells[index]=spell;
        }
    }

    

    public void TryGetItem(ulong clientID,int itemID){
        
        Buyable buyable = SpellShop.instance.buyableIDs[itemID];
        if (IsServer)
        {   
            
           if ((!inventory.items.Contains(buyable)||!inventory.spells.Contains(buyable))&&inventory.gold>buyable.price)
            {
                Debug.Log("in if check of item contains");
             SpellShop.instance.BuyBuyable(); 
            }
        }else
        {   
             GetItemServerRpc(clientID,itemID);
        }
    }

    [ServerRpc]
    void GetItemServerRpc(ulong clientID,int itemID){
        
        Buyable buyable = SpellShop.instance.buyableIDs[itemID];
         if (inventory.items.Contains(buyable)||inventory.spells.Contains(buyable)&&inventory.gold>buyable.price)
            {
               GetItemClientRpc(clientID);
            }
         
    }

    [ClientRpc]
    void GetItemClientRpc(ulong clientID){
        
        if (clientID==NetworkManager.Singleton.LocalClientId)
        {
               SpellShop.instance.BuyBuyable();  
        }
         
    }
    /// <summary>
    /// Changes the units health and clamps the value between 0 and maxHealth. Calls Death method if health reaches 0. Server Only. - Silas Thule
    /// </summary>
    /// <param name="dealer"></param>
    /// <param name="amount"></param>
    public void ModifyHealth(UnitController dealer,int amount)
    {
        
        if (RoundManager.instance && !RoundManager.instance.roundIsOngoing) return;
        health = Mathf.Clamp(health+amount,0,unitClass.maxHealth);
        if (health == 0)
        {
            Death(dealer);
        }
    }
    /// <summary>
    /// Is called on Death and sends KillData to RoundManager. Server Only. - Silas Thule
    /// </summary>
    /// <param name="killer"></param>
    public void Death(UnitController killer)
    {
        if (isDead) return;
        SetDead(true);
        KillData kill = new KillData(this, killer);
        OnUnitDeath.Invoke(kill);
    }
    /// <summary>
    /// Resets health to full and Sets dead to false. Server Only. - Silas Thule
    /// </summary>
    public void ResetHealth()
    {
        health = unitClass.maxHealth;
        SetDead(false);
    }
    /// <summary>
    /// Disables Colliders on clients and server.
    /// </summary>
    /// <param name="isDead"></param>
    private void SetDead(bool isDead)
    {
        this.isDead = isDead;
        GetComponent<Collider>().enabled = !isDead; //Disables Collider on server
        SetDeadClientRPC(isDead); //Disables Collider on clients
    }
    /// <summary>
    /// Disables Colliders on clients
    /// </summary>
    /// <param name="isDead"></param>
    [ClientRpc]
    private void SetDeadClientRPC(bool isDead)
    {
        GetComponent<Collider>().enabled = !isDead;
    }
    public void InitializeBot(Brain brain)
    {
        this.brain = brain;
    }
}


[Serializable]
public class KillEvent : UnityEvent<KillData> { }
