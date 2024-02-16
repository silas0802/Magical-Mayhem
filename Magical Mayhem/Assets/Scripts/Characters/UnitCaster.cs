using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
[RequireComponent(typeof(UnitController))]
public class UnitCaster : NetworkBehaviour
{
    [SerializeField] private Spell[] spells = new Spell[6];
    private UnitController controller;

    private void Awake()
    {
        controller = GetComponent<UnitController>();
    }

    void Update()
    {
        
    }

    [ServerRpc]
    public void CastSpellServerRPC(int index, Vector3 target)
    {
        CastSpell(index,target);
    }

    /// <summary>
    /// Casts the given spell based on the index. Can only be called from server.
    /// </summary>
    /// <param name="index"></param>
    /// <param name="target"></param>
    public void CastSpell(int index, Vector3 target)
    {
        
        Spell spell = spells[index];
        if (!spell) return;
        spell.Activate(controller, target);
        

        
    }
    /// <summary>
    /// Checks if the equipped spells match with the units current class's allowed spells
    /// </summary>
    /// <returns></returns>
    public bool ValidateSpells(){
        return true;
    }

    
}
