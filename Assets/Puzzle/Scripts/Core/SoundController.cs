using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class SoundController : MonoBehaviour
{

    static SoundController _instance;
    public static SoundController Instance
    {
        get { return _instance; }
    }

    AudioSource _audioSource;
    AudioSource audioSource
    {
        get
        {
            if (_audioSource == null)
                _audioSource = GetComponent<AudioSource>();

            return _audioSource;
        }
    }

    void Awake()
    {
        _instance = this;
    }

    public void Play(AudioClip sound, bool loop = false, float pitch = 1f)
    {
        audioSource.pitch = pitch;
        audioSource.loop = loop;
        audioSource.clip = sound;
        audioSource.Play();
    }

    public void Stop()
    {
        audioSource.Stop();
    }

    public void PlaySound(AudioClip sound, float pitch = 1)
    {
        audioSource.pitch = pitch;
        audioSource.PlayOneShot(sound);
    }
}
