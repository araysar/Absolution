using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cube : MonoBehaviour
{
    public bool move = true;
    public GameObject overchargeEffect;
    public Transform myDestination;
    [HideInInspector] public List<Transform> animationPositions = new List<Transform>();
    private int currentPosition = 0;
    public Character_Movement player;
    public float t;
    public float speed;
    public float animationSpeed;
    private bool idleAnimation = true;

    [Header("Astral Ball")]
    public bool astralBallActivated = false;

    public float rotateSpeed; // Qué tan rápido da vueltas en círculos
    public float radius = 3; // Distancia desde el jugador
    public float radiusX = 3;
    public float radiusY = 1.5f;
    public float currentAngle = 0;

    private void Start()
    {
        player = Character_Movement.instance;
        if (player == null) player = FindObjectOfType<Character_Movement>();
        GameManager.instance.DestroyEvent += Destroy;
        GameManager.instance.StopMovementEvent += StopMove;
        GameManager.instance.ResumeMovementEvent += ResumeMove;
        if (player.GetComponent<Character_Attack>().currentAttack.GetComponent<Ball_Attack>() == true) astralBallActivated = true;
        if (player.GetComponent<Character_Attack>().damageUpgrade == true) overchargeEffect.SetActive(true);
    }

    void Update()
    {
        if(move && Time.timeScale > 0)
        {
            if (!astralBallActivated) Move();
            else AstralMove();
        }
    }

    public void StopMove()
    {
        move = false;
    }

    public void ResumeMove()
    { 
        move = true;
    }
    public void EmpoweredCube(bool isActivated)
    {
        overchargeEffect.SetActive(isActivated);
    }

    private void Destroy()
    {
        Destroy(gameObject);
    }

    private void OnDestroy()
    {
        GameManager.instance.DestroyEvent -= Destroy;
    }

    void AstralMove()
    {
        // 1. Aumentamos el ángulo constantemente
        currentAngle += rotateSpeed * Time.deltaTime;

        // 2. Calculamos el desplazamiento (Offset) desde el centro
        // Usamos Cos para X y Sin para Y para crear el círculo/óvalo
        float x = Mathf.Cos(currentAngle) * radiusX;
        float y = Mathf.Sin(currentAngle) * radiusY;

        // 3. ASIGNACIÓN DIRECTA (World Space)
        // Posición final = Posición del Jugador + El desplazamiento calculado
        transform.position = player.transform.position + new Vector3(x, y, 0);

        // 4. Resetear rotación (opcional, si el sprite gira y no quieres que lo haga)
        transform.rotation = Quaternion.identity;
    }

    void Move()
    {
        Vector2 a = transform.position;
        Vector2 b = myDestination.position;
        Vector2 desired = b - a;
        if (idleAnimation)
        {
            if(player.rb.velocity.magnitude == 0)
            {
                transform.position = Vector2.MoveTowards(a, animationPositions[currentPosition].position, animationSpeed);
                if ((animationPositions[currentPosition].position - transform.position).magnitude < 0.05f)
                {
                    currentPosition++;
                    if (currentPosition >= animationPositions.Count) currentPosition = 0;
                }
            }
            else
            {
                idleAnimation = false;
            }
        }
        else
        {
            if (desired.magnitude < 0.05f)
            {
                idleAnimation = true;
                currentPosition = 0;
            }
            else
            {
                transform.position = Vector2.MoveTowards(a, Vector2.Lerp(a, b, t), speed);
            }
        }
    }
}
