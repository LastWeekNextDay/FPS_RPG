using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }
    private AudioSource _mainAudioSource;
    public AudioClip pickupItemSound;
    public AudioClip pickupItemContainerSound;
    public AudioClip cancelItemContainerPickupSound;
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

        InventoryItemContainer.OnMouseAttach += (_) => PlayPickupItemContainerSound();

        InventoryItemContainer.OnDrop += (_) => PlayDropItemSound();

        InventoryItemContainer.OnCancelSelection += (_) => PlayCancelItemContainerPickupSound();

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

    public void PlayPickupItemContainerSound()
    {
        _mainAudioSource.PlayOneShot(pickupItemContainerSound);
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

    public void PlayCancelItemContainerPickupSound()
    {
        _mainAudioSource.PlayOneShot(cancelItemContainerPickupSound);
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
