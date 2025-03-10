using System.Collections;
using UnityEngine;
using UnityEngine.Audio;

public enum Music
{
    TitleScreen,
    Level1_A,
    Level1_B,
    Level1_C,
    Level1_D,
    Level1_Spin
}

public enum SoundEffects
{
    JumpGround,
    JumpWall,
    TakeDamage,
    ShootGrapple,
    ShootGun,
    RoomRotation,
    MadMoleRoar,
    MadMoleJump,
    MadMoleLand,
    MadMoleSpin,
    MadMoleSuperLand,
}

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    [SerializeField] private AudioMixerGroup masterMixerGroup;
    [SerializeField] private AudioMixerGroup musicMixerGroup;
    [SerializeField] private AudioMixerGroup soundEffectsMixerGroup;

    public static float masterVolume { get; private set; }
    public static float musicVolume { get; private set; }
    public static float soundEffectsVolume { get; private set; }
    
    [SerializeField] private AudioSource musicAudioSourceA;
    [SerializeField] private AudioSource musicAudioSourceB;
    private AudioSource currentMusicSource;
    private AudioSource otherMusicSource;
    private Music currentMusicTrack;

    [SerializeField] private AudioSource sfxAudioSource;

    [SerializeField] private AudioClip[] musicList;
    [SerializeField] private AudioClip[] soundList;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        } else {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }

    private void Start()
    {
        currentMusicSource = musicAudioSourceA;
        otherMusicSource = musicAudioSourceB;

        SetVolumeFromSaved();

        SwapMusicTrack(Music.TitleScreen);
    }

    /// <summary>
    /// Play music from an enum list of music.
    /// </summary>
    /// <param name="music">The music you want to play.</param>
    /// <param name="volume">(Optional) Set a linear transition between the two tracks with a value other than 0</param>
    public static void SwapMusicTrack(Music newMusicTrack, float transitionDuration = 0f) {
        if(Instance.musicList[(int)newMusicTrack] == null) {
            Debug.LogError("Error: Can't find music entry.");
            return;
        }

        Instance.currentMusicTrack = newMusicTrack;

        if (transitionDuration == 0f) {
            Instance.musicAudioSourceA.clip = Instance.musicList[(int)newMusicTrack];
            Instance.musicAudioSourceA.Play();
        } else {
            AudioClip newMusicClip = Instance.musicList[(int)newMusicTrack];
            Instance.StartCoroutine(SwapMusicTrackCoroutine(newMusicClip, transitionDuration));
        }
    }

    private static IEnumerator SwapMusicTrackCoroutine(AudioClip newMusicClip, float transitionDuration) {
        float timer = 0;
        float timeNormalized = 0;

        float musicPlayTime = Instance.currentMusicSource.time;
        Instance.otherMusicSource.clip = newMusicClip;
        Instance.otherMusicSource.Play();
        Instance.otherMusicSource.time = musicPlayTime;
        Instance.currentMusicSource.time = musicPlayTime;
        
        while (timer < transitionDuration) {
            timer += Time.unscaledDeltaTime;
            timeNormalized = timer/transitionDuration;

            Instance.currentMusicSource.volume = 1f - timeNormalized;
            Instance.otherMusicSource.volume = timeNormalized;

            yield return null;
        }

        AudioSource tempRef = Instance.currentMusicSource;
        Instance.currentMusicSource = Instance.otherMusicSource;
        Instance.otherMusicSource = tempRef;
    }

    public static Music GetCurrentMusicTrack() => Instance.currentMusicTrack;

    /// <summary>
    /// Play a sound effect from an enum list of sound effects.
    /// </summary>
    /// <param name="sound">The sound effect you want to play.</param>
    /// <param name="volume">(Optional) The volume of the sound effect. Only change this if the sound effect needs to be lowered apart from the audio mixer. Default = 1</param>
    public static void PlaySound(SoundEffects sound, float volume = 1)
    {
        if(Instance.soundList[(int)sound] != null)
        {
            Instance.sfxAudioSource.PlayOneShot(Instance.soundList[(int)sound], volume);
        } else
        {
            Debug.LogError("Error: Can't find sound effect entry.");
        }
    }

    /// <summary>
    /// Updates the mixer volume.
    /// </summary>
    /// <param name="master">The volume of the master group.</param>
    /// <param name="music">The volume of the music group.</param>
    /// <param name="soundEffects">The volume of the sound effects group.</param>
    public void UpdateMixerVolume(float master, float music, float soundEffects)
    {
        masterMixerGroup.audioMixer.SetFloat("Master Volume", master > 0 ? Mathf.Log10(master) * 20f : -80f);
        musicMixerGroup.audioMixer.SetFloat("Music Volume", music > 0 ? Mathf.Log10(music) * 20f : -80f);
        soundEffectsMixerGroup.audioMixer.SetFloat("SFX Volume", soundEffects > 0 ? Mathf.Log10(soundEffects) * 20f : -80f);

        SaveMixerVolume(master, music, soundEffects);
    }

    private void SaveMixerVolume(float master, float music, float soundEffects)
    {
        masterVolume = master;
        musicVolume = music;
        soundEffectsVolume = soundEffects;

        PlayerPrefs.SetFloat("masterVolume", masterVolume);
        PlayerPrefs.SetFloat("musicVolume", musicVolume);
        PlayerPrefs.SetFloat("soundEffectsVolume", soundEffectsVolume);
    }

    private void SetVolumeFromSaved()
    {
        masterVolume = PlayerPrefs.GetFloat("masterVolume", 0.6f);
        musicVolume = PlayerPrefs.GetFloat("musicVolume", 0.6f);
        soundEffectsVolume = PlayerPrefs.GetFloat("soundEffectsVolume", 0.6f);

        UpdateMixerVolume(masterVolume, musicVolume, soundEffectsVolume);
    }
}

