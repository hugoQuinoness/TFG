using System.Diagnostics;
using UnityEngine;

[RequireComponent(typeof(Animator), typeof(Rigidbody2D))]
[DebuggerDisplay("{" + nameof(GetDebuggerDisplay) + "(),nq}")]
public class PlayerControler : MonoBehaviour
{
    public static PlayerControler instance;

    [Header("Movement Settings")]
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float runSpeed = 8f; // Nueva variable para la velocidad de correr
    [SerializeField] private float directionSmoothing = 10f;

    [Header("Attack Settings")]
    [SerializeField] private GameObject hitArea; // Referencia al prefab HitArea
    [SerializeField] private Transform hitAreaSpawnPoint; // Punto de aparición del HitArea
    [SerializeField] private float hitAreaVerticalOffset = 0.5f; // Desplazamiento vertical
    [SerializeField] private float hitAreaHorizontalOffset = 0.5f; // Desplazamiento horizontal

    private Animator _animator;
    private Rigidbody2D _rb;
    private Vector2 _targetDirection;
    private Vector2 _currentDirection;
    private bool _isRunning; // Nueva variable para verificar si está corriendo

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else if (instance!=this)
        {
            Destroy(gameObject);
        }
        
        _animator = GetComponent<Animator>();
        _rb = GetComponent<Rigidbody2D>();
        _rb.gravityScale = 0;

    }

    void Update()
    {
        GetInputDirection();
        UpdateCurrentDirection();
        UpdateAnimator();
        UpdateHitAreaSpawnPoint(); // Actualizar la posición del hitAreaSpawnPoint
        HandleAttack();
    }

    void FixedUpdate()
    {
        MoveCharacter();
    }

    private void GetInputDirection()
    {
        _targetDirection = Vector2.zero;
        _isRunning = Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift); // Verificar si Shift está presionado

        // Mapeo directo de teclas a direcciones absolutas
        if (Input.GetKey(KeyCode.W)) _targetDirection += Vector2.up;      // Arriba
        if (Input.GetKey(KeyCode.S)) _targetDirection += Vector2.down;    // Abajo
        if (Input.GetKey(KeyCode.A)) _targetDirection += Vector2.left;    // Izquierda
        if (Input.GetKey(KeyCode.D)) _targetDirection += Vector2.right;   // Derecha

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
        // Usar la velocidad de correr si Shift está presionado, de lo contrario usar la velocidad normal
        float speed = _isRunning ? runSpeed : moveSpeed;
        _rb.velocity = _targetDirection * speed;
    }

    private void UpdateAnimator()
    {
        _animator.SetFloat("Horizontal", _currentDirection.x);
        _animator.SetFloat("Vertical", _currentDirection.y);
        _animator.SetBool("Walk", _targetDirection.magnitude > 0.1f);
        _animator.SetBool("Run", _isRunning); // Actualizar la animación de correr
        if (_rb.velocity.x == 0 && _rb.velocity.y == 0)
        {
            _animator.SetTrigger("Standing");
        }
    }

    private void UpdateHitAreaSpawnPoint()
    {
        // Actualizar la posición del hitAreaSpawnPoint delante del personaje con desplazamientos
        if (_currentDirection.magnitude > 0)
        {
            Vector2 offset = _currentDirection.normalized * hitAreaHorizontalOffset;
            offset.y += hitAreaVerticalOffset;
            hitAreaSpawnPoint.position = (Vector2)transform.position + offset;
        }
    }

    private void HandleAttack()
    {
        if (Input.GetMouseButtonDown(0)) // Detectar clic del botón izquierdo del mouse
        {
            // Instanciar el objeto HitArea en el punto de aparición
            GameObject instantiatedHitArea = Instantiate(hitArea, hitAreaSpawnPoint.position, hitAreaSpawnPoint.rotation);

            // Destruir el objeto HitArea después de 0.5 segundos
            Destroy(instantiatedHitArea, 0.4f);

            // Reproducir la animación de ataque
            _animator.SetTrigger("Attack");
        }
    }

    private string GetDebuggerDisplay()
    {
        return ToString();
    }

}


