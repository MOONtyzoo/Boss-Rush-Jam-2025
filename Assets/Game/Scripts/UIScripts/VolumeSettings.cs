using UnityEngine;
using UnityEngine.UI;

public class VolumeSettings : MonoBehaviour
{
    private float masterVolume;
    private float musicVolume;
    private float soundEffectsVolume;

    [SerializeField] private Slider masterVolumeSlider;
    [SerializeField] private Slider musicVolumeSlider;
    [SerializeField] private Slider soundEffectsVolumeSlider;

    private void Start()
    {
        masterVolume = AudioManager.masterVolume;
        musicVolume = AudioManager.musicVolume;
        soundEffectsVolume = AudioManager.soundEffectsVolume;

        masterVolumeSlider.value = AudioManager.masterVolume;
        musicVolumeSlider.value = AudioManager.musicVolume;
        soundEffectsVolumeSlider.value = AudioManager.soundEffectsVolume;
    }

    public void OnMasterSliderValueChange(float value)
    {
        masterVolume = value;
        AudioManager.Instance.UpdateMixerVolume(masterVolume, musicVolume, soundEffectsVolume);
    }

    public void OnMusicSliderValueChange(float value)
    {
        musicVolume = value;
        AudioManager.Instance.UpdateMixerVolume(masterVolume, musicVolume, soundEffectsVolume);
    }

    public void OnSoundEffectsSliderValueChange(float value)
    {
        soundEffectsVolume = value;
        AudioManager.Instance.UpdateMixerVolume(masterVolume, musicVolume, soundEffectsVolume);
    }
}
