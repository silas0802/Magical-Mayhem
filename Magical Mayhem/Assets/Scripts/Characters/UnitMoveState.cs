using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitMoveState : UnitState
{
    public override void EnterState(UnitController controller)
    {

    }

    public override void StateUpdate(UnitController controller)
    {
        if (controller.IsServer && controller.IsLocalPlayer)
        {
            controller.unitMover.Move();
        }
        else if (controller.IsClient && controller.IsLocalPlayer)
        {
            controller.unitMover.MoveServerRPC();
        }
    }
}
