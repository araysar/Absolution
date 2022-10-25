using UnityEngine;
using UnityEngine.UIElements;

public class Transition : MonoBehaviour
{
    [SerializeField] private int nextScene = 0;
    [SerializeField] private Transform particlePosition;
    [SerializeField] private GameObject particleScreamEffect;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.tag == "Player")
        {
            TriggerTransition();
        }
    }

    public void TriggerTransition()
    {
        GameManager.instance.nextScene = nextScene;
        SoundManager.instance.PlaySound(SoundManager.SoundChannel.Music, null);
        GameManager.instance.TransitionEvent += ScreamEffect;
        GameManager.instance.Transition(GameManager.EventType.FadeTransition, 0);
    }

    public void ScreamEffect()
    {
        Instantiate(particleScreamEffect, particlePosition.position, Quaternion.identity);
    }
}
