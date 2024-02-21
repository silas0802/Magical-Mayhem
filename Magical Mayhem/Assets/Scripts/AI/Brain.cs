using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New AI Brain", menuName = "Game/AI/Brain")]
public class Brain : ScriptableObject
{
    float timer = 5;
    public BuyingLogic buyingLogic;
    public FightingLogic fightingLogic;

    /// <summary>
    /// Handles the decisions that the AI or player makes
    /// </summary>
    /// <param name="controller"></param>
    public void HandleActions(UnitController controller){

    }
}
