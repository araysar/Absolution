using UnityEngine;
using System.Collections.Generic;

[RequireComponent(typeof(Rigidbody2D))]
public class Enemy_Bat : MonoBehaviour
{
    [Header("Patrulla")]
    public List<Vector2> waypoints; //
    public float arrivalThreshold = 0.5f;
    private int _currentWaypointIndex = 0;

    [Header("Detección")]
    public float detectionRadius = 5f; //
    public LayerMask playerLayer; //
    private Transform _playerTransform;
    public GameObject rageEffect;
    public AudioSource rageSfx;
    bool isRaging = false;

    [Header("Steering (Boids)")]
    public float maxSpeed = 6f; //
    public float maxForce = 15f; //

    [Header("Evasión de Obstáculos")]
    public float wallAvoidanceDistance = 1f; //
    public float avoidanceForce = 10f; //
    public float raycastSeparation = 0.4f; // La distancia del centro a los rayos superior/inferior
    public LayerMask wallLayer; //

    [Header("Combate")]
    public int damage = 1;
    public Vector2 damageBoxSize = new Vector2(0.8f, 0.8f); // Tamaño del área de daño
    public float attackCooldown = 1f; // Tiempo entre golpes si se queda pegado
    private float _lastAttackTime;
    public AudioClip hitAudioClip;

    private Rigidbody2D _rb;
    [HideInInspector] public Enemy_Health myHealth;
    private bool _isChasing = false;
    private Vector2 _targetPosition;

    void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
        myHealth = GetComponentInChildren<Enemy_Health>();
        damageBoxSize = GetComponent<BoxCollider2D>().size * 0.25f;
        rageSfx = GetComponent<AudioSource>();
        rageSfx.volume = SoundManager.instance.sfxVolume;
        SoundManager.instance.audioSources.Add(rageSfx);
        _rb.gravityScale = 0; // Evita que caiga por gravedad
    }

    void Update()
    {
        if (myHealth.currentHP <= 0) return;

        VerificarAtaqueAlJugador();

        // Escaneo del jugador
        Collider2D playerHit = Physics2D.OverlapCircle(transform.position, detectionRadius, playerLayer);

        if (playerHit != null)
        {
            _isChasing = true;
            myHealth.myRenderer.color = Color.red;
            rageEffect.SetActive(true);
            if(!isRaging)
            {
                if(!rageSfx.isPlaying) rageSfx.Play();
                isRaging = true;

            }
            _playerTransform = playerHit.transform;

        }
        else
        {
            isRaging = false;
            rageEffect.SetActive(false);
            myHealth.myRenderer.color = Color.white;
            _isChasing = false;
        }

        ActualizarOrientacion();
    }

    void FixedUpdate()
    {
        if (myHealth.currentHP <= 0) return;

        // 1. Determinar el punto hacia donde queremos ir
        if (_isChasing)
        {
            _targetPosition = _playerTransform.position;
        }
        else if (waypoints != null && waypoints.Count > 0)
        {
            _targetPosition = waypoints[_currentWaypointIndex];

            if (Vector2.Distance(transform.position, _targetPosition) < arrivalThreshold)
            {
                _currentWaypointIndex = (_currentWaypointIndex + 1) % waypoints.Count;
            }
        }

        // 2. Calcular Fuerzas de Steering
        Vector2 steerSeek = CalcularSeek(_targetPosition);
        Vector2 steerAvoid = CalcularEvadirObstaculos();

        // 3. Aplicación equilibrada (La evasión tiene prioridad sobre el avance)
        Vector2 totalSteering = steerSeek + (steerAvoid * 1.5f);

        totalSteering = Vector2.ClampMagnitude(totalSteering, maxForce);
        _rb.AddForce(totalSteering);

        // Limitar velocidad máxima física
        _rb.velocity = Vector2.ClampMagnitude(_rb.velocity, maxSpeed);
    }

    Vector2 CalcularSeek(Vector2 target)
    {
        Vector2 desiredVelocity = (target - (Vector2)transform.position).normalized * maxSpeed;
        return desiredVelocity - _rb.velocity;
    }

    Vector2 CalcularEvadirObstaculos()
    {
        // 1. Determinar la dirección hacia donde miramos
        Vector2 lookAheadDir = _rb.velocity.magnitude > 0.1f ? _rb.velocity.normalized : (_targetPosition - (Vector2)transform.position).normalized;

        // 2. Calcular un vector perpendicular para los bigotes laterales
        // Si la dirección es (x, y), la perpendicular es (-y, x)
        Vector2 perpendicularDir = new Vector2(-lookAheadDir.y, lookAheadDir.x);

        // 3. Calcular los puntos de origen de los dos rayos
        Vector2 originTop = (Vector2)transform.position + perpendicularDir * raycastSeparation;
        Vector2 originBottom = (Vector2)transform.position - perpendicularDir * raycastSeparation;

        Vector2 totalAvoidanceForce = Vector2.zero;

        // --- RAYO SUPERIOR ---
        RaycastHit2D hitTop = Physics2D.Raycast(originTop, lookAheadDir, wallAvoidanceDistance, wallLayer);
        if (hitTop.collider != null)
        {
            Debug.DrawRay(originTop, lookAheadDir * wallAvoidanceDistance, Color.red);
            totalAvoidanceForce += hitTop.normal * avoidanceForce;
        }
        else
        {
            Debug.DrawRay(originTop, lookAheadDir * wallAvoidanceDistance, Color.green);
        }

        // --- RAYO INFERIOR ---
        RaycastHit2D hitBottom = Physics2D.Raycast(originBottom, lookAheadDir, wallAvoidanceDistance, wallLayer);
        if (hitBottom.collider != null)
        {
            Debug.DrawRay(originBottom, lookAheadDir * wallAvoidanceDistance, Color.red);
            // Sumamos la fuerza. Si ambos rayos golpean, el empuje será doble.
            totalAvoidanceForce += hitBottom.normal * avoidanceForce;
        }
        else
        {
            Debug.DrawRay(originBottom, lookAheadDir * wallAvoidanceDistance, Color.green);
        }

        return totalAvoidanceForce;
    }

    void ActualizarOrientacion()
    {
        if (_rb.velocity.x > 0.1f) transform.localScale = new Vector3(0.25f, 0.25f, 1);
        else if (_rb.velocity.x < -0.1f) transform.localScale = new Vector3(-0.25f, 0.25f, 1);
    }

    void VerificarAtaqueAlJugador()
    {
        // Si todavía estamos en tiempo de espera, no hacemos nada
        if (Time.time < _lastAttackTime + attackCooldown) return;

        // "Pintamos" una caja invisible en la posición del murciélago
        // angle: 0 porque la caja de daño no suele rotar aunque el sprite sí
        Collider2D playerHit = Physics2D.OverlapBox(transform.position, damageBoxSize, 0, playerLayer);

        if (playerHit != null)
        {
            // INTENTA OBTENER EL SCRIPT DE VIDA DEL JUGADOR
            // Asegúrate de que tu player tenga un script tipo "PlayerHealth"
            Player_Health healthScript = playerHit.GetComponent<Player_Health>();

            if (healthScript != null)
            {
                healthScript.TakeDamage(damage);
                if (hitAudioClip != null) SoundManager.instance.PlaySound(SoundManager.SoundChannel.SFX,hitAudioClip, transform);
                _lastAttackTime = Time.time; // Reseteamos el reloj
            }
        }
    }
    private void OnDrawGizmos()
    {
        // Visualizar Radio de Detección
        Gizmos.color = new Color(0, 1, 1, 0.2f);
        Gizmos.DrawWireSphere(transform.position, detectionRadius);

        // Visualizar Ruta de Patrulla
        if (waypoints != null && waypoints.Count > 0)
        {
            Gizmos.color = Color.yellow;
            for (int i = 0; i < waypoints.Count; i++)
            {
                Gizmos.DrawSphere(waypoints[i], 0.2f);
                int next = (i + 1) % waypoints.Count;
                Gizmos.DrawLine(waypoints[i], waypoints[next]);
            }
        }

        // Usamos un color diferente para distinguirlos
        Gizmos.color = Color.magenta;

        // Calculamos la dirección tentativa (asumimos que mira a la derecha si está quieto)
        Vector2 lookAheadDir = Vector2.right;
        if (Application.isPlaying && _rb != null && _rb.velocity.magnitude > 0.1f)
        {
            lookAheadDir = _rb.velocity.normalized;
        }

        Vector2 perpendicularDir = new Vector2(-lookAheadDir.y, lookAheadDir.x);
        Vector2 originTop = (Vector2)transform.position + perpendicularDir * raycastSeparation;
        Vector2 originBottom = (Vector2)transform.position - perpendicularDir * raycastSeparation;

        // Dibujamos las líneas de los rayos
        Gizmos.DrawLine(originTop, originTop + lookAheadDir * wallAvoidanceDistance);
        Gizmos.DrawLine(originBottom, originBottom + lookAheadDir * wallAvoidanceDistance);

        // Dibujamos una pequeña esfera en el origen para ver la separación exacta
        Gizmos.DrawSphere(originTop, 0.05f);
        Gizmos.DrawSphere(originBottom, 0.05f);

        // Visualizar CAJA DE DAÑO (Hurtbox)
        Gizmos.color = new Color(1, 0, 0, 0.4f); // Rojo semitransparente
        Gizmos.DrawCube(transform.position, damageBoxSize);
    }
}
