using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class MapWallScript : NetworkBehaviour, IDamagable
{
    private int health = 100;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void takeDmg(int dmg){
        health -= dmg;
        if(health <= 0){
            Destroy(this.gameObject);
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
        Destroy(this.gameObject);
    }
    
}
