using UnityEngine;
using System.Collections;

public class SoundManager : MonoBehaviour
{

    [System.Serializable]
    public class AudioClipEntry
    {
        public string name;
        public AudioClip clip;
    }

    public AudioClipEntry[] audioClipEntries;

    private AudioSource backgroundSoundSource;
    private AudioSource fxSoundSource;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != this)
        {
            Destroy(gameObject);
        }

        var goBackgroundSoundSource = transform.FindDeepChild("BackgroundSound");
        var goFXSoundSource = transform.FindDeepChild("FXSound");

        if (goBackgroundSoundSource == null || goFXSoundSource == null)
        {
            throw new UnityException("Couldn't find audio sources.");
        }
        else
        {
            backgroundSoundSource = goBackgroundSoundSource.GetComponent<AudioSource>();
            fxSoundSource = goFXSoundSource.GetComponent<AudioSource>();
        }

        backgroundSoundSource.clip = gameMusic;

        int playerMusic = 1;//PlayerPrefs.GetInt("SoundManager_MusicOn", 1);
        if (playMusicAtStart && playerMusic == 1)
        {
            TurnOnAllSound(true);
        }
        else
        {
            TurnOnAllSound(false);
        }
    }

    private void TurnOnAudioSource(bool turnOn, AudioSource source)
    {
        source.enabled = turnOn;
    }

    public void TurnOnMusic(bool turnOn)
    {
        PlayerPrefs.SetInt("SoundManager_MusicOn", turnOn ? 1 : 0);
        TurnOnAudioSource(turnOn, backgroundSoundSource);
        backgroundSoundSource.Play();
    }

    // Publicly available varibles, properties and methods
    public static SoundManager instance = null; // Static ref
    public bool playMusicAtStart = true;
    public AudioClip gameMusic;

    public bool IsMusicOn { get { return backgroundSoundSource.enabled; } }
    public bool IsFxOn { get { return fxSoundSource.enabled; } }

    public void TurnOnFX(bool turnOn)
    {
        PlayerPrefs.SetInt("SoundManager_FxOn", turnOn ? 1 : 0);
        TurnOnAudioSource(turnOn, fxSoundSource);
    }

    public void TurnOnAllSound(bool turnOn)
    {
        TurnOnMusic(turnOn);
        TurnOnFX(turnOn);
    }

    // Play fx sound from sudio source. If no audio souce is provdied it uses the default fx audio source. 
    public void PlayFX(string name, AudioSource source = null)
    {
        if (audioClipEntries == null)
            return; // Fail silently

        AudioClip clipToPlay = null;
        foreach (AudioClipEntry e in audioClipEntries)
        {
            if (e.name == name)
            {
                clipToPlay = e.clip;
                break;
            }
        }


        if (clipToPlay != null)
        {
            if (IsFxOn)
            {
                if (source == null)
                {
                    fxSoundSource.clip = clipToPlay;
                    fxSoundSource.Play();
                }
                else
                {
                    source.clip = clipToPlay;
                    source.Play();
                }
            }
        }
        else
        {
            throw new UnityException("Could not find audio clip for name: " + name);
        }
    }

    public void StopFX(AudioSource source = null)
    {
        if (source == null)
        {
            fxSoundSource.Stop();
        }
        else
        {
            source.Stop();
        }
    }
}
