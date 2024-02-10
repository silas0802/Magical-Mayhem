using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(UnitCaster),typeof(UnitMover))]
public class UnitController : NetworkBehaviour
{
    [SerializeField] Brain brain;
    [HideInInspector] public UnitCaster unitCaster;
    [HideInInspector] public UnitMover unitMover;
    private UnitState state;

    void Awake(){
        unitCaster = GetComponent<UnitCaster>();
        unitMover = GetComponent<UnitMover>();
    }
    void Start(){
        ChangeState(new UnitMoveState());
    }
    
    void Update(){
        brain?.HandleActions(this);
        state.StateUpdate(this);
    }

    void OnRightClick(){
        if (!IsLocalPlayer) return;
        bool validClickPosition;
        Vector3 target = HelperClass.GetMousePosInWorld(out validClickPosition); //gets mouse pos
        if (validClickPosition){
            target = new Vector3(target.x,0,target.z);
            unitMover.SetTargetPosition(target); //sets target pos to mouse pos
        }
    }

    void OnLeftClick(){

    }

    /// <summary>
    /// Changes the State of the UnitController and calls the EnterState function of state
    /// </summary>
    /// <param name="state"></param>
    public void ChangeState(UnitState state){
        this.state = state;
        state.EnterState(this);
    }

    
}
