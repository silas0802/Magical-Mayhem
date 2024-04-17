using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class MapWallScript : NetworkBehaviour, IDamagable
{
    [SerializeField]private int health = 20;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void TakeDmg(int dmg){
        health -= dmg;
        if(health <= 0){
            
        }
    }

    public void ModifyHealth(UnitController dealer, int health)
    {
        if (health <= 0)
        {
            return;   
        }
        this.health += health;
        if(health <= 0){
            Death(dealer);
        }
    }

    public void ResetHealth()
    {
        throw new System.NotImplementedException();
    }

    public void Death(UnitController killer)
    {
        GetComponent<NetworkObject>().Despawn();
        Destroy(this);
    }
    
}
