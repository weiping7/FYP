using UnityEngine;

public class EnemyStats : MonoBehaviour
{
    public EnemyScriptableObject enemyData;

    float currentMoveSpeed;
    float currentHealth;
    float currentDemage;

    private void Awake()
    {
        currentMoveSpeed = enemyData.MoveSpeed;
        currentHealth = enemyData.MaxHealth;
        currentDemage = enemyData.Demage;
    }

    public void TakeDemage(float dmg)
    {
        currentDemage -= dmg;

        if (currentDemage <= 0)
        {
            Kill();
        }

    }

    public void Kill()
    {
        Destroy(gameObject);
    }
}
