using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    private int playerHealth;
    private Image imagen;
    void Start()
    {
        imagen = GetComponent<Image>();
        imagen.fillAmount = 0.5f;
    }

    
    void Update()
    {
        playerHealth = PlayerControler.instance.GetComponent<Health>().health;

    }
}
