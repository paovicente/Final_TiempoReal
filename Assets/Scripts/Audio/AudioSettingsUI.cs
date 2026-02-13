using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class AudioSettingsUI : MonoBehaviour
{
    [SerializeField] private Slider musicSlider;
    [SerializeField] private Slider sfxSlider;
    [SerializeField] private Slider playerSlider;
    [SerializeField] private Toggle musicToggle;
    [SerializeField] private Toggle sfxToggle;

    private void OnEnable()
    {
        StartCoroutine(SyncSliders());
    }

    private IEnumerator SyncSliders()
    {
        yield return null;

        if (AudioManager.instance == null) yield break;

        musicSlider.onValueChanged.RemoveAllListeners();
        sfxSlider.onValueChanged.RemoveAllListeners();
        playerSlider.onValueChanged.RemoveAllListeners();
        
        musicToggle.onValueChanged.RemoveAllListeners();
        sfxToggle.onValueChanged.RemoveAllListeners();

        //to avoid a call to On Value Changed
        musicSlider.SetValueWithoutNotify(AudioManager.instance.GetMusic());
        sfxSlider.SetValueWithoutNotify(AudioManager.instance.GetSFX());
        playerSlider.SetValueWithoutNotify(AudioManager.instance.GetPlayerSound());
        
        musicToggle.SetIsOnWithoutNotify(AudioManager.instance.IsMusicEnabled());
        sfxToggle.SetIsOnWithoutNotify(AudioManager.instance.IsSFXEnabled());

        //listeners for sliders and toggles
        musicSlider.onValueChanged.AddListener(AudioManager.instance.SetMusic);
        sfxSlider.onValueChanged.AddListener(AudioManager.instance.SetSFX);
        playerSlider.onValueChanged.AddListener(AudioManager.instance.SetPlayerSound);
        
        musicToggle.onValueChanged.AddListener(AudioManager.instance.SetMusicEnabled);
        sfxToggle.onValueChanged.AddListener(AudioManager.instance.SetSFXEnabled);

        //to deactivate the slider if music/sfx is deactivate
        musicSlider.interactable = musicToggle.isOn;
        musicToggle.onValueChanged.AddListener(value => musicSlider.interactable = value);

        sfxSlider.interactable = sfxToggle.isOn;
        sfxToggle.onValueChanged.AddListener(value => sfxSlider.interactable = value);
    }

}