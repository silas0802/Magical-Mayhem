using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New AI Brain", menuName = "Game/AI/Brain")]
public class Brain : ScriptableObject
{

    [SerializeField] private bool isOn = false;

    public FightingLogic botDifficulty;

    public FightingLogic easyFightingLogic;
    public FightingLogic mediumFightingLogic;
    public FightingLogic hardFightingLogic;
    [SerializeField] private Spell[] spells;

    /// <summary>
    /// Handles the decisions that the AI or player makes
    /// </summary>
    /// <param name="controller"></param>
    public void HandleFightingLogic(UnitController controller) {
        if (isOn && RoundManager.instance.roundIsOngoing.Value) {
            botDifficulty.HandleFightingLogic(controller);
        }

        controller.inventory.spells[0] = spells[0];
        controller.inventory.spells[1] = spells[1];
        controller.inventory.spells[2] = spells[2];
        controller.inventory.spells[3] = spells[3];
        controller.inventory.spells[4] = spells[4];
        controller.inventory.spells[5] = spells[5];
    }

    public void SetBotDifficulty(string mode)
    {
        switch (mode)
        {
            case "Easy":
                botDifficulty = easyFightingLogic;
                break;
            case "Medium":
                botDifficulty = mediumFightingLogic;
                break;
            case "Hard":
                botDifficulty = hardFightingLogic;
                break;
            default:
                Debug.Log("No difficulty for the bot: " + mode);
                botDifficulty = null;
                break;
        }
    }
}
