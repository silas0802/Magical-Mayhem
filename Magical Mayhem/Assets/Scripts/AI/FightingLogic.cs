using UnityEngine;

public abstract class FightingLogic : ScriptableObject
{
    public abstract void HandleFightingLogic(UnitController controller);
}
