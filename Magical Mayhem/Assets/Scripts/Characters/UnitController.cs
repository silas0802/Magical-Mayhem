using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(UnitCaster), typeof(UnitMover))]
public class UnitController : NetworkBehaviour
{
    [SerializeField, Tooltip("The AI brain that will control the units behaviour")]
    private Brain brain;
    [SerializeField]
    private UnitClass unitClass;

    [HideInInspector]
    public UnitCaster unitCaster;

    [HideInInspector]
    public UnitMover unitMover;

    private UnitState state;

    void Awake() {
        unitCaster = GetComponent<UnitCaster>();
        unitMover = GetComponent<UnitMover>();
    }
    void Start() {
        ChangeState(new UnitMoveState());
    }

    void Update() {
        brain?.HandleActions(this);
        state.StateUpdate(this);
    }

    void OnRightClick() {
        if (!IsLocalPlayer) return;
        bool validClickPosition;
        Vector3 target = HelperClass.GetMousePosInWorld(out validClickPosition); //gets mouse pos
        if (validClickPosition) {
            target = new Vector3(target.x, 0, target.z);
            //Debug.Log(target);
            unitMover.SetTargetPositionServerRPC(target); //sets target pos to mouse pos
        }
    }
    
    void OnLeftClick() {

    }
    void OnStop()
    {

    }

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

    void CastSpell(int index)
    {
        bool validTarget;
        Vector3 pos = HelperClass.GetMousePosInWorld(out validTarget);
        if (validTarget)
        {
            if (IsServer && IsLocalPlayer)
            {
                unitCaster.CastSpell(index, pos);
            }
            else if (IsClient && IsLocalPlayer)
            {
                unitCaster.CastSpellServerRPC(index, pos);
            }
        }
        
    }


    /// <summary>
    /// Changes the State of the UnitController and calls the EnterState function of state
    /// </summary>
    /// <param name="state"></param>
    public void ChangeState(UnitState state)
    {
        this.state = state;
        state.EnterState(this);
    }


}
