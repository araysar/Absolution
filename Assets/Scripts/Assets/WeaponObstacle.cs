using UnityEngine;

public class WeaponObstacle : IObstacle
{
    [SerializeField] private GameObject destroyEffect;
    [SerializeField] private AudioClip destroySfx;
    public int myNumber;
    public GameObject myDoor;

    public void Enable()
    {
        gameObject.SetActive(true);
        if(myDoor != null) myDoor.SetActive(false);
        destroyEffect.SetActive(false);
    }

    public void Disable()
    {
        destroyEffect.SetActive(false);
        destroyEffect.transform.position = transform.position;
        destroyEffect.SetActive(true);
        SoundManager.instance.PlaySound(SoundManager.SoundChannel.SFX, destroySfx, transform);
        if(myDoor != null) myDoor.SetActive(true);
        gameObject.SetActive(false);
    }

    public override void Trigger(int number)
    {
        if(number == myNumber)
        {
            Disable();
        }
    }
}