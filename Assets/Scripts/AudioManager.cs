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

    AudioSource sfx2DSource;
    AudioSource musicAcoustic;
    AudioSource musicElectric;

    Transform audioListener;
    Transform playerT;

    public bool menuMusic;
    public AudioClip menu;
    public AudioClip acoustic;
    public AudioClip electric;

    void Awake()
    {
        GameObject sfx2DS = new GameObject("SFX_Source");
        sfx2DSource = sfx2DS.AddComponent<AudioSource>();
        sfx2DS.transform.parent = transform;

        audioListener = FindObjectOfType<AudioListener>().transform;
        if (FindObjectOfType<Character>() != null)
        {
            playerT = FindObjectOfType<Character>().transform;
        }

        masterVolume = sfxVolume = musicVolume = 1;
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

    public void PlaySound(AudioClip clip, Vector3 pos)
    {
        if (clip != null)
            AudioSource.PlayClipAtPoint(clip, pos, sfxVolume * masterVolume);
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
