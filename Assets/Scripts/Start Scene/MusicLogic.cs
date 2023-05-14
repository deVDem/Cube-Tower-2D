using UnityEngine;

public class MusicLogic : MonoBehaviour
{
    public AudioClip[] musicClips;
    private AudioSource _musicSource;
    private bool playing;
    private uint currentClip = 0;

    void Start()
    {
        _musicSource = GetComponent<AudioSource>();
        playing = PlayerPrefs.GetInt("musicSetting", 1) == 1;
        _musicSource.clip = musicClips[currentClip];
        _musicSource.Play();
        DontDestroyOnLoad(gameObject);
    }

    private void FixedUpdate()
    {
        if (Mathf.Abs(_musicSource.volume - 1f) >= 0.0001f && playing) _musicSource.volume = _musicSource.volume+(1f-_musicSource.volume)*Time.fixedDeltaTime*4f;
        else if (Mathf.Abs(_musicSource.volume) >= 0.0001f && !playing) _musicSource.volume = _musicSource.volume+(0f-_musicSource.volume)*Time.fixedDeltaTime * 4f;
        if (!_musicSource.isPlaying)
        {
            currentClip++;
            if (currentClip >= musicClips.Length) currentClip = 0;
            _musicSource.clip = musicClips[currentClip];
            _musicSource.Play();
        }
    }

    public void Change(bool a)
    {
        playing = a;
    }
}