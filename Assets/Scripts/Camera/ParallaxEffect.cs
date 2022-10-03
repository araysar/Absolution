using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParallaxEffect : MonoBehaviour
{
    [SerializeField, Tooltip("entre 0.1 y 1")] private float parallaxMultiplier = 0.25f;

    private Transform cameraTransform;
    private Vector3 previousCameraPosition;
    private float spriteWidth, startPosition;

    void Start()
    {
        cameraTransform = Camera.main.transform;
        previousCameraPosition = cameraTransform.position;
        spriteWidth = GetComponent<SpriteRenderer>().bounds.size.x;
        startPosition = transform.position.x;
    }

    private void LateUpdate()
    {
        float deltaX = (cameraTransform.position.x - previousCameraPosition.x) * parallaxMultiplier;
        float moveAmount = cameraTransform.position.x * (1 - parallaxMultiplier);
        transform.Translate(new Vector2(deltaX, 0));
        previousCameraPosition = cameraTransform.position;
    }
}
