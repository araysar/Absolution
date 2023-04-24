using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cube : MonoBehaviour
{
    public bool move = true;
    public Transform myDestination;
    [HideInInspector] public List<Transform> animationPositions = new List<Transform>();
    private int currentPosition = 0;
    public Character_Movement player;
    public float t;
    public float speed;
    public float animationSpeed;
    private bool idleAnimation = false;

    private void Start()
    {
        if (player == null) player = FindObjectOfType<Character_Movement>();
    }
    void FixedUpdate()
    {
        if(move)
        {
            Move();
        }
    }

    void Move()
    {
        Vector2 a = transform.position;
        Vector2 b = myDestination.position;
        Vector2 desired = b - a;
        if (idleAnimation)
        {
            if(!player.isMoving)
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
            if (desired.magnitude < 0.1f && !player.isMoving)
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
