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
    public void HandleActions(UnitController controller){

        //Prøver hver frame at kaste en fireball 5m foran sig selv
        Debug.Log("Bot cast");
        controller.unitCaster.TryCastSpell(0, controller.transform.forward * 5 + controller.transform.position);
    }
}
