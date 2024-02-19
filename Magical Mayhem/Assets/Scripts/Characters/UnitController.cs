using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
/// <summary>
/// Controls all the behaviour of a unit
/// </summary>
[RequireComponent(typeof(UnitCaster), typeof(UnitMover))]
public class UnitController : NetworkBehaviour, IDamagable
{
    #region Fields

    [SerializeField] private int health;
    [SerializeField, Tooltip("The AI brain that will control the units behaviour")]
    private Brain brain;
    [SerializeField]
    public UnitClass unitClass;

    [HideInInspector]
    public UnitCaster unitCaster;

    [HideInInspector]
    public Animator animator;

    [HideInInspector]
    public UnitMover unitMover;

    public static KillEvent OnUnitDeath;
    private bool isDead;
    #endregion


    #region Awake, Start and Update
    void Awake()
    {
        unitCaster = GetComponent<UnitCaster>();
        unitMover = GetComponent<UnitMover>();
        animator = GetComponentInChildren<Animator>();
        health = unitClass.maxHealth;
    }
    

    void Update()
    {
        brain?.HandleActions(this);
    }
    #endregion


    #region Movement Inputs
    /// <summary>
    /// What happens when right clicking. Sets units target position. - Silas Thule
    /// </summary>
    void OnRightClick()
    {
        if (!IsLocalPlayer) return;
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

    }
    void OnStop()
    {

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

    /// <summary>
    /// Tries to cast a spell with given index at the mousePosition in a server authoritative way. - Silas Thule
    /// </summary>
    /// <param name="index"></param>
    void CastSpell(int index)
    {   
        bool validTarget;
        Vector3 pos = HelperClass.GetMousePosInWorld(out validTarget);
        if (validTarget)
        {
            unitCaster.TryCastSpell(index, pos);
        }
        
    }
    

    public void ModifyHealth(UnitController dealer,int amount)
    {
        if (RoundManager.instance && !RoundManager.instance.roundIsOngoing) return;
        health = Mathf.Clamp(health+amount,0,unitClass.maxHealth);
        if (health == 0)
        {
            Death(dealer);
        }
    }

    public void Death(UnitController killer)
    {
        if (isDead) return;
        isDead = true;
        KillData kill = new KillData(this, killer);
        OnUnitDeath.Invoke(kill);
    }

    public void ResetHealth()
    {
        health = unitClass.maxHealth;
        isDead= false;
    }
}
[Serializable]
public class KillEvent : UnityEvent<KillData> { }
