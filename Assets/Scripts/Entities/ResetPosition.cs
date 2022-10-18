using System.Collections;
using UnityEngine;

public class ResetPosition : MonoBehaviour
{
    [SerializeField] private Transform moveToPosition;
    [SerializeField] private float damage;
    [SerializeField] private float disableInputsTime = 1;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.GetComponent<IDamageable>() != null)
        {
            if(collision.gameObject.tag == "Player")
            {
                collision.transform.position = moveToPosition.position;
                StartCoroutine(DisableInputs(collision.GetComponent<Character_Movement>()));
                collision.GetComponent<IDamageable>().TakeDamage(damage);
            }
            else
            {
                collision.GetComponent<IDamageable>().TakeDamage(9999);
            }
        }
    }

    private IEnumerator DisableInputs(Character_Movement myChar)
    {
        myChar.StopMovement();
        yield return new WaitForSeconds(disableInputsTime);
        myChar.ResumeMovement();
    }
}
