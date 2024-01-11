using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }
    private AudioSource _mainAudioSource;
    public AudioClip pickupItemSound;
    public AudioClip pickupInventoryItemSound;
    public AudioClip cancelInventoryItemPickupSound;
    public AudioClip hurtSound;
    public AudioClip dropItemSound;
    public AudioClip jumpSound;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
        }

        DontDestroyOnLoad(gameObject);
    }

    void LoadAudioSource()
    {
        if (_mainAudioSource == null)
        {
            _mainAudioSource = Camera.main.GetComponent<AudioSource>();
        }
    }

    public void PlayPickupItemSound()
    {
        LoadAudioSource();
        _mainAudioSource.PlayOneShot(pickupItemSound);
    }

    public void PlayPickupInventoryItemSound()
    {
        LoadAudioSource();
        _mainAudioSource.PlayOneShot(pickupInventoryItemSound);
    }

    public void PlayHurtSound(AudioSource audioSource = null)
    {
        if (audioSource == null)
        {
            LoadAudioSource();
            audioSource = _mainAudioSource;
        }
        audioSource.PlayOneShot(hurtSound);
    }

    public void PlayDropItemSound()
    {
        LoadAudioSource();
        _mainAudioSource.PlayOneShot(dropItemSound);
    }

    public void PlayCancelInventoryItemPickupSound()
    {
        LoadAudioSource();
        _mainAudioSource.PlayOneShot(cancelInventoryItemPickupSound);
    }

    public void PlayJumpSound(AudioSource audioSource = null)
    {
        if (audioSource == null)
        {
            LoadAudioSource();
            audioSource = _mainAudioSource;
        }
        audioSource.PlayOneShot(jumpSound);
    }
}
