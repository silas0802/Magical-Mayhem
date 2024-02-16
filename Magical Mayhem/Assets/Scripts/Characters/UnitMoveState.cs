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
            // if ((controller.unitMover.targetPosition-controller.transform.position).magnitude<controller.unitClass.acceptingDistance){
            //     controller.ChangeState(new UnitIdleState());
            // }
        }
        else if (controller.IsClient && controller.IsLocalPlayer)
        {
            controller.unitMover.MoveServerRPC();
        }
    }
}
