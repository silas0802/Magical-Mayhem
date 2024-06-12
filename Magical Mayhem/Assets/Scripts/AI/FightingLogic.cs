using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[CreateAssetMenu(fileName = "New Fighting Logic", menuName = "Game/AI/Fighting Logic")]
public class FightingLogic : ScriptableObject
{   
    [SerializeField] private bool isOn = false;

    [SerializeField] private float Aggressiveness;
    [SerializeField] private bool Safety;
    [SerializeField] private float roamingSpeed = 1.0f;
    [SerializeField] private float roamingInterval = 2.0f;
    [SerializeField] private float shootingMean = 6.0f;
    [SerializeField] private float shootingStdDev = 1.0f;

    private Vector3 roamingDirection;
    private float roamingTimer;
    private float shootingTimer;
    private readonly System.Random random = new();


    public void HandleFightingLogic(UnitController controller)
    {
        if (isOn)
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
                        float centerWeight = 0.7f;
                        Vector3 centerDirection = (Vector3.zero - controller.transform.position).normalized;
                        Vector3 combinedDirection = (roamingDirection + centerDirection * centerWeight).normalized;
                        roamingDirection = combinedDirection;
                    }

                    Vector3 targetPosition = controller.transform.position + roamingDirection * roamingSpeed;
                    controller.unitMover.SetTargetPosition(targetPosition);
                }

                controller.frameCounter = 0; // Just so that numbers don't get too large.
            }

            // For the shooting logic
            shootingTimer -= Time.deltaTime;

            if (shootingTimer <= 0 && ShouldCastSpell(controller))
            {
                CastSpell(controller);

                // We reset shooting timer with a new normal distributed cooldown
                shootingTimer = GetNormalRandomValue(shootingMean, shootingStdDev); // Reset shooting timer with a new normal distributed cooldown
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
            catch (System.Exception)
            {
                
            }
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

    // This needs to be smart: Which spell to use, when, how?
    private void CastSpell(UnitController controller)
    {
        UnitController nearestOpponent = RoundManager.instance.FindNearestUnit(controller.transform.position, controller);
        controller.unitCaster.TryCastSpell(1,  nearestOpponent.transform.position);
    }
    
    private bool ShouldCastSpell(UnitController controller)
    {
        float randomValue = Random.Range(0f, 2f);
        if (Aggressiveness > 1.5f && randomValue < Aggressiveness / 2)
        {
            return true;
        }
        return false;
    }

    private float GetNormalRandomValue(float mean, float stdDev)
    {
        double u1 = 1.0 - random.NextDouble(); // Uniform
        double u2 = 1.0 - random.NextDouble();
        double randStdNormal = System.Math.Sqrt(-2.0 * System.Math.Log(u1)) * System.Math.Sin(2.0 * System.Math.PI * u2); // Random normal(0,1)
        float randNormal = mean + stdDev * (float)randStdNormal;

        return Mathf.Max(0.1f, randNormal);
    }
}
