using System.Collections;
using UnityEngine;

public class Chakram : MonoBehaviour
{
    [HideInInspector] public Action_Shoot myShooter;
    [SerializeField] private Transform characterTransform;
    public GameObject impactEffect;
    public float damage = 5;
    public float speed = 3;
    public float timeToBack = 0.5f;
    public bool isBacking = false;
    private bool isFacingRight = true;

    private void OnEnable()
    {
        transform.position = myShooter.transform.position;
        if(!characterTransform.GetComponent<Character_Movement>().isWallSliding)
        {
            if (characterTransform.rotation.y >= 0) isFacingRight = true;
            else isFacingRight = false;
        }
        else
        {
            if (characterTransform.rotation.y >= 0) isFacingRight = false;
            else isFacingRight = true;
        }
        
        StartCoroutine(Timer());
    }

    private void Update()
    {
        if(!isBacking)
        {
            transform.position = new Vector2(isFacingRight == true? transform.position.x + (speed * Time.deltaTime) : transform.position.x - (speed * Time.deltaTime), transform.position.y);
        }
        else
        {
            transform.position = Vector2.MoveTowards(transform.position, characterTransform.position, speed * Time.deltaTime);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(isBacking)
        {
            if(collision.gameObject.tag == "Player")
            {
                myShooter.isShooting = false;
                isBacking = false;
                StopAllCoroutines();
                gameObject.SetActive(false);
            }
        }
        if(collision.GetComponent<IDamageable>() != null && collision.tag != "Player")
        {
            Impact();
            collision.GetComponent<IDamageable>().TakeDamage(damage);
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (!isBacking)
        {
            if (collision.gameObject.layer == 3) //ground layer
            {
                isBacking = true;
                StartCoroutine(RestartCollision());
            }
        }
    }

    private void Impact()
    {
        if(impactEffect != null) Instantiate(impactEffect, transform.position, Quaternion.identity);
    }

    IEnumerator Timer()
    {
        yield return new WaitForSeconds(timeToBack);
        isBacking = true;
        StartCoroutine(RestartCollision());
    }

    IEnumerator RestartCollision()
    {
        GetComponent<Collider2D>().enabled = false;
        yield return new WaitForEndOfFrame();
        GetComponent<Collider2D>().enabled = true;
    }
}
