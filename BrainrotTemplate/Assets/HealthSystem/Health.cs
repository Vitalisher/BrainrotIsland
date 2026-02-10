using System;
using System.Collections;
using TMPro;
using UnityEngine;

public class Health : MonoBehaviour, ITakeDamage
{
    public int MaxHealth = 100;
    public int CurrentHealth;
    public event Action onHealthChange; // НЕ static
    public bool isPlayerHealth;         // для игрока
    public bool isBot;                  // для бота
    public bool isDead { get; set; } = false;

    public static Action onPlayerDied;  // только для игрока
    public TMP_Text playerHealthText;
    public GameObject damageSprite;
    
    
    void Start()
    {
        CurrentHealth = MaxHealth;
        if (isPlayerHealth)
        {
            UpdateText();
        }
    }

    public void TakeDamage(int damage)
    {
        if (isDead) return;

        CurrentHealth -= damage;
        CurrentHealth = Mathf.Max(CurrentHealth, 0);

        onHealthChange?.Invoke();

        if (isPlayerHealth)
        {
            UpdateText();
            StartCoroutine(DamageSpriteVisible());
        }
        
        if (CurrentHealth <= 0)
        {
            Die();
        }
    }

    public void Die()
    {
        if (isDead) return;
        isDead = true;

        if (isPlayerHealth)
        {
            onPlayerDied?.Invoke();
        }
        else if (isBot)
        {
            BotManager.Instance?.OnBotDied(gameObject);
            Destroy(gameObject);
        }
    }
    
    public void UpdateText()
    {
        playerHealthText.text = CurrentHealth.ToString();
    }

    public IEnumerator DamageSpriteVisible()
    {
        damageSprite.SetActive(true);   
        yield return new WaitForSeconds(0.2f); 
        damageSprite.SetActive(false);   
    }
}