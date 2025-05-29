using System;
using System.Collections;
using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    public int health = 100;

    private PlayerControler playerController;

    public static event Action OnDeath;

    public static event Action OnRegenTick;

    public static event Action OnRegenEnd;

    private void Awake()
    {
        playerController = GetComponent<PlayerControler>();
    }

    public void TakeDamage(int damage)
    {
        if (!playerController.canBeHit)
        {
            return;
        }

        health -= damage;
        if (health <= 0)
        {
            Die();
        }
        else
        {
            StartCoroutine(TakeHit(damage));
        }
    }

    public void Die()
    {
        OnDeath?.Invoke();
    }

    public IEnumerator TakeHit(int damage)
    {
        playerController.canMove = false;
        playerController.PlayHitSound();
        playerController.GetAnimator().SetTrigger("TakeHit");
        StartCoroutine(Flashing());
        StartCoroutine(Invulnerability());
        TakeDamage(damage);
        yield return new WaitForSeconds(0.5f);
        playerController.canMove = true;

    }


    private IEnumerator Flashing()
    {
        SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();
        Color originalColor = spriteRenderer.color;
        Color flashColor = Color.red;

        for (int i = 0; i < 3; i++)
        {
            spriteRenderer.color = flashColor;
            yield return new WaitForSeconds(0.1f);
            spriteRenderer.color = originalColor;
            yield return new WaitForSeconds(0.1f);
        }
    }

    private IEnumerator Invulnerability()
    {
        playerController.canBeHit = false;
        yield return new WaitForSeconds(2f);
        playerController.canBeHit = true;
    }

    public void HealOverTime(int healAmount, float duration)
    {
        StartCoroutine(HealOverTimeCoroutine(healAmount, duration));
    }

    private IEnumerator HealOverTimeCoroutine(int healAmount, float duration)
    {
        float elapsed = 0f;

        int healPerTick = healAmount / (int)(duration / 0.5f);

        if(health <= 100)
        {
            OnRegenTick?.Invoke();
        }
        else
        {
            yield break;
        }

        while (elapsed < duration)
        {
            health += healPerTick;
            health = Mathf.Min(health, 100);
            elapsed += 0.5f;
            if (health >= 100)
            {
                break;
            }
            yield return new WaitForSeconds(0.5f);
        }

        OnRegenEnd?.Invoke();
    }

}
