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

    void Start()
    {
        LoadAudioSource();
        InventoryItem.OnMouseAttach += (_) => PlayPickupInventoryItemSound();
        InventoryItem.OnDrop += (_) => PlayDropItemSound();
        InventoryItem.OnCancelSelection += (_) => PlayCancelInventoryItemPickupSound();
        Character.OnPickupItem += (args) => PlayPickupItemSound(args.Source.audioSource);
        Character.OnDamageTaken += (args) => PlayHurtSound(args.Target.audioSource);
        Character.OnJump += (args) => PlayJumpSound(args.Source.audioSource);
    }

    void LoadAudioSource()
    {
        if (_mainAudioSource == null)
        {
            _mainAudioSource = Camera.main.GetComponent<AudioSource>();
        }
    }

    public void PlayPickupItemSound(AudioSource audioSource = null)
    {
        if (audioSource == null)
        {
            audioSource = _mainAudioSource;
        }
        audioSource.PlayOneShot(pickupItemSound);
    }

    public void PlayPickupInventoryItemSound()
    {
        _mainAudioSource.PlayOneShot(pickupInventoryItemSound);
    }

    public void PlayHurtSound(AudioSource audioSource = null)
    {
        if (audioSource == null)
        {
            audioSource = _mainAudioSource;
        }
        audioSource.PlayOneShot(hurtSound);
    }

    public void PlayDropItemSound()
    {
        _mainAudioSource.PlayOneShot(dropItemSound);
    }

    public void PlayCancelInventoryItemPickupSound()
    {
        _mainAudioSource.PlayOneShot(cancelInventoryItemPickupSound);
    }

    public void PlayJumpSound(AudioSource audioSource = null)
    {
        if (audioSource == null)
        {
            audioSource = _mainAudioSource;
        }
        audioSource.PlayOneShot(jumpSound);
    }
}
