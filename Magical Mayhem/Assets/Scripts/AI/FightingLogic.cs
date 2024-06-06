using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Fighting Logic", menuName = "Game/AI/Fighting Logic")]
public class FightingLogic : ScriptableObject
{
    [SerializeField] private float Aggressiveness;
    [SerializeField] private bool Safety;
    [SerializeField] private float roamingSpeed = 1.0f;
    [SerializeField] private float roamingInterval = 2.0f;

    private Vector3 roamingDirection;
    private float roamingTimer;

    public void HandleFightingLogic(UnitController controller)
    {
        controller.frameCounter++;
        // Every 10th frame we check for nearby projectiles and try to dodge them. This only affects movement.

        if (controller.frameCounter % 10 == 0)
        {
            List<ProjectileInstance> nearbyProjectiles = DetectNearbyProjectiles(controller);
            if (nearbyProjectiles != null && nearbyProjectiles.Count > 0)
            {
                Vector3 dodgeDirection = EvaluateNearbyProjectiles(controller, nearbyProjectiles);

                if (dodgeDirection != Vector3.zero) 
                {
                    Safety = false;
                    Vector3 targetPosition = controller.transform.position + dodgeDirection;
                    controller.unitMover.SetTargetPosition(targetPosition);
                    Debug.Log("Dodging to position: " + targetPosition);
                }
                else
                {
                    Safety = true;
                }
            }
            else
            {
                Safety = true;
            }

            // If there's no danger we roam randomly. Improve later if good idea
            if (Safety)
            {
                roamingTimer -= 0.1f;
                Aggressiveness += 0.1f;

                if (roamingTimer <= 0)
                {
                    roamingTimer = roamingInterval;
                    float randomAngle = Random.Range(0, 360);
                    roamingDirection = new Vector3(Mathf.Cos(randomAngle * Mathf.Deg2Rad), 0, Mathf.Sin(randomAngle * Mathf.Deg2Rad)).normalized;

                    // We will try to get the bot towards the center:
                    float centerWeight = 0.5f;
                    Vector3 centerDirection = (Vector3.zero - controller.transform.position).normalized;
                    Vector3 combinedDirection = (roamingDirection + centerDirection * centerWeight).normalized;
                    roamingDirection = combinedDirection;
                }

                Vector3 targetPosition = controller.transform.position + roamingDirection * roamingSpeed;
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

        float size = 10f;
        Collider[] detected = new Collider[20];
        List<ProjectileInstance> projectiles = new List<ProjectileInstance>();
        Physics.OverlapBoxNonAlloc(controller.transform.position + Vector3.up, new Vector3(size, 0.5f, size), detected);

        for (int i = 0; i < detected.Length; i++)
        {
            if (detected[i] == null)
            {
                continue; // Skip null detected objects??
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

            Vector3 projVelocity = projectile.GetComponent<Rigidbody>().velocity;
            Vector3 directionToController = controller.transform.position - projectile.transform.position;
            float angle = Vector3.Angle(projVelocity, directionToController);

            if (angle > 30)
            {
                continue;
            }

            Vector3 perpendicularDirection1 = Vector3.Cross(projVelocity.normalized, Vector3.up).normalized;
            Vector3 perpendicularDirection2 = -perpendicularDirection1;
            Vector3 futurePosition1 = controller.transform.position + perpendicularDirection1;
            Vector3 futurePosition2 = controller.transform.position + perpendicularDirection2;

            float distanceToProjectile1 = (futurePosition1 - projectile.transform.position).magnitude;
            float distanceToProjectile2 = (futurePosition2 - projectile.transform.position).magnitude;

            Vector3 bestDodgeDirection = distanceToProjectile1 > distanceToProjectile2 ? perpendicularDirection1 : perpendicularDirection2;
            dodgeDirection += bestDodgeDirection / (controller.transform.position - projectile.transform.position).magnitude;
        }

        return dodgeDirection.normalized;
    }
}