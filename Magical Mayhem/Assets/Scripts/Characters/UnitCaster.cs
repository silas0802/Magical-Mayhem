using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

[RequireComponent(typeof(UnitController))]
public class UnitCaster : NetworkBehaviour
{
    [SerializeField] private Spell[] spells = new Spell[6];
    [SerializeField] private float[] cooldowns = new float[6];
    private UnitController controller;

    private void Awake()
    {
        controller = GetComponent<UnitController>();
    }

    void Update()
    {
        HandleCooldowns();
    }

    public void TryCastSpell(int index, Vector3 target)
    {
        if (IsServer && IsLocalPlayer)
        {
            CastSpell(index, target);
        }
        else if (IsClient && IsLocalPlayer)
        {
            if (cooldowns[index] <= 0)
            {
                SetCooldown(index, spells[index].cooldown);
                CastSpellServerRPC(index, target);

            }
        }
    }
    [ServerRpc]
    private void CastSpellServerRPC(int index, Vector3 target)
    {
        CastSpell(index,target);
    }

    /// <summary>
    /// Casts the given spell based on the index. Can only be called from server.
    /// </summary>
    /// <param name="index"></param>
    /// <param name="target"></param>
    private void CastSpell(int index, Vector3 target)
    {
        
        Spell spell = spells[index];
        if (!spell || cooldowns[index]>0) return;

        SetCooldown(index, spells[index].cooldown);
        StartCoroutine(CastingSpell(spell, target));
        

        
    }
    IEnumerator CastingSpell(Spell spell, Vector3 target)
    {
        controller.unitMover.canMove = false;
        yield return new WaitForSeconds(spell.castTime);
        spell.Activate(controller, target);
        controller.unitMover.canMove = true;
    }
    /// <summary>
    /// Checks if the equipped spells match with the units current class's allowed spells
    /// </summary>
    /// <returns></returns>
    public bool ValidateSpells(){
        return true;
    }
    private void HandleCooldowns()
    {
        for (int i = 0; i < cooldowns.Length; i++)
        {
            cooldowns[i] -= Time.deltaTime;
        }
    }
    private void SetCooldown(int index, float value)
    {
        cooldowns[index] = value;
    }
    
}
