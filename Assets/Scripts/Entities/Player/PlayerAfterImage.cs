using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAfterImage : MonoBehaviour
{
    [SerializeField] private float activeTime = 0.1f;
    private float timeActivated;
    private float alpha;
    [SerializeField] private float alphaSet = 0.8f;
    private float alphaMultiplier = 0.85f;

    [HideInInspector] public PlayerAfterImagePool imagePool;
    private Transform player;
    private SpriteRenderer sR;
    private SpriteRenderer playerSR;

    public Color color;

    private void OnEnable()
    {
        sR = GetComponent<SpriteRenderer>();
        if (player == null) player = GameObject.FindGameObjectWithTag("Player").transform;
        playerSR = player.GetComponent<SpriteRenderer>();

        alpha = alphaSet;
        sR.sprite = playerSR.sprite;
        transform.position = player.transform.position;
        transform.rotation = player.transform.rotation;
        transform.localScale = player.transform.localScale;
        timeActivated = Time.time;
    }

    private void Update()
    {
        alpha = alphaMultiplier;
        color.a = alpha;
        sR.color = color;

        if(Time.time >= (timeActivated + activeTime))
        {
            imagePool.AddToPool(gameObject);
        }
    }
}
