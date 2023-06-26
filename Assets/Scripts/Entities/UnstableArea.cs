using System.Collections;
using UnityEngine;

public class UnstableArea : MonoBehaviour
{
    bool triggered = false;
    public int myArea;

    private void Start()
    {
        if (Character_Movement.instance.unstableAreaCleared.Contains(myArea)) gameObject.SetActive(false);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Character_Attack player = collision.GetComponent<Character_Attack>();
        if(collision.GetComponent<Character_Attack>() != null)
        {
            if(!triggered)
            {
                triggered = true;
                StartCoroutine(FreezeWeapon(player));
            }
        }
    }

    IEnumerator FreezeWeapon(Character_Attack player)
    {
        while(Character_Movement.instance.disableInputs)
        {
            yield return new WaitForEndOfFrame();
        }
        triggered = false;
        player.FreezeWeapon();
    }
}
