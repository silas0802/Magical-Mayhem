using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New AI Brain", menuName = "Game/AI/Brain")]
public class Brain : ScriptableObject
{
    public BuyingLogic buyingLogic;
    public FightingLogic fightingLogic;
    public float threatLevel = 0f;
    public bool isNearUnit = false;

    /// <summary>
    /// Handles the decisions that the AI or player makes
    /// </summary>
    /// <param name="controller"></param>
    public void HandleActions(UnitController controller){
        UnitController nearestUnit = RoundManager.instance.FindNearestUnit(controller.transform.position, controller);
        float distanceToNearestUnit = (controller.transform.position - nearestUnit.transform.position).magnitude;
        
        if (distanceToNearestUnit <= 3f) {
            isNearUnit = true;
        } else 
        {
            isNearUnit = false;
        }
    }

    private List<ProjectileInstance> CheckForNearbyProjectiles(UnitController controller)
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
