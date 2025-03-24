using UnityEngine;

[RequireComponent(typeof(Animator), typeof(Rigidbody2D))]
public class IsoMovementCardinalCorrect : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float directionSmoothing = 10f;

    // Direcciones cardinales en espacio isométrico
    private readonly Vector2 _isoUp = new Vector2(-1, 1).normalized;
    private readonly Vector2 _isoDown = new Vector2(1, -1).normalized;
    private readonly Vector2 _isoLeft = new Vector2(-1, -1).normalized;
    private readonly Vector2 _isoRight = new Vector2(1, 1).normalized;

    private Animator _animator;
    private Rigidbody2D _rb;
    private Vector2 _targetDirection;
    private Vector2 _currentDirection;

    void Awake()
    {
        _animator = GetComponent<Animator>();
        _rb = GetComponent<Rigidbody2D>();
        _rb.gravityScale = 0;
    }

    void Update()
    {
        GetInputDirection();
        UpdateCurrentDirection();
        UpdateAnimator();
    }

    void FixedUpdate()
    {
        MoveCharacter();
    }

    private void GetInputDirection()
    {
        _targetDirection = Vector2.zero;

        // Mapeo directo de teclas a direcciones isométricas
        if (Input.GetKey(KeyCode.W)) _targetDirection += _isoUp;      // Arriba (isométrico)
        if (Input.GetKey(KeyCode.S)) _targetDirection += _isoDown;    // Abajo (isométrico)
        if (Input.GetKey(KeyCode.A)) _targetDirection += _isoLeft;    // Izquierda (isométrico)
        if (Input.GetKey(KeyCode.D)) _targetDirection += _isoRight;   // Derecha (isométrico)

        // Normalizar solo si hay input
        if (_targetDirection.magnitude > 0)
        {
            _targetDirection.Normalize();
        }
    }

    private void UpdateCurrentDirection()
    {
        // Suavizado de dirección para animaciones
        _currentDirection = Vector2.Lerp(
            _currentDirection,
            _targetDirection.magnitude > 0 ? _targetDirection : _currentDirection,
            directionSmoothing * Time.deltaTime
        );
    }

    private void MoveCharacter()
    {
        // Movimiento directo sin conversión adicional
        _rb.velocity = _targetDirection * moveSpeed;
    }

    private void UpdateAnimator()
    {
        // Mapeo inverso para animaciones (de isométrico a cartesiano)
        Vector2 animDirection = Vector2.zero;

        if (_targetDirection.magnitude > 0.1f)
        {
            // Convertir dirección isométrica a coordenadas de animación
            animDirection.x = _targetDirection.x + _targetDirection.y;
            animDirection.y = _targetDirection.y - _targetDirection.x;
            animDirection.Normalize();
        }

        _animator.SetFloat("Horizontal", _currentDirection.x);
        _animator.SetFloat("Vertical", _currentDirection.y);
        _animator.SetBool("Walk", _targetDirection.magnitude > 0.1f);
    }
}