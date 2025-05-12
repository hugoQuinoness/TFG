using System.Diagnostics;
using UnityEngine;

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
    private Rigidbody2D _rb;
    private BoxCollider2D _collider;
    private Vector2 _targetDirection;
    private Vector2 _currentDirection;
    private bool _isRunning;
    private Vector2 _lastSafePosition; // Para guardar posición sin colisión

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
        _rb = GetComponent<Rigidbody2D>();
        _collider = GetComponent<BoxCollider2D>();

        // Configuración del Rigidbody para colisiones
        _rb.gravityScale = 0;
        _rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
        _rb.freezeRotation = true;

        _lastSafePosition = transform.position;
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
        _targetDirection = Vector2.zero;
        _isRunning = Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift);

        if (Input.GetKey(KeyCode.W)) _targetDirection += Vector2.up;
        if (Input.GetKey(KeyCode.S)) _targetDirection += Vector2.down;
        if (Input.GetKey(KeyCode.A)) _targetDirection += Vector2.left;
        if (Input.GetKey(KeyCode.D)) _targetDirection += Vector2.right;

        if (_targetDirection.magnitude > 0)
        {
            _targetDirection.Normalize();
        }
    }

    private void UpdateCurrentDirection()
    {
        _currentDirection = Vector2.Lerp(_currentDirection, _targetDirection.magnitude > 0 ? _targetDirection : _currentDirection, directionSmoothing * Time.deltaTime);
    }

    private void MoveCharacter()
    {
        if (_targetDirection.magnitude > 0)
        {
            float speed = _isRunning ? runSpeed : moveSpeed;
            
            Vector2 desiredPosition = (Vector2)transform.position + _targetDirection * speed * Time.fixedDeltaTime;

            _rb.MovePosition(desiredPosition);
        }
        else
        {
            _rb.velocity = Vector2.zero;
        }
    }

    private void UpdateAnimator()
    {
        _animator.SetFloat("Horizontal", _currentDirection.x);
        _animator.SetFloat("Vertical", _currentDirection.y);
        _animator.SetBool("Walk", _targetDirection.magnitude > 0.1f);
        _animator.SetBool("Run", _isRunning);

        if (_rb.velocity.x == 0 && _rb.velocity.y == 0)
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