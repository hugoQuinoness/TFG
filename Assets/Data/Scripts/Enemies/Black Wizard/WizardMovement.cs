using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WizardMovement : MonoBehaviour
{
    public float speed = 2f;
    public float stopDistance = 0.5f; // Distancia mínima para detenerse cerca del jugador
    private Rigidbody2D rb;
    private Transform target;
    private Vector2 moveDirection;
    private Animator animator;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>(); // Obtener el componente Animator
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
            }
            else
            {
                moveDirection = Vector2.zero;
                animator.SetBool("Run", false);
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

