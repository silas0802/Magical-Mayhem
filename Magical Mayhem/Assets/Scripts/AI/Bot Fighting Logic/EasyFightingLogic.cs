using UnityEngine;

[CreateAssetMenu(fileName = "Easy Fighting Logic", menuName = "Game/AI/Fighting Logic/Easy Bot")]
public class EasyFightingLogic : FightingLogic
{
    public override void HandleFightingLogic()
    {
        Debug.Log("Easy Fighting");
    }
}
