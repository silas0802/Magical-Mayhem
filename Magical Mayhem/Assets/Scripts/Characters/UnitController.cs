using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Netcode;
using UnityEditor;
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
    [SerializeField] NetworkVariable<int> health = new NetworkVariable<int>();
    [SerializeField] private int arcaneMultiplier =0;
    [SerializeField] private int fireDamageMultiplier=0;
    [SerializeField] private int frostDamageMultiplier=0;
    [SerializeField] private bool inLava = false;

    private int lavaDMG = 5;
    private float lavaTick = 1f;
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

    public int frameCounter;

    public static KillEvent OnUnitDeath;
    public bool isDead { get; private set; }

    public int GetHealth()
    {
        return health.Value;
    }
    public int GetFrostMult(){
        return frostDamageMultiplier;
    }
    public int GetArcaneMult(){
        return arcaneMultiplier;
    }
    public int GetFireMult(){
        return fireDamageMultiplier;
    }
    public int GetArcaneDamageMultiplier()
    {
        return arcaneMultiplier;
    }
    public void SetInLavaBool(bool x){
        inLava = x;
    }

    public bool GetInLavaBool()
    {
        return inLava;
    }
    
    [Header("Debugging")]
    public int threatLevel = 0;
    public bool isNearUnit = false;
    #endregion


    #region Awake, Start and Update
    void Awake()
    {
        unitCaster = GetComponent<UnitCaster>();
        unitMover = GetComponent<UnitMover>();
        animator = GetComponentInChildren<Animator>();
        health.Value = unitClass.maxHealth;
        inventory = new Inventory();
    }

    

    void Update()
    {
        if (!IsServer) return;
        brain?.HandleFightingLogic(this);

        if(inLava){

            if (lavaTick > 0){
                lavaTick -= Time.deltaTime;
            }
            else{
                ModifyHealth(this, -lavaDMG);
                lavaTick = 1;
            }
        }
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

    #region Shop related
    [ClientRpc]
    public void ConnectUnitToShopClientRPC()
    {
        if (IsLocalPlayer)
        {
            SpellShop.instance.ConnectPlayer(this);
            SpellShop.instance.InitalizePlayerInformation();
        }
    }

    [ClientRpc]
    public void ConnectUnitToHUDClientRPC()
    {
        if (IsLocalPlayer)
        {
            HUDScript.instance.ConnectPlayer(this);
            
        }
    }
    public void TryPlaceBuyable(int itemId, int index)
    {
        if (IsServer)
        {
            PlaceBuyable(itemId, index);
        }
        else
        {
            PlaceBuyableServerRpc(itemId, index);
        }
    }
    [ServerRpc]

    void PlaceBuyableServerRpc(int itemId, int index)
    {

        PlaceBuyable(itemId, index);
    }

    void PlaceBuyable(int itemId, int index)
    {
        Buyable buyable = SpellShop.instance.buyableIDs[itemId];
        inventory.gold = inventory.gold - buyable.price;
        if (buyable is Item)
        {
            Item item = buyable as Item;
            inventory.items[index] = item;

            health.Value += item.health;
            if (item.itemElement is SpellElementType.Frost)
            {
                frostDamageMultiplier += item.elementBoostPercent;
            }
            else if (item.itemElement is SpellElementType.Arcane)
            {
                arcaneMultiplier += item.elementBoostPercent;
            }
            else if (item.itemElement is SpellElementType.Fire)
            {
                fireDamageMultiplier += item.elementBoostPercent;
            }
        }
        else
        {
            Spell spell = buyable as Spell;
            inventory.spells[index] = spell;
        }
    }




    public void TryGetItem(ulong clientID, int itemID)
    {

        Buyable buyable = SpellShop.instance.buyableIDs[itemID];
        if (IsServer)
        {

            if (inventory.gold >= buyable.price && !inventory.items.Contains(buyable) && !inventory.spells.Contains(buyable))
            {

                SpellShop.instance.BuyBuyable();
            }
        }
        else
        {
            GetItemServerRpc(clientID, itemID);
        }
    }

    [ServerRpc]
    void GetItemServerRpc(ulong clientID, int itemID)
    {

        Buyable buyable = SpellShop.instance.buyableIDs[itemID];
        if (inventory.gold > buyable.price && !inventory.items.Contains(buyable) && !inventory.spells.Contains(buyable))
        {
            GetItemClientRpc(clientID, itemID);
        }

    }

    [ClientRpc]
    void GetItemClientRpc(ulong clientID, int itemid)
    {

        if (clientID == NetworkManager.Singleton.LocalClientId)
        {
            Buyable buyable = SpellShop.instance.buyableIDs[itemid];
            inventory.gold -= buyable.price;
            SpellShop.instance.BuyBuyable();

        }

    }

    public void TrySellItem(ulong clientID, int itemID)
    {
        Buyable buyable = SpellShop.instance.buyableIDs[itemID];
        if (IsServer)
        {
            if (inventory.spells.Contains(buyable) || inventory.items.Contains(buyable))
            {
                inventory.gold += buyable.price / 2;
                if (buyable is Item)
                {
                    Item item = buyable as Item;
                    health.Value -= item.health;
                    if (item.itemElement is SpellElementType.Frost)
                    {
                        frostDamageMultiplier -= item.elementBoostPercent;
                    }
                    else if (item.itemElement is SpellElementType.Arcane)
                    {
                        arcaneMultiplier -= item.elementBoostPercent;
                    }
                    else if (item.itemElement is SpellElementType.Fire)
                    {
                        fireDamageMultiplier -= item.elementBoostPercent;
                    }

                }
                SpellShop.instance.SellOwnedBuyable();
            }
        }
        else
        {
            SellItemServerRPC(clientID, itemID);
        }
    }

    [ServerRpc]
    void SellItemServerRPC(ulong clientID, int itemID)
    {

        SellItemClientRPC(clientID, itemID);
    }

    [ClientRpc]
    void SellItemClientRPC(ulong clientID, int itemID)
    {
        Buyable buyable = SpellShop.instance.buyableIDs[itemID];
        if (clientID == NetworkManager.Singleton.LocalClientId)
        {
            if (inventory.spells.Contains(buyable) || inventory.items.Contains(buyable))
            {
                inventory.gold += buyable.price / 2;
                if (buyable is Item)
                {
                    Item item = buyable as Item;
                    health.Value -= item.health;
                    if (item.itemElement is SpellElementType.Frost)
                    {
                        frostDamageMultiplier -= item.elementBoostPercent;
                    }
                    else if (item.itemElement is SpellElementType.Arcane)
                    {
                        arcaneMultiplier -= item.elementBoostPercent;
                    }
                    else if (item.itemElement is SpellElementType.Fire)
                    {
                        fireDamageMultiplier -= item.elementBoostPercent;
                    }


                }
                SpellShop.instance.SellOwnedBuyable();
            }

        }
    }
    #endregion

    #region IDamagable
    /// <summary>
    /// Changes the units health and clamps the value between 0 and maxHealth. Calls Death method if health reaches 0. Server Only. - Silas Thule
    /// </summary>
    /// <param name="dealer"></param>
    /// <param name="amount"></param>
    public void ModifyHealth(UnitController dealer,int amount)
    {
        
        if (RoundManager.instance && !RoundManager.instance.roundIsOngoing) return;
        health.Value = Mathf.Clamp(health.Value+amount,0,unitClass.maxHealth);
        HUDScript.instance.ModyfyHealthbarClientRPC();
        if (health.Value == 0)
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
        health.Value = unitClass.maxHealth;
        SetDead(false);
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
    /// <summary>
    /// Disables Colliders on clients and server.
    /// </summary>
    /// <param name="isDead"></param>
    private void SetDead(bool isDead)
    {
        this.isDead = isDead;
        GetComponent<Collider>().enabled = !isDead; //Disables Collider on server
        SetDeadClientRPC(isDead); //Disables Collider on clients
        if (isDead)
        {
            animator.SetTrigger("Death");
        }
        else
        {
            animator.SetTrigger("Respawn");
        }
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


    

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        foreach (UnitController unit in RoundManager.instance.GetUnits)
        {
            Gizmos.DrawLine(transform.position, unit.transform.position);
        }


        if (brain && threatLevel >= 0f && threatLevel < 0.25f)
        {
            Handles.color = Color.green;
            Handles.DrawWireDisc(transform.position, Vector3.up, 0.5f);
        }
        else if (brain && threatLevel >= 0.25f && threatLevel < 0.5f)
        {
            Handles.color = Color.yellow;
            Handles.DrawWireDisc(transform.position, Vector3.up, 0.5f);
        }
        else
        {
            Handles.color = Color.red;
            Handles.DrawWireDisc(transform.position, Vector3.up, 0.5f);
        }

        if (brain && isNearUnit)
        {
            Handles.color = Color.red;
            Handles.DrawWireDisc(transform.position, Vector3.up, 3f);
        }
        else
        {
            Handles.color = Color.green;
            Handles.DrawWireDisc(transform.position, Vector3.up, 3f);
        }
    }
#endif
}


[Serializable]
public class KillEvent : UnityEvent<KillData> { }
