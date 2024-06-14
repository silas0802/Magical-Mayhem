using UnityEngine;

[CreateAssetMenu(fileName = "Medium Fighting Logic", menuName = "Game/AI/Fighting Logic/Medium Bot")]
public class MediumFightingLogic : FightingLogic
{
    public override void HandleFightingLogic()
    {
        Debug.Log("Medium Fighting");
    }
}
