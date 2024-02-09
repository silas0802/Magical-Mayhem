using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class UnitState 
{
    public abstract void EnterState(UnitController controller);
    public abstract void StateUpdate(UnitController controller);

}
