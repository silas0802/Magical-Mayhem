using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IDamagable 
{
    public abstract void ModifyHealth(UnitController dealer,int health);
    public abstract void ResetHealth();
    public abstract void Death(UnitController killer);
}
