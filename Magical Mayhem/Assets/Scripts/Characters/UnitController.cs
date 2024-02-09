using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(UnitCaster),typeof(UnitMover))]
public class UnitController : MonoBehaviour
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
        bool validClickPosition;
        Vector3 target = HelperClass.GetMousePosInWorld(out validClickPosition);
        if (validClickPosition){
            target = new Vector3(target.x,0,target.z);
            Debug.Log(target);
            unitMover.SetTargetPosition(target);
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
