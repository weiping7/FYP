using UnityEngine;

public class EnemyStats : MonoBehaviour, IPoolable
{
    public EnemyScriptableObject enemyData;

    float currentMoveSpeed;
    float currentHealth;
    float currentDamage;

    private void Awake()
    {
        currentMoveSpeed = enemyData.MoveSpeed;
        currentHealth = enemyData.MaxHealth;
        currentDamage = enemyData.Damage;
    }

    public void TakeDamage(float dmg)
    {
        currentHealth -= dmg;

        if (currentHealth <= 0)
        {
            Kill();
        }
    }

    public void Kill()
    {
        EnemySpawner es = FindAnyObjectByType<EnemySpawner>();

        if (es != null)
        {
            es.OnEnemyKilled();
        }

        if (ObjectPoolManager.Instance != null)
        {
            ObjectPoolManager.Instance.ReleaseObject(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    /*private void OnDestroy()
    {
        EnemySpawner es = FindAnyObjectByType<EnemySpawner>();
        if (es != null && Application.isPlaying)
        {
            es.OnEnemyKilled();
        }
    }*/

    public void OnCollisionStay2D(Collision2D col)
    {
        if (col.gameObject.CompareTag("Player"))
        {
            PlayerStats player = col.gameObject.GetComponent<PlayerStats>();

            if (player != null)
            {
                player.TakeDamage(currentDamage);
            }
        }
    }

    void OnEnable()
    {
        if (!EnemySpawner.activeEnemies.Contains(this))
        {
            EnemySpawner.activeEnemies.Add(this);
        }
    }

    void OnDisable()
    {
        EnemySpawner.activeEnemies.Remove(this);
    }

    public void OnGetFromPool()
    {
        currentHealth = enemyData.MaxHealth;
        currentDamage = enemyData.Damage;
    }

    public void OnReturnToPool()
    {
        currentHealth = enemyData.MaxHealth;
    }

}
