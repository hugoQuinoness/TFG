using System.Diagnostics;
using UnityEngine;
using UnityEngine.EventSystems;

[RequireComponent(typeof(Animator), typeof(Rigidbody2D), typeof(BoxCollider2D))]
public class PlayerControler : MonoBehaviour
{
    public static PlayerControler instance;

    [Header("Movement Settings")]
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float runSpeed = 8f;
    [SerializeField] private float directionSmoothing = 10f;

    [Header("Attack Settings")]
    [SerializeField] private GameObject hitArea;
    [SerializeField] private Transform hitAreaSpawnPoint;
    [SerializeField] private float hitAreaVerticalOffset = 0.5f;
    [SerializeField] private float hitAreaHorizontalOffset = 0.5f;

    private Animator _animator;
    private Rigidbody2D rb;
    private Vector2 targetDirection;
    private Vector2 _currentDirection;
    private bool _isRunning;


    [Header("References")]
    private SpriteRenderer spriteRenderer;
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

        _animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        GetInputDirection();
        UpdateCurrentDirection();
        UpdateAnimator();
        UpdateHitAreaSpawnPoint();
        HandleAttack();
    }

    void FixedUpdate()
    {
        MoveCharacter();
    }

    private void GetInputDirection()
    {
        targetDirection = Vector2.zero;
        _isRunning = Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift);

        if (Input.GetKey(KeyCode.W)) targetDirection += Vector2.up;
        if (Input.GetKey(KeyCode.S)) targetDirection += Vector2.down;
        if (Input.GetKey(KeyCode.A)) targetDirection += Vector2.left;
        if (Input.GetKey(KeyCode.D)) targetDirection += Vector2.right;

        if (targetDirection.magnitude > 0)
        {
            targetDirection.Normalize();
        }
    }

    private void UpdateCurrentDirection()
    {
        _currentDirection = Vector2.Lerp(_currentDirection, targetDirection.magnitude > 0 ? targetDirection : _currentDirection, directionSmoothing * Time.deltaTime);
    }

    private void MoveCharacter()
    {
        if (targetDirection.magnitude > 0)
        {
            float speed = _isRunning ? runSpeed : moveSpeed;
            
            Vector2 desiredPosition = (Vector2)transform.position + targetDirection * speed * Time.fixedDeltaTime;

            rb.MovePosition(desiredPosition);
        }
        else
        {
            rb.velocity = Vector2.zero;
        }

        if (targetDirection.x > 0)
        {
            spriteRenderer.flipX = true; 
        }
        else if (targetDirection.x < 0)
        {
            spriteRenderer.flipX = false; 
        }
    }

    private void UpdateAnimator()
    {
        _animator.SetFloat("Horizontal", _currentDirection.x);
        _animator.SetFloat("Vertical", _currentDirection.y);
        _animator.SetBool("Walk", targetDirection.magnitude > 0.1f);
        _animator.SetBool("Run", _isRunning);

        if (rb.velocity.x == 0 && rb.velocity.y == 0)
        {
            _animator.SetTrigger("Standing");
        }
    }

    private void UpdateHitAreaSpawnPoint()
    {
        if (_currentDirection.magnitude > 0)
        {
            Vector2 offset = _currentDirection.normalized * hitAreaHorizontalOffset;
            offset.y += hitAreaVerticalOffset;
            hitAreaSpawnPoint.position = (Vector2)transform.position + offset;
        }
    }

    private void HandleAttack()
    {
        if (Input.GetMouseButtonDown(0))
        {
            GameObject instantiatedHitArea = Instantiate(hitArea, hitAreaSpawnPoint.position, hitAreaSpawnPoint.rotation);
            Destroy(instantiatedHitArea, 0.4f);
            _animator.SetTrigger("Attack");
        }
    }
}