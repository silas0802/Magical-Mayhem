using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName ="New Fighting Logic", menuName = "Game/AI/Fighting Logic")]
public class FightingLogic : ScriptableObject
{
    [SerializeField] private float Aggresiveness;
    [SerializeField] private float Safety;
    public void HandleFightingLogic(UnitController controller)
    {
        
        Vector3 targetPosition = controller.transform.position + Vector3.forward;


        controller.unitMover.SetTargetPosition(targetPosition);
        try
        {
            UnitController nearestUnit = RoundManager.instance.FindNearestUnit(controller.transform.position, controller);
            float distanceToNearestUnit = (controller.transform.position - nearestUnit.transform.position).magnitude;

            if (distanceToNearestUnit <= 3f)
            {
                controller.isNearUnit = true;
            }
            else
            {
                controller.isNearUnit = false;
            }
        }
        catch (System.Exception)
        {
            controller.isNearUnit = false;
        }
        

    }
    private List<ProjectileInstance> CheckForNearbyProjectiles(UnitController controller)
    {
        float size = 10;
        Collider[] detected = new Collider[20];
        List<ProjectileInstance> projectiles = new List<ProjectileInstance>();
        Physics.OverlapBoxNonAlloc(controller.transform.position + Vector3.up, new Vector3(size, 0.5f, size), detected);
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
