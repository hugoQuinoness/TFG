using System;
using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Animator), typeof(Rigidbody2D), typeof(BoxCollider2D))]
public class PlayerControler : MonoBehaviour
{
    public static PlayerControler instance;

    [Header("Movement Settings")]
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float runSpeed = 8f;
    [SerializeField] private float directionSmoothing = 10f;
    private Vector2 lastMoveDirection;
    private Vector2 moveInput;
    public Transform Aim;
    private float speed;
    private float currentSpeed;

    private bool canMove;

    [Header("Attack Settings")]
    public GameObject attackHitbox;
    private float attackCooldown = 0.7f;
    private bool canAttack = true;

    private Animator animator;
    private Rigidbody2D rb;
    private Vector2 targetDirection;
    public bool isRunning;
    private PlayerInput playerInput;

    private Action<InputAction.CallbackContext> moveCanceledDelegate;
    private Action<InputAction.CallbackContext> attackPerformedDelegate;
    private Action<InputAction.CallbackContext> runPerformedDelegate;
    private Action<InputAction.CallbackContext> runCanceledDelegate;


    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != this)
        {
            Destroy(gameObject);
        }

        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        playerInput = EventSystem.current.GetComponent<PlayerInput>();

        // Store delegates for proper unsubscribe
        moveCanceledDelegate = ctx => SetLastMoveInput();
        attackPerformedDelegate = ctx => HandleAttack();
        runPerformedDelegate = ctx => { isRunning = true; animator.SetBool("isRunning", true); };
        runCanceledDelegate = ctx => { isRunning = false; animator.SetBool("isRunning", false); };

        playerInput.actions["Move"].canceled += moveCanceledDelegate;
        playerInput.actions["Attack"].performed += attackPerformedDelegate;
        playerInput.actions["Run"].performed += runPerformedDelegate;
        playerInput.actions["Run"].canceled += runCanceledDelegate;
    }

    void OnDestroy()
    {
        if (playerInput != null && playerInput.actions != null)
        {
            playerInput.actions["Move"].canceled -= moveCanceledDelegate;
            playerInput.actions["Attack"].performed -= attackPerformedDelegate;
            playerInput.actions["Run"].performed -= runPerformedDelegate;
            playerInput.actions["Run"].canceled -= runCanceledDelegate;
        }
    }

    void Update()
    {
        GetInputDirection();
        GetCharacterSpeed();
        animator.SetFloat("Speed", currentSpeed);
        animator.SetBool("isMoving", moveInput.magnitude > 0.1f);
    }


    void FixedUpdate()
    {
        if (!canMove)
        {
            rb.velocity = Vector2.zero;
            return;
        }

        MoveCharacter();
        CharacterAim();
    }

    public void GetCharacterSpeed()
    {
        if (targetDirection.magnitude > 0)
        {
            currentSpeed = isRunning ? runSpeed : moveSpeed;
        }
        else
        {
            currentSpeed = 0f;
        }
    }

    private void GetInputDirection()
    {

        if (!canMove)
        {
            rb.velocity = Vector2.zero;
            return;
        }

        moveInput = playerInput.actions["Move"].ReadValue<Vector2>();

        if (moveInput.sqrMagnitude > 0.01f)
        {
            lastMoveDirection = moveInput;
        }

        animator.SetFloat("InputX", moveInput.x);
        animator.SetFloat("InputY", moveInput.y);

        targetDirection = moveInput.normalized;

        isRunning = Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift);
    }



    private void MoveCharacter()
    {
        if (targetDirection.magnitude > 0)
        {
            speed = isRunning ? runSpeed : moveSpeed;

            Vector2 desiredPosition = (Vector2)transform.position + targetDirection * speed * Time.fixedDeltaTime;

            rb.MovePosition(desiredPosition);
        }
        else
        {
            rb.velocity = Vector2.zero;

            speed = 0f;
        }
    }

    private void CharacterAim()
    {
        Vector2 directionToUse = moveInput.sqrMagnitude > 0.01f ? moveInput : lastMoveDirection;

        Vector3 vector3 = Vector3.left * directionToUse.x + Vector3.down * directionToUse.y;
        Aim.rotation = Quaternion.LookRotation(Vector3.forward, vector3);
    }


    private void SetLastMoveInput()
    {
        animator.SetFloat("LastInputX", lastMoveDirection.x);
        animator.SetFloat("LastInputY", lastMoveDirection.y);

        Vector3 vector3 = Vector3.left * lastMoveDirection.x + Vector3.down * lastMoveDirection.y;
        Aim.rotation = Quaternion.LookRotation(Vector3.forward, vector3);
    }

    private void HandleAttack()
    {
        if (!canAttack)
        {
           return;  
        } 
        Debug.Log("Attack");
        StartCoroutine(HandleAttackCoroutine());
    }

    private IEnumerator HandleAttackCoroutine()
    {
        canAttack = false;
        animator.SetTrigger("Attack");
        canMove = false;
        attackHitbox.SetActive(true);
        yield return new WaitForSeconds(0.5f);
        attackHitbox.SetActive(false);
        canMove = true;
        yield return new WaitForSeconds(attackCooldown);
        canAttack = true;
    }
}