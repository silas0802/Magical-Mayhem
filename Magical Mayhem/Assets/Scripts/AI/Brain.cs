using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New AI Brain", menuName = "Game/AI/Brain")]
public class Brain : ScriptableObject
{
    public BuyingLogic buyingLogic;
    public FightingLogic fightingLogic;

    /// <summary>
    /// Handles the decisions that the AI or player makes
    /// </summary>
    /// <param name="controller"></param>
    public void HandleActions(UnitController controller){

        //Prøver hver frame at kaste en fireball 5m foran sig selv
        Debug.Log("Bot cast");
        controller.unitCaster.TryCastSpell(0, controller.transform.forward * 5 + controller.transform.position);
    }

    //RoundManager.instance.FindNearestUnit() 
    
    private List<ProjectileInstance> CheckForNearbyProjectiles(UnitController controller)//Dette er ikke testet så kan ikke garantere at det virker
    {
        float size = 10;
        Collider[] detected = new Collider[20];
        List<ProjectileInstance> projectiles = new List<ProjectileInstance>();
        Physics.OverlapBoxNonAlloc(controller.transform.position+ Vector3.up,new Vector3(size,0.5f,size), detected);
        for (int i = 0; i < detected.Length; i++)
        {
            ProjectileInstance projectile = detected[i].GetComponent<ProjectileInstance>();
            if (projectile != null && projectile.owner)
            {
                projectiles.Add(projectile);
            }
        }
        return projectiles;
    } 
}
