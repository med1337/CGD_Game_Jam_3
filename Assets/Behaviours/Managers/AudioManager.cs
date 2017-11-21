using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public enum MusicType
{
    TITLE,
    GAME
}

public class AudioManager : MonoBehaviour
{
    public static AudioSettings settings { get { return instance.settings_; } }

    [SerializeField] AudioSettings settings_;

    [Header("Music")]
    [SerializeField] AudioClip title_music;
    [SerializeField] AudioClip game_music;

    private static AudioManager instance;

    private AudioSource music_source;
    private AudioSource sfx_source;
    private AudioSource sfx_unscaled_source;

    private AudioClip last_clip_played;


    public static void PlayMusic(MusicType _music)
    {
        instance.music_source.Stop();

        switch (_music)
        {
            case MusicType.TITLE: instance.music_source.clip = instance.title_music; break;
            case MusicType.GAME: instance.music_source.clip = instance.game_music; break;
        }

        if (instance.music_source.clip == null)
            return;

        instance.music_source.loop = true;
        instance.music_source.Play();
    }


    public static void PlayOneShot(string _clip_name)
    {
        PlayOneShot(instance.GetAudioClip(_clip_name));
    }


    public static void PlayOneShot(AudioClip _clip)
    {
        if (instance.last_clip_played == _clip)
            return;

        instance.last_clip_played = _clip;

        if (_clip != null)
            instance.sfx_source.PlayOneShot(_clip);
    }


    public static void PlayOneShotUnscaled(string _clip_name)
    {
        PlayOneShotUnscaled(instance.GetAudioClip(_clip_name));
    }


    public static void PlayOneShotUnscaled(AudioClip _clip)
    {
        if (instance.last_clip_played == _clip)
            return;

        instance.last_clip_played = _clip;

        if (_clip != null)
            instance.sfx_unscaled_source.PlayOneShot(_clip);
    }


    public static void StopAllSFX()
    {
        instance.sfx_source.Stop();
    }


    public AudioClip GetAudioClip(string _clip_name)
    {
        return settings.sfx_clips.Find(elem => elem != null &&
            elem.name.Substring(0) == _clip_name);
    }


    void Awake()
    {
        if (instance == null)
        {
            InitSingleton();
        }
        else
        {
            Destroy(this.gameObject);
        }
    }


    void InitSingleton()
    {
        instance = this;

        GameObject audio_parent = new GameObject("Audio");
        audio_parent.transform.SetParent(this.transform);

        music_source = audio_parent.AddComponent<AudioSource>();
        sfx_source = audio_parent.AddComponent<AudioSource>();
        sfx_unscaled_source = audio_parent.AddComponent<AudioSource>();

        music_source.volume = settings.music_volume;
        sfx_source.volume = settings.sfx_volume;
        sfx_unscaled_source.volume = settings.sfx_volume;
    }


    void Update()
    {
        sfx_source.pitch = Time.timeScale;
    }


    void LateUpdate()
    {
        last_clip_played = null;
    }

}
