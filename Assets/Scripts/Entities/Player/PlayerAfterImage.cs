using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAfterImage : MonoBehaviour
{
    [SerializeField] private float activeTime = 0.1f;


    [HideInInspector] public PlayerAfterImagePool imagePool;
    private Transform player;
    private SpriteRenderer sR;

    private void OnEnable()
    {
        sR = GetComponent<SpriteRenderer>();
        if (player == null) player = GameObject.FindGameObjectWithTag("Player").transform;

        sR.sprite = player.GetComponent<SpriteRenderer>().sprite;
        transform.position = player.transform.position;
        transform.rotation = player.transform.rotation;
        transform.localScale = player.transform.localScale;
        StartCoroutine(Timer());
    }

    IEnumerator Timer()
    {
        sR.material.SetFloat("_DeathValue", 0);
        for (float i = 0; i < activeTime; i += Time.deltaTime)
        {
            sR.material.SetFloat("_DeathValue", i  / activeTime);
            yield return null;
        }
        imagePool.AddToPool(gameObject);
    }
}
