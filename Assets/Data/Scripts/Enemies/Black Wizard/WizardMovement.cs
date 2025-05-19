using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WizardMovement : MonoBehaviour
{
    public float speed = 2f;
    public float stopDistance = 0.5f; // Distancia m�nima para detenerse cerca del jugador
    public float attackDistance = 1f; // Distancia m�nima para atacar al jugador
    private Rigidbody2D rb;
    private Transform target;
    private Vector2 moveDirection;
    private Animator animator;
    private SpriteRenderer spriteRenderer; // Referencia al componente SpriteRenderer

    private bool canMove = true; // Variable para controlar el movimiento

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
        if (!canMove)
        {
            return;
        }

        if (target)
        {
            Vector3 direction = (target.position - transform.position).normalized;
            float distance = Vector3.Distance(target.position, transform.position);


            if (distance > stopDistance)
            {
                moveDirection = direction;
                animator.SetBool("Run", true);

                // Invertir el sprite dependiendo de la direcci�n
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

                // Activar la animaci�n de ataque si est� dentro de la distancia de ataque
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
    
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("PlayerDamageCollider"))
        {
            canMove = false;
            animator.Play("Death");
        
        }
    }
}



