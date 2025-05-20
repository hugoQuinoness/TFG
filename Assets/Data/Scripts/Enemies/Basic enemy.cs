using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Basicenemy : MonoBehaviour
{
    public Transform player; // Asigna el transform del jugador en el inspector
    public float maxSpeed = 1.8f; // Velocidad máxima (más lento)
    public float chaseDistance = 10f; // Distancia máxima para empezar a perseguir
    public float minDistance = 0.5f; // Distancia mínima para detenerse cerca del jugador
    public float slowDownDistance = 1.5f; // Distancia desde la cual empieza a desacelerar

    private Animator animator;
    private Vector3 originalScale;
    private bool canAttack = true;

    void Start()
    {
        animator = GetComponent<Animator>();
        originalScale = transform.localScale;
    }

    void Update()
    {
        if (player == null) return;

        float distance = Vector3.Distance(transform.position, player.position);

        // Invertir sprite según la posición del jugador sin deformar
        if (player.position.x < transform.position.x)
        {
            transform.localScale = new Vector3(-Mathf.Abs(originalScale.x), originalScale.y, originalScale.z);
        }
        else
        {
            transform.localScale = new Vector3(Mathf.Abs(originalScale.x), originalScale.y, originalScale.z);
        }

        if (distance < chaseDistance && distance > minDistance)
        {
            float t = Mathf.InverseLerp(minDistance, slowDownDistance, distance);
            float currentSpeed = Mathf.Lerp(0, maxSpeed, t);

            Vector3 direction = (player.position - transform.position).normalized;
            transform.position += direction * currentSpeed * Time.deltaTime;

            animator.SetBool("Running", true);
        }
        else
        {
            animator.SetBool("Running", false);
        }

        // Ataque si está suficientemente cerca
        if (distance <= minDistance && canAttack)
        {
            StartCoroutine(AttackCoroutine());
        }
    }

    private IEnumerator AttackCoroutine()
    {
        canAttack = false;
        animator.SetTrigger("Attack");
        yield return new WaitForSeconds(1f); // Espera 1 segundo antes de permitir otro ataque
        canAttack = true;
    }
}