using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WizardMovement : MonoBehaviour
{
    public float speed = 2f;
    public float stopDistance = 0.5f; // Distancia mínima para detenerse cerca del jugador
    public float attackDistance = 1f; // Distancia mínima para atacar al jugador
    private Rigidbody2D rb;
    private Transform target;
    private Vector2 moveDirection;
    private Animator animator;
    private SpriteRenderer spriteRenderer; // Referencia al componente SpriteRenderer

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>(); // Obtener el componente Animator
        spriteRenderer = GetComponent<SpriteRenderer>(); // Obtener el componente SpriteRenderer
    }

    void Start()
    {
        target = GameObject.FindGameObjectWithTag("Player").GetComponent<Transform>();
    }

    // Update is called once per frame
    void Update()
    {
        if (target)
        {
            Vector3 direction = (target.position - transform.position).normalized;
            float distance = Vector3.Distance(target.position, transform.position);


            if (distance > stopDistance)
            {
                moveDirection = direction;
                animator.SetBool("Run", true);

                // Invertir el sprite dependiendo de la dirección
                if (moveDirection.x > 0)
                {
                    spriteRenderer.flipX = false; // Mirar a la derecha
                }
                else if (moveDirection.x < 0)
                {
                    spriteRenderer.flipX = true; // Mirar a la izquierda
                }
            }
            else
            {
                moveDirection = Vector2.zero;
                animator.SetBool("Run", false);

                // Activar la animación de ataque si está dentro de la distancia de ataque
                if (distance <= attackDistance)
                {
                    animator.SetTrigger("Attack");
                }
            }
        }
    }

    private void FixedUpdate()
    {
        if (target)
        {
            rb.velocity = moveDirection * speed;
        }
    }
}



