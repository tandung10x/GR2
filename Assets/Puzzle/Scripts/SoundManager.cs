using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public enum AudioType
{
    SOUNDS,
    MUSIC
}

public class SoundManager : MonoBehaviour
{

	public delegate void OnValueChange();

	public static OnValueChange OnSoundsChange;
	public static OnValueChange OnMusicChange;

	public AudioMixerGroup sfxOutput;

	public float timeToReachSnapshot = 0.5f;
	public AudioMixerSnapshot sfxDefault;
	public AudioMixerSnapshot sfxMuted;
	public AudioMixerSnapshot musicDefault;
	public AudioMixerSnapshot musicMuted;

	public AudioSource musicSource;
	public AudioClip music;

	static SoundManager _instance;
	public static SoundManager Instance
	{
		get { return _instance; }
	}

	List<AudioSource> sources = new List<AudioSource>();

	public void PlaySfx(AudioClip clip, float pitch = 1)
	{
		AudioSource source = GetFreeSource();
		source.pitch = pitch;
		source.clip = clip;
		source.Play();
	}

	public static void PlayMusic(AudioClip clip, float pitch = 1)
	{
		AudioSource source = _instance.musicSource;
		source.pitch = pitch;
		source.clip = clip;
		source.Play();
	}

	public void StopMusic()
	{
		_instance.musicSource.Stop();
	}

	void Awake()
	{
		_instance = this;
	}

	void Start()
	{
		AudioMixerSnapshot sfx = (SfxOn ? sfxDefault : sfxMuted);
		sfx.TransitionTo(timeToReachSnapshot);

		AudioMixerSnapshot snapshot = (MusicOn ? musicDefault : musicMuted);
		snapshot.TransitionTo(timeToReachSnapshot);

		PlayMusic(music, 1);
	}

	void CreateSources(int count)
	{
		for (int i = 0; i < count; i++)
		{
			AudioSource source = gameObject.AddComponent<AudioSource>();
			source.outputAudioMixerGroup = sfxOutput;
			source.playOnAwake = false;
			sources.Add(source);
		}
	}

	AudioSource GetFreeSource()
	{
		foreach (AudioSource source in sources)
		{
			if (!source.isPlaying)
				return source;
		}

		CreateSources(1);
		return sources[sources.Count - 1];
	}

	public bool SfxOn
	{
		get { return PlayerPrefs.GetInt("SfxOn", 1) == 1; }
		private set
		{
			AudioMixerSnapshot snapshot = (value ? sfxDefault : sfxMuted);
			snapshot.TransitionTo(timeToReachSnapshot);
			PlayerPrefs.SetInt("SfxOn", value ? 1 : 0);

			if (OnSoundsChange != null)
			{
				OnSoundsChange();
			}
		}
	}

	public bool MusicOn
	{
		get { return PlayerPrefs.GetInt("MusicOn", 1) == 1; }
		private set
		{
			AudioMixerSnapshot snapshot = (value ? musicDefault : musicMuted);
			snapshot.TransitionTo(timeToReachSnapshot);
			PlayerPrefs.SetInt("MusicOn", value ? 1 : 0);

			if (OnMusicChange != null)
			{
				OnMusicChange();
			}
		}
	}

	public void MuteAudio(AudioType type)
	{
		switch (type)
		{
			case AudioType.SOUNDS:
				SfxOn = !SfxOn;
				break;
			case AudioType.MUSIC:
				MusicOn = !MusicOn;
				break;
		}
	}
}