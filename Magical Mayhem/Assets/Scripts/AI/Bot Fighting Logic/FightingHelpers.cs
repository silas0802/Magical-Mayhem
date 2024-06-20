using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public static class FightingHelpers
{
    public static bool IsProjectileIncoming(UnitController controller, List<ProjectileInstance> nearbyProjectileInstances, int angleWeight)
    {
        foreach (ProjectileInstance projectile in nearbyProjectileInstances)
        {
            if (projectile == null)
            {
                continue;
            }

            Vector3 projVelocity = projectile.GetComponent<Rigidbody>().velocity;
            Vector3 directionToController = controller.transform.position - projectile.transform.position;
            float angle = Vector3.Angle(projVelocity, directionToController);

            if (angle < angleWeight)
            {
                return true;
            }
        }

        return false;
    }

    
    public static List<ProjectileInstance> DetectNearbyProjectiles(UnitController controller)
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

    public static bool IsEnemyInRange(UnitController controller, float range)
    {
        UnitController nearestOpponent = RoundManager.instance.FindNearestUnit(controller.transform.position, controller);
        if (Vector3.Distance(controller.transform.position, nearestOpponent.transform.position) <= range)
        {
            return true;
        }

        return false;
    }

    public static List<UnitController> DetectEnemiesInRange(UnitController controller, float range)
    {
        Collider[] detected = new Collider[8];
        List<UnitController> units = new List<UnitController>();
        int numDetected = Physics.OverlapBoxNonAlloc(controller.transform.position + Vector3.up, new Vector3(range, 0.5f, range), detected);

        for (int i = 0; i < numDetected; i++)
        {
            if (detected[i] == null)
            {
                continue; 
            }

            UnitController unit = detected[i].GetComponent<UnitController>();
            if (unit != null)
            {
                units.Add(unit);
            }
        }
        
        return units;
    }

    public static void FireIndexAtPosition(UnitController controller, int index, Vector3 position)
    {
        controller.unitCaster.TryCastSpell(index, position);
    }

    public static IEnumerator WaitForMovement(UnitController controller, Vector3 targetPosition, System.Action onComplete)
    {
        while (Vector3.Distance(controller.transform.position, targetPosition) >= 0.1f)
        {
            yield return null;
        }
        Debug.Log("Reached Target Position");
        onComplete?.Invoke();
        Debug.Log("Completed move");
    }

    public static float GetDistanceBetweenTwoVector3(Vector3 vector1, Vector3 vector2)
    {
        float deltaX = vector1.x - vector2.x;
        float deltaZ = vector1.z - vector2.z;

        return Mathf.Sqrt((deltaX * deltaX) + (deltaZ * deltaZ));
    }

    public static bool ClearPath(UnitController controller, UnitController nearestUnit)
    {
        Vector3 origin = controller.transform.position;
        origin.y = 1.45f;

        Vector3 direction = (nearestUnit.transform.position - origin).normalized;
        float rayLength = Vector3.Distance(origin, nearestUnit.transform.position);

        if (Physics.Raycast(origin, direction, out RaycastHit hit, rayLength))
        {
            if (hit.collider.CompareTag("Wall"))
            {
                return false;
            }
        }

        return true;
    }
}
