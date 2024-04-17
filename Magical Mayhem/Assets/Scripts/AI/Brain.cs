using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New AI Brain", menuName = "Game/AI/Brain")]
public class Brain : ScriptableObject
{
    public BuyingLogic buyingLogic;
    public FightingLogic fightingLogic;
    

    /// <summary>
    /// Handles the decisions that the AI or player makes
    /// </summary>
    /// <param name="controller"></param>
    public void HandleFightingLogic(UnitController controller){
        fightingLogic.HandleFightingLogic(controller);
    }
    public void HandleShoppingLogic()
    {
        buyingLogic.HandleShoppingLogic();
    }

    
}
