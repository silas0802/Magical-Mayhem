using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Easy Fighting Logic", menuName = "Game/AI/Fighting Logic/Easy Bot")]
public class EasyFightingLogic : FightingLogic
{   
    [SerializeField] private float moveDistance = 10f;
    [SerializeField] private float centerWeight = 10f;
    [SerializeField] private float roamingInterval = 15f;

    private enum State
    {
        Idle,
        Dodging,
        Shooting,
        Roaming
    }

    private State currentState = State.Idle;
    private List<ProjectileInstance> nearbyProjectileInstances = new List<ProjectileInstance>();
    private bool projectileIsIncoming;
    private bool enemyIsInRange;
    private float roamingTimer;
    private Vector3 roamingDirection;

    public override void HandleFightingLogic(UnitController controller)
    {
        nearbyProjectileInstances = FightingHelpers.DetectNearbyProjectiles(controller);
        projectileIsIncoming = FightingHelpers.IsProjectileIncoming(controller, nearbyProjectileInstances, 30);
        enemyIsInRange = FightingHelpers.IsEnemyInRange(controller, 15f);

        switch (currentState)
        {
            case State.Idle:
                EvaluateSurroundings(controller);
                break;
            case State.Dodging:
                PerformDodge(controller);
                break;
            case State.Shooting:
                TargetAndFire(controller);
                break;
            case State.Roaming:
                PerformRandomMovement(controller);
                break;
        }
    }

    private void PerformRandomMovement(UnitController controller)
    {
        roamingTimer -= 0.1f;

        if (roamingTimer <= 0)
        {
            roamingTimer = roamingInterval;
            float randomAngle = Random.Range(0, 360);
            roamingDirection = new Vector3(Mathf.Cos(randomAngle * Mathf.Deg2Rad), 0, Mathf.Sin(randomAngle * Mathf.Deg2Rad)).normalized;
            
            // We will try to get the bot towards the center:
            Vector3 centerDirection = (Vector3.zero - controller.transform.position).normalized;
            Vector3 combinedDirection = (roamingDirection + centerDirection * centerWeight).normalized;
            roamingDirection = combinedDirection;
        }

        Vector3 targetPosition = controller.transform.position + roamingDirection * moveDistance;
        controller.unitMover.SetTargetPosition(targetPosition);

        currentState = State.Idle;
    }

    private void EvaluateSurroundings(UnitController controller)
    {
        if (projectileIsIncoming)
        {
            currentState = State.Dodging;
        }
        else if (enemyIsInRange)
        {
            currentState = State.Shooting;
        }
        else
        {
            currentState = State.Roaming;
        }
    }

    private void PerformDodge(UnitController controller)
    {
        Vector3 dodgeDirection = Vector3.zero;
        Vector3 targetPosition = Vector3.zero;
        foreach (ProjectileInstance projectile in nearbyProjectileInstances)
        {
            if (projectile == null) continue;

            Vector3 projVelocity = projectile.GetComponent<Rigidbody>().velocity;
            dodgeDirection = projVelocity.normalized;
        }

        if (dodgeDirection != Vector3.zero)
        {
            dodgeDirection.Normalize();
            targetPosition = controller.transform.position + dodgeDirection * 5f;
            controller.unitMover.SetTargetPosition(targetPosition);
        }

        currentState = State.Idle;
    }

    private void TargetAndFire(UnitController controller)
    {
        UnitController nearestUnit = RoundManager.instance.FindNearestUnit(controller.transform.position, controller);
        FightingHelpers.FireIndexAtPosition(controller, 0, nearestUnit.transform.position);
        currentState = State.Idle;
    }
    
}
