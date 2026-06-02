using System;
using UnityEngine;

public class MusicManager : MonoBehaviour {
    
    private const int MUSIC_VOLUME_MAX = 10;
    private const string PREFS_MUSIC_VOLUME = "MusicVolume";
    
    public static MusicManager Instance { get; private set; }

    private static float musicTime;
    private static int musicVolume = 4;
    
    public event EventHandler OnMusicVolumeChanged;
    
    private AudioSource musicAudioSource;

    private void Awake() {
        Instance = this;

        musicVolume = PlayerPrefs.GetInt(PREFS_MUSIC_VOLUME, 4);
        musicVolume = Mathf.Clamp(musicVolume, 0, MUSIC_VOLUME_MAX - 1);

        musicAudioSource = GetComponent<AudioSource>();
        musicAudioSource.time = musicTime;
    }

    private void Start() {
        musicAudioSource.volume = GetMusicVolumeNormalized();
    }

    private void Update() {
        musicTime = musicAudioSource.time;
    }
    
    public void ChangeMusicVolume() {
        musicVolume = (musicVolume + 1) % MUSIC_VOLUME_MAX;
        
        PlayerPrefs.SetInt(PREFS_MUSIC_VOLUME, musicVolume);
        PlayerPrefs.Save();
        
        musicAudioSource.volume = GetMusicVolumeNormalized();
        OnMusicVolumeChanged?.Invoke(this, EventArgs.Empty);
    }

    public int GetMusicVolume() {
        return musicVolume;
    }

    public float GetMusicVolumeNormalized() {
        return ((float)musicVolume) / MUSIC_VOLUME_MAX;
    }
}
