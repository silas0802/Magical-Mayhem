using UnityEngine;

[CreateAssetMenu(fileName = "Hard Fighting Logic", menuName = "Game/AI/Fighting Logic/Hard Bot")]
public class HardFightingLogic : FightingLogic
{
    public override void HandleFightingLogic()
    {
        Debug.Log("Hard Fighting");
    }
}
