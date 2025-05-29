using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    private int playerHealth;
    private Image imagen;

    Color originalColor;
    void Start()
    {
        imagen = GetComponent<Image>();
        imagen.fillAmount = 0.5f;
        PlayerHealth.OnRegenTick += OnRegenTick;
        PlayerHealth.OnRegenEnd += OnRegenEnd;
        originalColor = imagen.color;
    }

    private void OnDestroy()
    {
        PlayerHealth.OnRegenTick -= OnRegenTick;
        PlayerHealth.OnRegenEnd -= OnRegenEnd;
    }


    void Update()
    {
        playerHealth = PlayerControler.Instance.GetComponent<PlayerHealth>().health;

        if (playerHealth > 0)
        {
            imagen.fillAmount = playerHealth / 100f;
        }
        else
        {
            imagen.fillAmount = 0f;
        }
    }

    public void OnRegenTick()
    {
        imagen.color = Color.magenta;
    }

    public void OnRegenEnd()
    {
        imagen.color = originalColor;
    }
}
