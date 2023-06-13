using System.Collections;
using UnityEngine;

public class BlackFog : MonoBehaviour
{
    SpriteRenderer myFog;
    public float timeToDisappear = 2;
    private bool triggered = false;
    private void Start()
    {
        myFog = GetComponent<SpriteRenderer>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(!triggered)
        {
            if (collision.gameObject == GameManager.instance.player.gameObject)
            {
                triggered = true;
                StartCoroutine(Transition());
            }
        }
    }

    IEnumerator Transition()
    {
        float count = 0;
        for (float i = 0; count < 1; i += Time.deltaTime)
        {
            count = i / timeToDisappear;
            myFog.color = new Color(0, 0, 0, 1 - count);
            yield return new WaitForEndOfFrame();
        }
        myFog.color = new Color(0, 0, 0, 0);
        yield return null;
    }
}
