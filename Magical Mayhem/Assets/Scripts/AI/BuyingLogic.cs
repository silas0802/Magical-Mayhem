using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[CreateAssetMenu(fileName ="New Buying Logic", menuName = "Game/AI/Buying Logic")]
public class BuyingLogic : ScriptableObject
{
    [SerializeField] private Spell[] spells;
    private bool hasSetupInventory = false;

    public void HandleShoppingLogic(UnitController controller)
    {
        controller.inventory.spells[0] = spells[0];
        controller.inventory.spells[1] = spells[1];
        controller.inventory.spells[2] = spells[2];
        controller.inventory.spells[3] = spells[3];
        controller.inventory.spells[4] = spells[4];
        controller.inventory.spells[5] = spells[5];
    }
}
