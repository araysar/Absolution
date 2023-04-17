using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cube : MonoBehaviour
{
    public bool arrive = true;
    public Transform myDestination;
    public float t;
    public float speed;

    void Start()
    {
        
    }

    void FixedUpdate()
    {
        if(arrive) Arrive();
    }

    void Arrive()
    {
        Vector2 a = transform.position;
        Vector2 b = myDestination.position;
        Vector2 desired = b - a;

        transform.position = Vector2.MoveTowards(a, Vector2.Lerp(a, b, t), speed);

    }
}
