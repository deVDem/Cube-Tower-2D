using System.Collections;
using Game_Scene;
using UnityEngine;
using UnityEngine.Audio;
using Random = UnityEngine.Random;

public class Cube : MonoBehaviour
{
    public float delayToDestroy = 2.5f;
    public AudioClip ClipDestroy;
    public AudioMixerGroup SFXMixer;
    private Coroutine _coroutine;


    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.CompareTag("World") && gameObject.transform.parent == null)
        {
            _coroutine = StartCoroutine(DestroyDelay());
        }
    }

    private IEnumerator DestroyDelay()
    {
        yield return new WaitForSeconds(Random.Range(1f, delayToDestroy));
        float volume = 0.3f - Mathf.Abs(transform.position.x / GameController.GetSizeCamera())*0.3f;
        if (volume >= 0.001f)
        {
            GameObject soundObject = new GameObject();
            soundObject.name = "Sound";
            soundObject.AddComponent<DestroyDelay>();
            AudioSource audioSource = soundObject.AddComponent<AudioSource>();
            audioSource.clip = ClipDestroy;
            audioSource.volume = volume;
            audioSource.outputAudioMixerGroup = SFXMixer;
            audioSource.Play();
        }

        Destroy(gameObject);
        StopCoroutine(_coroutine);
    }
}