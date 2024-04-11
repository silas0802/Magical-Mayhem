using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class TeleportInstance : NetworkBehaviour
{
    float timeLeft;
    TeleportSpell spell;
    public void InitializeSpell(TeleportSpell spell){
        this.spell = spell;
        timeLeft = spell.vfxLifetime;
        GetComponent<NetworkObject>().Spawn();
    }

    // Update is called once per frame
    void Update()
    {
        timeLeft -= Time.deltaTime;
        if (timeLeft < 0){
            GetComponent<NetworkObject>().Despawn();
            Destroy(gameObject);
        }
    }
}
