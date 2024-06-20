using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Medium Fighting Logic", menuName = "Game/AI/Fighting Logic/Medium Bot")]
public class MediumFightingLogic : FightingLogic
{
    private readonly System.Random random = new();

    [SerializeField] private float predictionFactorFireball = 1f;
    [SerializeField] private float mean = 2f;
    [SerializeField] private float standardDeviation = 0.5f;

    private List<ProjectileInstance> nearbyProjectileInstances = new List<ProjectileInstance>();
    private bool projectileIsIncoming;
    private bool enemyIsInRange;
    private float moveTimer;
    private float offensiveTimer;
    private UnitController nearestUnit;


    public override void HandleFightingLogic(UnitController controller)
    {
        moveTimer -= Time.deltaTime;
        offensiveTimer -= Time.deltaTime;

        // Update local variables..
        EvaluateSurroundings(controller);

        //Shooting logic here.
        //0 = Fireball (Common) > round 1
        //1 = Ice Prison (Little bit more rare than common) > round 2
        //2 = Flame Barrage (Overpowered, use rarely (low chance)) > round 3

        // HARD FIGHTING LOGIC ALSO GETS:
        //3 = Blink (Use only for dodge and fast health grab) > round 2
        //4 = Dash (Use for fast health grab in conjunction with Blink to grab far away health buffs)
        //5 = Barrier (Use to prevent a big hit)

        if (nearestUnit != null && offensiveTimer < 0)
        {
            int roundNumber = RoundManager.instance.roundNumber;
            int fireIndex = DetermineFireIndex(roundNumber);

            Vector3 targetPosition = nearestUnit.transform.position;
            Vector3 targetVelocity = nearestUnit.GetComponent<Rigidbody>().velocity;
            Vector3 predictedPosition = targetPosition;
            
            if (fireIndex == 0) {
                predictedPosition += targetVelocity * predictionFactorFireball;
            } else {
                predictedPosition += targetVelocity; 
            }

            FightingHelpers.FireIndexAtPosition(controller, fireIndex, predictedPosition);
            offensiveTimer = GetNormalCooldown(mean, standardDeviation);
        }
        
        if(projectileIsIncoming)
        {
            PerformDodge(controller);
        } else {
            if (moveTimer < 0)
            {
                float randomAngle = UnityEngine.Random.Range(0, 360);
                Vector3 roamingDirection = new Vector3(Mathf.Cos(randomAngle * Mathf.Deg2Rad), 0, Mathf.Sin(randomAngle * Mathf.Deg2Rad)).normalized;
                    
                // We will try to get the bot towards the center:
                Vector3 centerDirection = (Vector3.zero - controller.transform.position).normalized;
                Vector3 combinedDirection = (roamingDirection + centerDirection * 10).normalized;
                roamingDirection = combinedDirection;

                Vector3 targetPosition = controller.transform.position + roamingDirection * 10;
                controller.unitMover.SetTargetPosition(targetPosition);
                moveTimer = 2f;
            }
        }
    }

    private void EvaluateSurroundings(UnitController controller)
    {
        nearbyProjectileInstances = FightingHelpers.DetectNearbyProjectiles(controller);
        projectileIsIncoming = FightingHelpers.IsProjectileIncoming(controller, nearbyProjectileInstances, 30);
        nearestUnit = RoundManager.instance.FindNearestUnit(controller.transform.position, controller);
        enemyIsInRange = FightingHelpers.IsEnemyInRange(controller, 15f);
    }

    private int DetermineFireIndex(int roundNumber)
    {
        if (roundNumber == 1)
        {
            return 0;
        }
        else if (roundNumber <= 2)
        {
            return UnityEngine.Random.Range(1, 3) == 1 ? 1 : 0;
        }
        else
        {
            int roll = UnityEngine.Random.Range(1, 10);
            if (roll >= 8)
            {
                return 2;
            }
            else if (roll >= 4 && roll <= 7)
            {
                return 1;
            }
            else
            {
                return 0;
            }
        }
    }

    private float GetNormalCooldown(float mean, float standardDeviation)
    {
        double u1 = 1.0 - random.NextDouble();
        double u2 = 1.0 - random.NextDouble();
        double randStdNormal = Math.Sqrt(-2.0 * Math.Log(u1)) * Math.Sin(2.0 * Math.PI * u2);
        double randNormal = mean + standardDeviation * randStdNormal;
        return (float)randNormal;
    }

    private void PerformDodge(UnitController controller)
    {
        Vector3 dodgeDirection = Vector3.zero;

        foreach (ProjectileInstance projectile in nearbyProjectileInstances)
        {
            if (projectile == null) //Not sure if projectile could ever be null??
            {
                continue;
            }

            Vector3 projVelocity = projectile.GetComponent<Rigidbody>().velocity;
            Vector3 directionToController = controller.transform.position - projectile.transform.position;
            float angle = Vector3.Angle(projVelocity, directionToController);

            if (angle > 20)
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
            
        controller.unitMover.SetTargetPosition(controller.transform.position + dodgeDirection * 3f);
    }
}