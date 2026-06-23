using UnityEngine;

public class KnifeController : WeaponController
{

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    protected override void Start()
    {
        base.Start();
    }

    protected override void Attack()
    {
        base.Attack();

        // Find the nearest enemy
        Transform nearestEnemy = FindNearestEnemy();

        // Do not fire if there are no enemies
        if (nearestEnemy == null) return;

        // Calculate the direction from the player to the enemy
        Vector3 direction = (nearestEnemy.position - transform.position).normalized;

        GameObject spawnedKnife = Instantiate(weaponData.Prefab);
        spawnedKnife.transform.position = transform.position;

        spawnedKnife.GetComponent<KnifeBehavior>().DirectionChecker(direction);


    }

    Transform FindNearestEnemy()
    {
        // Find all objects on the field with the tag "Enemy"
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");

        Transform nearest = null;
        float shortestDistance = Mathf.Infinity;

        foreach (GameObject enemy in enemies)
        {
            float distance = Vector2.Distance(
                transform.position,      // Player position
                enemy.transform.position // Enemy position
            );

            if (distance < shortestDistance)
            {
                shortestDistance = distance;
                nearest = enemy.transform;
            }
        }

        return nearest; // If there are no enemies, return null.
    }
}
  