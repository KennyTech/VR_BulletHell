using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour {

    public enum Sound {
        PlayerLaser,
        PlayerHit,
        EnemyFire1,
        EnemyFire2,
        EnemyFire3,
        EnemyDeath,
        Woosh,
        HologramStart,
        HologramEnd,
        Text,
        Count,
        WooshLow
    }

    public enum Music {
        MainTheme,
        Count
    }

    public enum Priority
    {
        Music = 0,
        Low = 64,
        Default = 128,
        High = 192,
        Spam = 256
    }

    [SerializeField]
    int numSources;

    [SerializeField]
    List<AudioClip> clips;

    [SerializeField]
    List<AudioClip> tracks;

    [SerializeField]
    List<AudioClip> trackIntros;

    [SerializeField]
    List<AudioSource> loopingChannels;

    [SerializeField]
    List<Transform> loopingSourcePositions;

    AudioSource[] sources;

    AudioSource musicSource;

    [SerializeField]
    [Range(0, 1)]
    float soundVolume;

    [SerializeField]
    [Range(0, 1)]
    float musicVolume;

    public static AudioManager Singleton;

    bool queueIntro;

    [SerializeField]
    Music currentTrack;

    [SerializeField]
    AudioListener listener;

    [SerializeField]
    GameObject sourcePrefab;

	// Use this for initialization
	void Awake () {
        if (Singleton == null)
        {
            Singleton = this;
        }
        else
            Destroy(gameObject);

        DontDestroyOnLoad(gameObject);

        sources = new AudioSource[numSources];
        for (int i = 0; i < numSources; i++)
        {
            sources[i] = Instantiate(sourcePrefab, transform).GetComponent<AudioSource>();
        }
        
        //Get a reference to all our audiosources on startup
        sources = GetComponentsInChildren<AudioSource>();

        GameObject m = new GameObject("MusicSource");
        m.transform.parent = transform;
        m.AddComponent<AudioSource>();
        musicSource = m.GetComponent<AudioSource>();
        musicSource.priority = (int)Priority.Music;

        SetSoundVolume(soundVolume);
        SetMusicVolume(musicVolume);

        // Find the listener if not manually set
        if (listener == null)
        {
            listener = Camera.main.GetComponent<AudioListener>();
        }
    }

    private void OnLevelWasLoaded(int level)
    {
        // Find the listener if not manually set
        if (listener == null)
        {
            listener = Camera.main.GetComponent<AudioListener>();
        }
    }

    /// <summary>
    /// Swaps the current music track with the new music track
    /// </summary>
    /// <param name="m">Index of the music</param>
    /// <param name="hasIntro">Does the clip have an intro portion?</param>
    public void PlayMusic(Music m, bool hasIntro = false)
    {
        currentTrack = m;
        if (hasIntro)
        {
            musicSource.clip = trackIntros[(int)m];
            musicSource.loop = false;
        }
        else
        {
            musicSource.clip = tracks[(int)m];
            musicSource.loop = true;
        }
        queueIntro = hasIntro;

        musicSource.Play();
    }

    /// <summary>
    /// Stop whatever is playing in musicSource
    /// </summary>
    public void StopMusic()
    {
        musicSource.Stop();
    }

    private void Update()
    {
        if (queueIntro)
        {
            if (!musicSource.isPlaying)
            {
                musicSource.clip = tracks[(int)currentTrack];
                musicSource.loop = true;
                musicSource.Play();
                queueIntro = false;
            }
        }

        if (loopingChannels.Count > 0)
        {
            for (int i = 0; i < loopingChannels.Count; i++)
            {
                if (loopingSourcePositions[i] != null)
                {
                    loopingChannels[i].transform.position = loopingSourcePositions[i].transform.position;
                }
            }
        }
    }

    /// <summary>
    /// Equivalent of PlayOneShot
    /// </summary>
    /// <param name="s"></param>
    /// <param name="p">The priority of the sound</param>
    /// <param name="trans">The transform of the sound's source</param>
    public void PlaySoundOnce(Sound s, Priority p = Priority.Default, Transform trans = null)
    {
        AudioSource a = GetAvailableChannel();
        if (trans != null)
        {
            a.transform.position = trans.position;
        }
        else
        {
            a.transform.position = listener.transform.position;
        }  
        a.priority = (int)p;
        a.PlayOneShot(clips[(int)s]);
    }

    /// <summary>
    /// Play a sound and loop it forever
    /// </summary>
    /// <param name="s"></param>
    /// <param name="p">The priority of the sound</param>
    /// <param name="trans">The transform of the sound's source</param>
    public void PlaySoundLoop(Sound s, Priority p = Priority.Default, Transform trans = null)
    {
        loopingChannels.Add(GetAvailableChannel());
        if (trans != null)
        {
            loopingSourcePositions.Add(trans);
        }
        else
        {
            loopingSourcePositions.Add(listener.transform);
        }
        loopingChannels[loopingChannels.Count - 1].priority = (int)p;
        loopingChannels[loopingChannels.Count - 1].clip = clips[(int)s];
        loopingChannels[loopingChannels.Count - 1].Play();
        loopingChannels[loopingChannels.Count - 1].loop = true;
    }

    /// <summary>
    /// Stops a looping sound
    /// </summary>
    /// <param name="s"></param>
    /// <param name="stopPlaying">Stops sound instantly if true</param>
    public void StopSoundLoop(Sound s, bool stopPlaying = false)
    {
        for (int i = 0; i < loopingChannels.Count; i++)
        {
            if (loopingChannels[i].clip == clips[(int)s])
            {
                if (stopPlaying) loopingChannels[i].Stop();
                loopingChannels[i].loop = false;
                loopingChannels.RemoveAt(i);
                loopingSourcePositions.RemoveAt(i);
                return;
            }
        }
        //Debug.LogError("AudioManager Error: Did not find specified loop to stop!");
    }

    public void StopSoundLoopAll(bool stopPlaying = false)
    {
        foreach (AudioSource a in loopingChannels)
        {
            if (stopPlaying) a.Stop();
            a.loop = false;
            loopingChannels.Remove(a);
        }
        if (loopingSourcePositions != null)
            loopingSourcePositions.Clear();
    }

    public void SetSoundVolume(float v)
    {
        soundVolume = v;
        foreach (AudioSource s in sources)
        {
            s.volume = v;
        }
    }

    public void SetSoundVolume(UnityEngine.UI.Slider v)
    {
        soundVolume = v.value;
        foreach (AudioSource s in sources)
        {
            s.volume = soundVolume;
        }
    }

    public void SetMusicVolume(UnityEngine.UI.Slider v)
    {
        musicVolume = v.value;
        musicSource.volume = musicVolume;
    }

    /*
     * Called externally to update slider values
     */
    public void UpdateSoundSlider(UnityEngine.UI.Slider s)
    {
        s.value = soundVolume;
    }

    public void UpdateMusicSlider(UnityEngine.UI.Slider s)
    {
        s.value = musicVolume;
    }

    public void SetMusicVolume(float v)
    {
        musicVolume = v;
        musicSource.volume = musicVolume;
    }

    AudioSource GetAvailableChannel()
    {
        foreach(AudioSource a in sources)
        {
            if (!a.isPlaying)
            {
                return a;
            }
        }
        Debug.LogError("AudioManager Error: Ran out of Audio Channels!");
        return null;
    }

    public static AudioManager GetInstance()
    {
        return Singleton;
    }

    public float GetSoundVolume()
    {
        return soundVolume;
    }

    public float GetMusicVolume()
    {
        return musicVolume;
    }
}
