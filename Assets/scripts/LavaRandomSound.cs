using UnityEngine;
using UnityEngine.Audio;

public class LavaRandomSound : MonoBehaviour
{
    public AudioMixerGroup[] mixerGroups; // mehrere Mixer Groups
    public AudioClip clip; // der Sound der abgespielt wird

    private AudioSource source;

    public float minDelay = 2f;
    public float maxDelay = 5f;

    void Start()
    {
        source = GetComponent<AudioSource>();
        StartCoroutine(PlayRandom());
    }

    System.Collections.IEnumerator PlayRandom()
    {
        while (true)
        {
            float delay = Random.Range(minDelay, maxDelay);
            yield return new WaitForSeconds(delay);

            // zufällige Mixer Group auswählen
            source.outputAudioMixerGroup = mixerGroups[Random.Range(0, mixerGroups.Length)];

            source.pitch = Random.Range(0.9f, 1.1f);
            source.PlayOneShot(clip);
        }
    }
}