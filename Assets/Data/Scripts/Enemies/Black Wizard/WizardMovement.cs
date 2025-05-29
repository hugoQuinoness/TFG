using System.Collections;
using UnityEngine;

public class WizardMovement : MonoBehaviour
{
    enum State { Idle, Chasing, Attacking, Cooldown }

    [Header("Movement")]
    public float moveSpeed = 2f;
    public float stopDistance = 1f;          // when to stop chasing
    public float retreatDistance = 0.5f;     // min distance boss ever gets
    public float wobbleMagnitude = 0.3f;     // sideways noise
    public float wobbleSpeed = 4f;           // noise frequency

    [Header("Attack")]
    public float attackDistance = 1.2f;
    public float attackWindup = 0.3f;        // time before hit
    public float attackDuration = 0.2f;      // hit active time
    public float cd = 1.5f;                // cooldown after attack
    public GameObject attackHitbox;

    // control flags
    public bool canMove = true;
    public bool canAttack = true;

    private State currentState = State.Idle;
    private Transform target;
    private Rigidbody2D rb;
    private Animator animator;
    private Vector3 originalScale;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        originalScale = transform.localScale;
    }

    void Start()
    {
        target = PlayerControler.Instance.transform;
        EnterState(State.Idle);
    }

    void Update()
    {
        if (!canMove || target == null)
            return;

        float dist = Vector2.Distance(transform.position, target.position);

        switch (currentState)
        {
            case State.Idle:
                if (dist < 5f)
                    EnterState(State.Chasing);
                break;

            case State.Chasing:
                HandleChasing(dist);
                break;

            case State.Attacking:
                // attack coroutine is already running
                break;

            case State.Cooldown:
                // waiting for cooldown to finish
                break;
        }
    }

    void HandleChasing(float dist)
    {
        float dirX = target.position.x - transform.position.x;
        transform.localScale = new Vector3(Mathf.Sign(dirX) * Mathf.Abs(originalScale.x), originalScale.y, originalScale.z);

        Vector2 toTarget = (target.position - transform.position).normalized;
        Vector2 perp = new Vector2(-toTarget.y, toTarget.x);
        float wobble = Mathf.Sin(Time.time * wobbleSpeed) * wobbleMagnitude;
        Vector2 noisyDir = (toTarget + perp * wobble).normalized;

        if (dist < retreatDistance)
        {
            noisyDir = -toTarget;
        }


        rb.velocity = noisyDir * moveSpeed;
        animator.SetBool("Run", true);

        if (canAttack && dist <= attackDistance)
        {
            EnterState(State.Attacking);
        }

    }

    private void EnterState(State newState)
    {
        currentState = newState;

        switch (newState)
        {
            case State.Idle:

            case State.Attacking:
                rb.velocity = Vector2.zero;
                animator.SetBool("Run", false);
                StartCoroutine(DoAttack());
                break;


            case State.Cooldown:
                rb.velocity = Vector2.zero;
                animator.SetBool("Run", false);
                break;
        }
    }

    public IEnumerator DoAttack()
    {
        // Temporarily block further attacks
        canAttack = false;

        // Active hit
        attackHitbox.SetActive(true);
        animator.SetTrigger("Attack");
        yield return new WaitForSeconds(attackDuration);
        attackHitbox.SetActive(false);

        EnterState(State.Cooldown);
        yield return new WaitForSeconds(cd);

        if (canMove)
        {
            EnterState(State.Chasing);
        }

        canAttack = true;
    }

    public void StopMovement()
    {
        canMove = false;
        canAttack = false;
        rb.velocity = Vector2.zero;
        attackHitbox.SetActive(false);
        animator.SetBool("Run", false);
        currentState = State.Idle;
        StopAllCoroutines();
    }

    public void ResumeMovement()
    {
        canMove = true;
        canAttack = true;
        EnterState(State.Chasing);
    }

}
