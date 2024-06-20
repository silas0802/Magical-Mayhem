using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[CreateAssetMenu(fileName = "Hard Fighting Logic", menuName = "Game/AI/Fighting Logic/Hard Bot")]
public class HardFightingLogic : FightingLogic
{
    private readonly System.Random random = new();

    [SerializeField] private float moverInterval = 0.62f;
    [SerializeField] private float predictionFactorFireball = 1f;
    [SerializeField] private float mean = 2f;
    [SerializeField] private float standardDeviation = 0.5f;
    [SerializeField] private int healthThresholdForDefensiveMode = 80;

    private List<ProjectileInstance> nearbyProjectileInstances = new List<ProjectileInstance>();
    private bool projectileIsIncoming;
    private bool enemyIsInRange;
    private float moveTimer;
    private float offensiveTimer;
    private float toReduceCallsToGetNearestHealthBuff;
    private int healthPercent;
    private int lavaTileCounter;
    private int mapSize;
    private UnitController nearestUnit;
    
    private Vector3 lastTargetPosition;
    private List<Vector3> wallPositions;

    public override void HandleFightingLogic(UnitController controller)
    {
        moveTimer -= Time.deltaTime;
        offensiveTimer -= Time.deltaTime;
        toReduceCallsToGetNearestHealthBuff -= Time.deltaTime;

        healthPercent = controller.GetHealth();

        // Update local variables
        EvaluateSurroundings(controller);
        HandleOffensiveActions(controller);

        if (projectileIsIncoming)
        {
            PerformDodge(controller);
        }
        else if (moveTimer < 0)
        {
            Roam(controller);
        }
    }

    private void EvaluateSurroundings(UnitController controller)
    {
        nearbyProjectileInstances = FightingHelpers.DetectNearbyProjectiles(controller);
        projectileIsIncoming = FightingHelpers.IsProjectileIncoming(controller, nearbyProjectileInstances, 30);
        nearestUnit = RoundManager.instance.FindNearestUnit(controller.transform.position, controller);
        enemyIsInRange = FightingHelpers.IsEnemyInRange(controller, 15f);
        //healthBuffPositions = MapGenerator.instance.GetHealthBuffPositions();
        //speedBuffPositions = MapGenerator.instance.GetSpeedBuffPositions();
        lavaTileCounter = MapGenerator.instance.lavaTileCounter;
        wallPositions = MapGenerator.instance.GetWallPositions();
        mapSize = MapGenerator.instance.GetMapSize();
    }

    private void HandleOffensiveActions(UnitController controller)
    {
        if (nearestUnit != null && offensiveTimer < 0 && Vector3.Distance(nearestUnit.transform.position, controller.transform.position) < 20)
        {
            if (FightingHelpers.ClearPath(controller, nearestUnit))
            {
                int roundNumber = RoundManager.instance.roundNumber;
                int fireIndex = DetermineFireIndex(roundNumber);

                Vector3 targetPosition = nearestUnit.transform.position;
                Vector3 targetVelocity = nearestUnit.GetComponent<Rigidbody>().velocity;
                Vector3 predictedPosition = PredictTargetPosition(fireIndex, targetPosition, targetVelocity);

                FightingHelpers.FireIndexAtPosition(controller, fireIndex, predictedPosition);
                offensiveTimer = GetNormalCooldown(mean, standardDeviation);
            }
        }
    }

    private Vector3 PredictTargetPosition(int fireIndex, Vector3 targetPosition, Vector3 targetVelocity)
    {
        if (fireIndex == 0)
        {
            return targetPosition + targetVelocity * predictionFactorFireball;
        }
        else
        {
            return targetPosition + targetVelocity;
        }
    }

    private bool MoveTowardsHealthBuff(UnitController controller)
    {
        if (MapGenerator.instance != null)
        {
            Vector3 targetPosition = MapGenerator.instance.GetClosestHealthBuff(controller.transform.position);
            if (targetPosition == controller.transform.position) // Will return unit controller position if the buffArray of positions isn't intact or we don't find any buff.
            {
                toReduceCallsToGetNearestHealthBuff = 5f; 
                // This is if something goes wrong while checking the network objects.
                // We reduce the calls and hopefully next time the call will return a valid healthBuff position to go to.
                return false;
            }
            
            targetPosition = AdjustForObstacles(controller, targetPosition);
            controller.unitMover.SetTargetPosition(targetPosition);
            toReduceCallsToGetNearestHealthBuff = 1f;
            return true;
        } 

        return false;
    }

    private void Roam(UnitController controller)
    {
        if (healthPercent <= healthThresholdForDefensiveMode)
        {
            if (toReduceCallsToGetNearestHealthBuff < 0) {
                if(MoveTowardsHealthBuff(controller)) {
                    moveTimer = 1f;
                    return; //We got a position, don't find another one, so we return and movetimer will be 2s to wait for the buff to be grabbed.
                }
            } 
        }

        Vector3 strategicPoint = DetermineStrategicPoint(controller);
        Vector3 directionToStrategicPoint = (strategicPoint - controller.transform.position).normalized;

        // Randomness to make the movement more humane
        float randomAngle = UnityEngine.Random.Range(-45, 45);
        Vector3 randomDirection = Quaternion.Euler(0, randomAngle, 0) * directionToStrategicPoint;
        Vector3 targetPosition = controller.transform.position + randomDirection;

        // Ensure the target position is within the map.
        targetPosition = ClampPositionWithinRange(targetPosition, -(mapSize/2) + lavaTileCounter, (mapSize / 2) - lavaTileCounter);
        targetPosition = AdjustForObstacles(controller, targetPosition);
        controller.unitMover.SetTargetPosition(targetPosition);
        moveTimer = moverInterval;
    }

    private Vector3 DetermineStrategicPoint(UnitController controller)
    {
        Vector3 center = Vector3.zero;
        Vector3 nearestEnemy = Vector3.zero;
        Vector3 nearestResource = Vector3.zero;

        if (RoundManager.instance != null)
        {
            UnitController nearestEnemyController = RoundManager.instance.FindNearestUnit(controller.transform.position, controller);
            if (nearestEnemyController != null)
            {
                nearestEnemy = nearestEnemyController.transform.position;
            }
        }

        if (MapGenerator.instance != null)
        {
            nearestResource = MapGenerator.instance.GetClosestHealthBuff(controller.transform.position);
        }

        float centerWeight = 0.15f;
        float enemyWeight = 0.45f;
        float resourceWeight = 0.45f;

        // Combined:
        Vector3 strategicPoint = center * centerWeight + nearestEnemy * enemyWeight;
        if (nearestResource != Vector3.zero)
        {
            strategicPoint += nearestResource * resourceWeight;
        }
        
        float randomOffsetX = UnityEngine.Random.Range(-(mapSize/2) + lavaTileCounter, (mapSize / 2) - lavaTileCounter);
        float randomOffsetZ = UnityEngine.Random.Range(-(mapSize/2) + lavaTileCounter, (mapSize / 2) - lavaTileCounter);
        strategicPoint += new Vector3(randomOffsetX, 0, randomOffsetZ);

        return strategicPoint;
    }

private Vector3 GetNearestEnemyPosition(UnitController controller)
{
    // Implement logic to find the nearest enemy position
    return new Vector3(0, 0, 0); // Placeholder
}

private Vector3 GetNearestResourcePosition(UnitController controller)
{
    // Implement logic to find the nearest resource position
    return new Vector3(0, 0, 0); // Placeholder
}

private Vector3 GetSafeZonePosition(UnitController controller)
{
    // Implement logic to find a safe zone position
    return new Vector3(0, 0, 0); // Placeholder
}


    // Helper method to clamp the position within the specified range
    private Vector3 ClampPositionWithinRange(Vector3 position, int min, int max)
    {
        float clampedX = Mathf.Clamp(position.x, min, max);
        float clampedZ = Mathf.Clamp(position.z, min, max);
        return new Vector3(clampedX, position.y, clampedZ);
    }

    private Vector3 AdjustForObstacles(UnitController controller, Vector3 targetPosition)
    {
        Vector3 origin = controller.transform.position;
        Vector3 forwardDirection = controller.transform.forward;
        float rayLength = 2f;
        origin.y = 1.45f; // The walls are placed in the height.

        if (Physics.Raycast(origin, forwardDirection, out RaycastHit hit, rayLength))
        {
            if (!hit.collider.CompareTag("Wall"))
            {
                return targetPosition;
            }

            Vector3 leftDirection = Quaternion.Euler(0, -90, 0) * forwardDirection;
            Vector3 rightDirection = Quaternion.Euler(0, 90, 0) * forwardDirection;
            
            if (!Physics.Raycast(origin, leftDirection, rayLength))
            {
                origin.y = 0;
                return origin + leftDirection * rayLength;
            }
            else if (!Physics.Raycast(origin, rightDirection, rayLength))
            {
                origin.y = 0;
                return origin + rightDirection * rayLength;
            }
            else
            {
                return targetPosition;
            }
        }

        return targetPosition;
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
            if (projectile == null) // Not sure if projectile could ever be null??
            {
                continue;
            }

            Vector3 projVelocity = projectile.GetComponent<Rigidbody>().velocity;
            Vector3 directionToController = controller.transform.position - projectile.transform.position;

            if (Vector3.Distance(controller.transform.position, projectile.transform.position) < 2.5)
            {
                FightingHelpers.FireIndexAtPosition(controller, 5, controller.transform.position);
            }
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

        controller.unitMover.SetTargetPosition(controller.transform.position + dodgeDirection * 5);
        moveTimer = 2 * moverInterval;
    }
}
