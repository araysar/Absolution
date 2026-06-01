using System.Collections;
using UnityEngine;

public class UnstableArea : MonoBehaviour
{
    bool triggered = false;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Character_Attack myPlayer = collision.GetComponent<Character_Attack>();
        if(myPlayer != null && !Character_Movement.instance.isBusy)
        {
            if(!triggered)
            {
                triggered = true;
                StartCoroutine(FreezeWeapon(myPlayer));
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
