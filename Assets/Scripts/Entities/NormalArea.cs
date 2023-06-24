using UnityEngine;

public class NormalArea : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        Character_Attack player = collision.GetComponent<Character_Attack>();
        if (collision.GetComponent<Character_Attack>() != null)
        {
            if (player.weaponFrozen)
            {
                player.UnFreezeWeapon();
            }
        }
    }

}
