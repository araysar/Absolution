using UnityEngine;

public class WeaponObstacle : MonoBehaviour
{

    [SerializeField] private GameObject destroyEffect;
    [SerializeField] private AudioClip destroySfx;
    [SerializeField] private DamageStay myDamage;
    private void Start()
    {
        GameManager.instance.EnemyRespawnEvent += Enable;
    }

    private void Enable()
    {
        gameObject.SetActive(true);
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {

    }
    public void Destroy()
    {
        if (destroyEffect != null) Instantiate(destroyEffect, transform.position, Quaternion.identity);
        if (destroySfx != null) SoundManager.instance.PlaySound(SoundManager.SoundChannel.SFX, destroySfx, transform);

        gameObject.SetActive(false);
    }
}