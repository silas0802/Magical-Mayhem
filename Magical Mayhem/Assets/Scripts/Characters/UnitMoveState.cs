using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitMoveState : UnitState
{
    public override void EnterState(UnitController controller)
    {
        throw new System.NotImplementedException();
    }

    public override void StateUpdate(UnitController controller)
    {
        controller.unitMover.Move();
    }
}
