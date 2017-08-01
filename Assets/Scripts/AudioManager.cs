using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class AudioManager : MonoBehaviour
{
    public enum AudioChannel
    {
        Master,
        SFX,
        Music,
        Acoustic,
        Electric
    };

    public float masterVolume { get; private set; }
    public float sfxVolume { get; private set; }
    public float musicVolume { get; private set; }
    public float musicElectricVolume { get; private set; }
    public float musicAcousticVolume { get; private set; }

    AudioSource sfx;
    AudioSource musicAcoustic;
    AudioSource musicElectric;

    Transform audioListener;
    Transform playerT;

    public bool menuMusic;

    public enum Sound
    {
        Mine,
        CraftWire,
        PlaceWire,
        DiscoverSource,
        CompletePath,
        WireFail
    }

    [Header("Clips")]
    public AudioClip menu;
    public AudioClip acoustic;
    public AudioClip electric;

    public AudioClip mine;
    public AudioClip craftWire;
    public AudioClip placeWire;
    public AudioClip discoverSource;
    public AudioClip completePath;
    public AudioClip wireFail;

    void Awake()
    {
        GameObject sfx2DS = new GameObject("SFX_Source");
        sfx = sfx2DS.AddComponent<AudioSource>();
        sfx2DS.transform.parent = transform;

        audioListener = FindObjectOfType<AudioListener>().transform;
        if (FindObjectOfType<Character>() != null)
        {
            playerT = FindObjectOfType<Character>().transform;
        }

        masterVolume = 1;
        sfxVolume = 1;
        musicVolume = 0.8f;
        musicElectricVolume = 0;
        musicAcousticVolume = 1;

        GameObject sourceAcoustic = new GameObject("MusicSource Acoustic");
        musicAcoustic = sourceAcoustic.AddComponent<AudioSource>();
        GameObject sourceElectric = new GameObject("MusicSource Electric");
        musicElectric = sourceElectric.AddComponent<AudioSource>();
        PlayMusic();
    }

    void Update()
    {
        if (playerT != null)
        {
            audioListener.position = playerT.position;
        }
    }

    public void SetVolume(float volume, AudioChannel channel)
    {
        switch (channel)
        {
            case AudioChannel.Master:
                masterVolume = volume;
                break;
            case AudioChannel.SFX:
                sfxVolume = volume;
                break;
            case AudioChannel.Music:
                musicVolume = volume;
                break;
            case AudioChannel.Acoustic:
                musicAcousticVolume = volume;
                break;
            case AudioChannel.Electric:
                musicElectricVolume = volume;
                break;
        }

        musicAcoustic.volume = musicAcousticVolume * musicVolume * masterVolume;
        musicElectric.volume = musicElectricVolume * musicVolume * masterVolume;
    }

    public void PlayMusic()
    {
        if (menuMusic)
        {
            musicAcoustic.clip = menu;
            musicAcoustic.loop = true;
            musicAcoustic.Play();
        }
        else
        {
            musicAcoustic.clip = acoustic;
            musicAcoustic.loop = true;
            musicAcoustic.Play();
            musicElectric.clip = electric;
            musicElectric.loop = true;
            musicElectric.Play();
        }
    }

    public void PlaySound(Sound clipName)
    {
        AudioClip clip = null;
        switch (clipName)
        {
            case Sound.Mine:
                clip = mine;
                break;
            case Sound.CraftWire:
                clip = craftWire;
                break;
            case Sound.PlaceWire:
                clip = placeWire;
                break;
            case Sound.DiscoverSource:
                clip = discoverSource;
                break;
            case Sound.CompletePath:
                clip = completePath;
                break;
            case Sound.WireFail:
                clip = wireFail;
                break;
        }
        if (clip != null)
        {
            sfx.clip = clip;
            sfx.loop = false;
            sfx.Play();
        }
    }

    void OnLevelFinishedLoading(Scene scene, LoadSceneMode sceneMode)
    {
        if (playerT == null)
        {
            if (FindObjectOfType<Character>() != null)
            {
                playerT = FindObjectOfType<Character>().transform;
            }
        }
    }
}
