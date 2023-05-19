using UnityEngine;

public class ParticleSoundColission : MonoBehaviour
{
    public AudioClip[] audioClips;

    private void OnParticleCollision(GameObject other)
    {
        SoundManager.instance.PlaySound(SoundManager.SoundChannel.SFX, audioClips[Random.Range(0, audioClips.Length)], transform);
    }
}
