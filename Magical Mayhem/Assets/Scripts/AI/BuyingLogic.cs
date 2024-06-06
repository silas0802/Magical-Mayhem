using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName ="New Buying Logic", menuName = "Game/AI/Buying Logic")]
public class BuyingLogic : ScriptableObject
{
    [SerializeField] private Spell[] spells;

    public void HandleShoppingLogic(UnitController controller)
    {
        switch (RoundManager.instance.roundNumber)
        {
            case 1:
                controller.inventory.spells[0] = spells[0];
                break;
            case 2:
                controller.inventory.spells[1] = spells[1];
                break;
            default:
                controller.inventory.spells = spells;
                break;
        }
        throw new System.NotImplementedException();
    }
}
