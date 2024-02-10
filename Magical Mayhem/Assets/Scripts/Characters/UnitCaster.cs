using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

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
    public void CastSpell(int index, Vector3 target)
    {
        
        Spell spell = spells[index];
        if (!spell) return;
        spell.Activate(controller, target);
        

        
    }

    
}
