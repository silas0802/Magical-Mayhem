using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class BarrierInstance : MonoBehaviour
{
    float timeLeft;
    public void InitializeSpell(BarrierSpell spell, UnitController owner)
    {
        timeLeft = spell.duration;
        NetworkObject ob = GetComponent<NetworkObject>();
        ob.Spawn();
        ob.TrySetParent(owner.transform);
    }
    private void Update()
    {
        timeLeft -= Time.deltaTime;
        if (timeLeft < 0)
        {
            GetComponent<NetworkObject>().Despawn();
            Destroy(gameObject);
        }
    }
}
