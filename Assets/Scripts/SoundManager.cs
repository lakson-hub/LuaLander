using System;
using UnityEngine;

public class SoundManager : MonoBehaviour {

    private const int SOUND_VOLUME_MAX = 10;
    private const string PREFS_SOUND_VOLUME = "SoundVolume";
    
    public static SoundManager Instance { get; private set; }
    
    private static int soundVolume = 5;

    public event EventHandler OnSoundVolumeChanged;

    [SerializeField] private AudioClip fuelPickupAudioClip;
    [SerializeField] private AudioClip coinPickupAudioClip;
    [SerializeField] private AudioClip crashAudioClip;
    [SerializeField] private AudioClip landingSuccessAudioClip;

    private void Awake() {
        Instance = this;

        soundVolume = PlayerPrefs.GetInt(PREFS_SOUND_VOLUME, 5);
        soundVolume = Mathf.Clamp(soundVolume, 0, SOUND_VOLUME_MAX - 1);
    }

    private void Start() {
        Lander.Instance.OnFuelPickup += Lander_OnFuelPickup;
        Lander.Instance.OnCoinPickup += Lander_OnCoinPickup;
        Lander.Instance.OnLanded += Lander_OnLanded;
        Lander.Instance.OnFellOutOfMap += Lander_OnFellOutOfMap;
    }

    private void Lander_OnFellOutOfMap(object sender, EventArgs e) {
        AudioSource.PlayClipAtPoint(crashAudioClip, Camera.main.transform.position, GetSoundVolumeNormalized());
    }

    private void Lander_OnLanded(object sender, Lander.OnLandedEventArgs e) {
        switch (e.landingType) {
            case Lander.LandingType.Success:
                AudioSource.PlayClipAtPoint(landingSuccessAudioClip, Camera.main.transform.position, GetSoundVolumeNormalized());
                break;
            default:
                AudioSource.PlayClipAtPoint(crashAudioClip, Camera.main.transform.position, GetSoundVolumeNormalized());
                break;
        }
    }

    private void Lander_OnCoinPickup(object sender, EventArgs e) {
        AudioSource.PlayClipAtPoint(coinPickupAudioClip, Camera.main.transform.position, GetSoundVolumeNormalized());
    }

    private void Lander_OnFuelPickup(object sender, EventArgs e) {
        AudioSource.PlayClipAtPoint(fuelPickupAudioClip, Camera.main.transform.position, GetSoundVolumeNormalized());
    }

    public void ChangeSoundVolume() {
        soundVolume = (soundVolume + 1) % SOUND_VOLUME_MAX;
        
        PlayerPrefs.SetInt(PREFS_SOUND_VOLUME, soundVolume);
        PlayerPrefs.Save();
        
        OnSoundVolumeChanged?.Invoke(this, EventArgs.Empty);
    }

    public int GetSoundVolume() {
        return soundVolume;
    }

    public float GetSoundVolumeNormalized() {
        return ((float)soundVolume) / SOUND_VOLUME_MAX;
    }
}
