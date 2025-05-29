using System;
using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Animator), typeof(Rigidbody2D), typeof(BoxCollider2D))]
public class PlayerControler : MonoBehaviour
{
    public static PlayerControler Instance;

    [Header("Movement Settings")]
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float runSpeed = 8f;
    private Vector2 lastMoveDirection;
    private Vector2 moveInput;
    public Transform Aim;
    private float speed;
    private float currentSpeed;
    public bool canMove;
    public bool isRunning;
    public bool canBeHit = true;

    [Header("Attack Settings")]
    public GameObject attackHitbox;
    private float attackCooldown = 0.7f;
    private bool canAttack = true;
    private Animator animator;
    private Rigidbody2D rb;
    private Vector2 targetDirection;
    private PlayerInput playerInput;

    [Header("SFX")]

    public AudioClip attackSound;

    public AudioClip hitSound;

    public AudioClip deathSound;

    private AudioSource audioSource;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else if (Instance != this)
        {
            Destroy(gameObject);
        }

        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        audioSource = GetComponent<AudioSource>();
        playerInput = EventSystem.current.GetComponent<PlayerInput>();

        PlayerHealth.OnDeath += OnDeath;

        EventManager.AttackEvent += HandleAttack;

        EventManager.RunEvent += OnRunStarted;

        EventManager.RunCanceledEvent += OnRunCanceled;

        EventManager.MoveEvent += OnMoveEvent;

    }


    void OnDestroy()
    {
        if (Instance == this)
        {
            Instance = null;
        }

        PlayerHealth.OnDeath -= OnDeath;
        EventManager.AttackEvent -= HandleAttack;
        EventManager.RunEvent -= OnRunStarted;
        EventManager.RunCanceledEvent -= OnRunCanceled;
        EventManager.MoveEvent -= OnMoveEvent;
    }

    void Update()
    {
        if (!canMove)
        {
            rb.velocity = Vector2.zero;
            return;
        }
        if (DialogueManager.Instance.isDialogueActive)
        {
            rb.velocity = Vector2.zero;
            animator.SetBool("isMoving", false);
            return;
        }
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

        if (DialogueManager.Instance.isDialogueActive)
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

        if (DialogueManager.Instance.isDialogueActive)
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

    private void OnRunStarted()
    {
        animator.SetBool("isRunning", true);
        isRunning = true;
    }

    private void OnRunCanceled()
    {
        animator.SetBool("isRunning", false);
        isRunning = false;
    }

    private void OnMoveEvent()
    {
        SetLastMoveInput();
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

        if (!canMove)
        {
            rb.velocity = Vector2.zero;
            return;
        }

        if (DialogueManager.Instance.isDialogueActive)
        {
            rb.velocity = Vector2.zero;
            return;
        }

        if (moveInput.sqrMagnitude > 0.01f)
        {
            lastMoveDirection = moveInput;
        }

        SetLastMoveInput();

        StartCoroutine(HandleAttackCoroutine());
    }

    private IEnumerator HandleAttackCoroutine()
    {
        animator.SetTrigger("Attack");
        PlayAttackSound();
        canAttack = false;
        canMove = false;
        attackHitbox.SetActive(true);
        yield return new WaitForSeconds(0.5f);
        attackHitbox.SetActive(false);
        canMove = true;
        yield return new WaitForSeconds(attackCooldown);
        canAttack = true;
    }

    public void PlayAttackSound()
    {
        audioSource.PlayOneShot(attackSound);
    }

    public void PlayHitSound()
    {
        audioSource.PlayOneShot(hitSound);
    }

    public void OnDeath()
    {
        Debug.Log("Player has died.");
        audioSource.PlayOneShot(deathSound);
        canMove = false;
        canBeHit = false;
        animator.SetTrigger("Death");
        SongManager.Instance.PlaySongFromAddressable("GameOver");
        rb.velocity = Vector2.zero;
    }

    public Animator GetAnimator()
    {
        return animator;
    }

    private void UpdateSFXAudioVolume(float volume)
    {
        audioSource.volume = volume;
        Debug.Log($"[PlayerControler] SFX volume updated to: {volume}");
    }
}