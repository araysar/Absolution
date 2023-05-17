using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleSoundColission : MonoBehaviour
{
    public AudioClip[] audioClips;
    private AudioSource myAudio;

    private void Start()
    {
        myAudio = GetComponent<AudioSource>();
    }

    private void OnParticleCollision(GameObject other)
    {
        myAudio.PlayOneShot(audioClips[Random.Range(0, audioClips.Length)]);
    }
}
