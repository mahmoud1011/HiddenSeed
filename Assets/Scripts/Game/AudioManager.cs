using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class AudioManager : PersistentSingleton<AudioManager>
{
    [SerializeField] private AudioClip clipFlip, clipMatch, clipMismatch, clipGameOver;

    AudioSource audioSource;

    void Start() => audioSource = GetComponent<AudioSource>();

    public void PlayFlip() => PlayOneShot(clipFlip);
    public void PlayMatch() => PlayOneShot(clipMatch);
    public void PlayMismatch() => PlayOneShot(clipMismatch);
    public void PlayGameOver() => PlayOneShot(clipGameOver);

    private void PlayOneShot(AudioClip c)
    {
        if (c == null)
        {
            Debug.LogWarning("Audio clip is null");
            return;
        }
        audioSource.PlayOneShot(c);
    }
}