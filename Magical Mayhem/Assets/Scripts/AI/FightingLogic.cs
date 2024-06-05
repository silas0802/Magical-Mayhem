using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Fighting Logic", menuName = "Game/AI/Fighting Logic")]
public class FightingLogic : ScriptableObject
{
    [SerializeField] private bool Aggresiveness;
    [SerializeField] private bool Safety;

    public void HandleFightingLogic(UnitController controller)
    {
        if (controller == null)
        {
            Debug.LogError("Controller is null in HandleFightingLogic");
            return;
        }

        controller.frameCounter++;
        // Every 10th frame we check for nearby projectiles and try to dodge them. This only affects movement.

        if (controller.frameCounter % 10 == 0)
        {
            // Get list of nearby projectiles. If this list is not empty, check danger from each projectile. We also need some way to combine the dangers into one unified decision of what to do.
            // This problem will be solved later.
            List<ProjectileInstance> nearbyProjectiles = DetectNearbyProjectiles(controller);
            if (nearbyProjectiles != null && nearbyProjectiles.Count > 0)
            {
                Vector3 dodgeDirection = EvaluateNearbyProjectiles(controller, nearbyProjectiles);
                // ^ Dodgedirection is zeroVector if we don't want the bot to dodge anything.

                if (dodgeDirection != Vector3.zero) 
                {
                    Safety = false;
                    Vector3 targetPosition = controller.transform.position + dodgeDirection;
                    controller.unitMover.SetTargetPosition(targetPosition);
                } else {
                    Safety = true;
                }
            }
            
            // If there's no danger we roam randomly. Improve later if good idea
            if (Safety) {
                float randomAngle = Random.Range(0, 360);
                Vector3 randomRoamingDirection = new Vector3(Mathf.Cos(randomAngle), 0, Mathf.Sin(randomAngle)).normalized;
                Vector3 targetPosition = controller.transform.position + randomRoamingDirection * 3f;
                controller.unitMover.SetTargetPosition(targetPosition);
            }

            controller.frameCounter = 0; // Just so that numbers don't get too large.
        }

        

        try
        {
            UnitController nearestUnit = RoundManager.instance.FindNearestUnit(controller.transform.position, controller);
            if (nearestUnit != null)
            {
                float distanceToNearestUnit = (controller.transform.position - nearestUnit.transform.position).magnitude;

                controller.isNearUnit = distanceToNearestUnit <= 3f;
            }
            else
            {
                controller.isNearUnit = false;
            }
        }
        catch (System.Exception ex)
        {
            Debug.LogError("Exception in finding nearest unit: " + ex.Message);
            controller.isNearUnit = false;
        }
    }

    private List<ProjectileInstance> DetectNearbyProjectiles(UnitController controller)
    {
        if (controller == null || controller.transform == null)
        {
            Debug.LogError("Controller or its transform is null in DetectNearbyProjectiles");
            return null;
        }

        float size = 10f;
        Collider[] detected = new Collider[20];
        List<ProjectileInstance> projectiles = new List<ProjectileInstance>();
        Physics.OverlapBoxNonAlloc(controller.transform.position + Vector3.up, new Vector3(size, 0.5f, size), detected);

        for (int i = 0; i < detected.Length; i++)
        {
            if (detected[i] == null)
            {
                continue; // Skip null detected objects
            }

            ProjectileInstance projectile = detected[i].GetComponent<ProjectileInstance>();
            if (projectile != null && projectile.owner != null)
            {
                projectiles.Add(projectile);
            }
        }
        return projectiles;
    }

    private Vector3 EvaluateNearbyProjectiles(UnitController controller, List<ProjectileInstance> nearbyProjectiles)
    {
        Vector3 dodgeDirection = Vector3.zero;

        foreach (ProjectileInstance projectile in nearbyProjectiles)
        {
            if (projectile == null) //Not sure if projectile could ever be null??
            {
                continue;
            }

            // Get the projectile's velocity
            Vector3 projVelocity = projectile.GetComponent<Rigidbody>().velocity;

            // https://www.reddit.com/r/summonerschool/comments/y751c3/how_to_get_good_at_dodging_spells/
            // Says "Dodge at 90 degrees to the angle the spell is coming from"

            // Let's first find a way to decide if the spell is actually approaching us. Else we should look to dodge.
            // Calculate the angle between the projectile's velocity and the direction to the controller
             Vector3 directionToController = controller.transform.position - projectile.transform.position;
            float angle = Vector3.Angle(projVelocity, directionToController);

            // Check if the projectile is headed towards the controller within the angle threshold
            if (angle > 30)
            {
                // If the angle is greater than the threshold, continue
                Debug.Log("Projectile angle " + angle + " is too wide. Not dodging...");
                continue;
            } else {
                Debug.Log("Nearing me");
            }
            
            // So the idea we will use is to take the BEST perpendicular direction. This should make the bot not run into the projectile..
            Vector3 perpendicularDirection1 = Vector3.Cross(projVelocity.normalized, Vector3.up).normalized;
            Vector3 perpendicularDirection2 = -perpendicularDirection1;
            Vector3 futurePosition1 = controller.transform.position + perpendicularDirection1;
            Vector3 futurePosition2 = controller.transform.position + perpendicularDirection2;

            float distanceToProjectile1 = (futurePosition1 - projectile.transform.position).magnitude;
            float distanceToProjectile2 = (futurePosition2 - projectile.transform.position).magnitude;

            // We choose the direction that maximizes distance from the projectile
            Vector3 bestDodgeDirection = distanceToProjectile1 > distanceToProjectile2 ? perpendicularDirection1 : perpendicularDirection2;

            // Accumulate the direction to move away from each projectile - In my mind this should allow us one direction that helps dodging ALL nearby spells?
            dodgeDirection += bestDodgeDirection / (controller.transform.position - projectile.transform.position).magnitude;
        }

        return dodgeDirection.normalized;
    }
}