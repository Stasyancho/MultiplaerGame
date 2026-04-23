using Mirror;
using UnityEngine;
using UnityEngine.UI;

public class Player : NetworkBehaviour
{
    [SerializeField] private int maxHealth = 5;
    [SyncVar(hook = nameof(OnHealthChanged))]
    private int currentHealth;
    
    [SerializeField] private Slider healthSlider; 
    
    private void Start()
    {
        if (isServer)
            currentHealth = maxHealth;
        
        if (healthSlider != null)
        {
            healthSlider.maxValue = maxHealth;
            healthSlider.value = currentHealth;
        }
    }

    [Server]
    public void TakeDamage(int amount)
    {
        if (currentHealth <= 0) return;
        currentHealth = Mathf.Max(0, currentHealth - amount);
        if (currentHealth == 0)
            Die();
    }

    [Server]
    private void Die()
    {
        // Логика смерти
        RpcOnDeath();
    }

    [ClientRpc]
    private void RpcOnDeath()
    {
        Debug.Log("Player died");
        // Визуальные эффекты
    }

    private void OnHealthChanged(int oldValue, int newValue)
    {
        if (healthSlider != null)
            healthSlider.value = newValue;
    }
}
