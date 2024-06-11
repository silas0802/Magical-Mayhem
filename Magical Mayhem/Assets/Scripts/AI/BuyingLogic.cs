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
    }
}
