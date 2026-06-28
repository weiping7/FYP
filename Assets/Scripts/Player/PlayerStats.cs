using UnityEngine;
using UnityEngine.UI;

public class PlayerStats : MonoBehaviour
{
    public CharacterScriptableObject characterData;

    [Header("Health")]
    public float currentHealth;
    public float maxHealth;


    float currentRecovery;
    float currentMoveSpeed;
    float currentMight;
    float currentProjectileSpeed;
    
    //i-Frames
        [Header("I-Frames")]
        public float invincibilityDuration = 0.5f;
        float invincibilityTimer;
        bool isInvincible;

    [Header("Health Regen")]
    public float regenDelay = 3f;
    public float regenAmount = 2f;
    public float regenInterval = 1f;

    float timeSinceLastDamage;
    float regenTimer;

    [Header("UI")]
    public Slider healthBar;

    private void Awake()
    {
        //Assign the variables
        maxHealth = characterData.MaxHealth;
        currentHealth = maxHealth;

        UpdateHealthBar();

        currentRecovery = characterData.Recovery;
        currentMoveSpeed = characterData.MoveSpeed;
        currentMight = characterData.Might;
        currentProjectileSpeed = characterData.ProjectileSpeed;

    }

    void Update()
    {
        HandleInvincibility();
        HandleHealthRegen();
    }

    void HandleInvincibility()
    {
        if (invincibilityTimer > 0)
        {
            invincibilityTimer -= Time.deltaTime;
        }
        else
        {
            isInvincible = false;
        }
    }

    void HandleHealthRegen()
    {
        if (currentHealth >= maxHealth)
        {
            return;
        }

        timeSinceLastDamage += Time.deltaTime;

        if (timeSinceLastDamage < regenDelay)
        {
            return;
        }

        regenTimer += Time.deltaTime;

        if (regenTimer >= regenInterval)
        {
            regenTimer = 0f;
            Heal(regenAmount);
        }
    }

    public void TakeDamage(float damage)
    {
        if (isInvincible)
        {
            return;
        }

        currentHealth -= damage;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);

        timeSinceLastDamage = 0f;
        regenTimer = 0f;

        invincibilityTimer = invincibilityDuration;
        isInvincible = true;

        UpdateHealthBar();

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    public void Heal(float amount)
    {
        currentHealth += amount;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);

        UpdateHealthBar();
    }

    void UpdateHealthBar()
    {
        if (healthBar != null)
        {
            healthBar.value = currentHealth / maxHealth;
        }
    }

    void Die()
    {
        Debug.Log("PLAYER IS DEAD");
    }


}
