using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CubeCredits : MonoBehaviour
{
    public bool move = true;

    public Transform myDestination;
    public float t;
    public float speed;
    public float animationSpeed;

    void Update()
    {
        if (move)
        {
            Move();
        }
    }

    void Move()
    {
        Vector2 a = transform.position;
        Vector2 b = myDestination.position;
        Vector2 desired = b - a;

        if(desired.magnitude > 0.1f)
        {
            transform.position = Vector2.MoveTowards(a, Vector2.Lerp(a, b, t), speed * Time.deltaTime);
        }
    }
}
