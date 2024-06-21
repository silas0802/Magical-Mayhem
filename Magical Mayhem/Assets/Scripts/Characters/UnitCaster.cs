using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

[RequireComponent(typeof(UnitController))]
public class UnitCaster : NetworkBehaviour
{
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

    public float[] getCooldowns(){
        return cooldowns;
    }

    /// <summary>
    /// Is called locally and determines how to cast depending on user being server or client.
    /// </summary>
    /// <param name="index"></param>
    /// <param name="target"></param>
    public void TryCastSpell(int index, Vector3 target)
    {
        if (IsServer)
        {
            CastSpell(index, target);
        }
        else if (IsClient)
        {
            if (cooldowns[index] <= 0)
            {
                SetCooldown(index, controller.inventory.spells[index].cooldown);
                CastSpellServerRPC(index, target);
            }
        }
    }
    /// <summary>
    /// Requests the server to cast spell for client. Called from client (including host).
    /// </summary>
    /// <param name="index"></param>
    /// <param name="target"></param>
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
        if (controller.isDead) {
            return;
        }
        
        Spell spell = controller.inventory.spells[index];
        if (!spell || cooldowns[index]>0) return;

        SetCooldown(index, controller.inventory.spells[index].cooldown);
        controller.unitMover.ReachTarget();
        transform.LookAt(target);
        if (spell.castTime <= 1f)
        {
            controller.animator.SetTrigger("Cast");
            
        }
        StartCoroutine(CastingSpell(spell, target));
        

        
    }
    /// <summary>
    /// Casts spell and makes sure that the unit cant move while casting.
    /// </summary>
    /// <param name="spell"></param>
    /// <param name="target"></param>
    /// <returns></returns>
    IEnumerator CastingSpell(Spell spell, Vector3 target)
    {
        
        if (!spell.canMoveWhileCasting) controller.unitMover.canMove = false;

        yield return new WaitForSeconds(spell.castTime);
        spell.Activate(controller, target);
        if (spell is MultiCastSpell)
        {
            MultiCastSpell m = spell as MultiCastSpell;
            float extraDelay = (m.amount - 1) * m.timeInterval;
            yield return new WaitForSeconds(extraDelay);
        }
        if (!spell.canMoveWhileCasting) controller.unitMover.canMove = true;
    }
    /// <summary>
    /// Checks if the equipped spells match with the units current class's allowed spells
    /// </summary>
    /// <returns></returns>
    public bool ValidateSpells(){
        return true;
    }
    /// <summary>
    /// Decreases all the cooldowns each update
    /// </summary>
    private void HandleCooldowns()
    {
        for (int i = 0; i < cooldowns.Length; i++)
        {
            cooldowns[i] -= Time.deltaTime;
        }
    }
    /// <summary>
    /// Set Cooldown of a given spell.
    /// </summary>
    /// <param name="index"></param>
    /// <param name="value"></param>
    private void SetCooldown(int index, float value)
    {
        cooldowns[index] = value;
    }

    public float[] GetCooldowns()
    {
        return cooldowns;
    }
    
}
