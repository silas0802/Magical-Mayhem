using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class KillData 
{
    public UnitController deadUnit;
    public UnitController killer;

    public KillData(UnitController deadUnit, UnitController killer)
    {
        this.deadUnit = deadUnit;
        this.killer = killer;
    }
}
