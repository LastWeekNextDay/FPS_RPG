using System;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public Camera playerCamera;
    public Character character;
    private readonly float _cameraUpLimit = -70f;
    private readonly float _cameraDownLimit = 55f;
    private Vector3 _localEulerAngles;
    private float _mouseSensitivity;
    public static Action<PrimaryActionArgs> OnPrimaryAction;
    public static Action<SecondaryActionArgs> OnSecondaryAction;
    public static Action<UseActionArgs> OnUseAction;
    public static Action<ReadyActionArgs> OnReadyAction;
    public bool IsControllingAllowed;

    void Awake()
    {
        _mouseSensitivity = PlayerPrefs.GetFloat("MouseSensitivity");
        IsControllingAllowed = true;
    }

    void Start()
    {
        _localEulerAngles = playerCamera.transform.localEulerAngles;
    }

    void Update()
    {
        Debugging();
        if (IsControllingAllowed)
        {
            CameraRotation();
            Movement();
            MiscInputs();
        }
    }

    void CameraRotation()
    {
        if (UIManager.Instance.IsInventoryOpen)
        {
            return;
        }

        var camLeftRightRawDelta = Input.GetAxis("Mouse X");
        var camUpDownRawDelta = Input.GetAxis("Mouse Y");
        var camLeftRightDelta = camLeftRightRawDelta * _mouseSensitivity;
        var camUpDownDelta = camUpDownRawDelta * _mouseSensitivity;

        var newCamUpDown = _localEulerAngles.x + -camUpDownDelta * Time.deltaTime;

        character.transform.Rotate(0f, camLeftRightDelta * Time.deltaTime, 0f);

        if (_cameraUpLimit <= newCamUpDown && newCamUpDown <= _cameraDownLimit)
        {
            _localEulerAngles.x = newCamUpDown;
            playerCamera.transform.localEulerAngles = _localEulerAngles;
        }
    }

    void Movement()
    {
        if (Input.GetKey(KeyCode.Space))
        {
            character.Jump();
        }

        var moveInput = new Vector3(Input.GetAxis("Horizontal"), 0f, Input.GetAxis("Vertical"));

        var moveVector = character.transform.TransformDirection(moveInput).normalized;
        moveVector = new Vector3(moveVector.x * character.Speed, 
                                character.rigidBody.velocity.y, 
                                moveVector.z * character.Speed);

        character.rigidBody.velocity = moveVector;
    }

    void PrimaryAction()
    {
        var dir = playerCamera.transform.forward;
        character.AttackPrimary(dir.normalized);
        var args = new PrimaryActionArgs{
            Source = character,
        };
        OnPrimaryAction?.Invoke(args);
    }

    void SecondaryAction()
    {
        var args = new SecondaryActionArgs{
            Source = character,
        };
        OnSecondaryAction?.Invoke(args);
    }

    void UseAction()
    {
        var ray = new Ray(playerCamera.transform.position, playerCamera.transform.forward);
        var distance = 2f;
        if (Physics.Raycast(ray, out var hit, distance))
        {
            if (hit.collider != null)
            {
                switch(hit.collider.tag)
                {
                    case "Item":
                        var itemC = hit.collider.gameObject.GetComponent<Item>();
                        character.PickupItem(itemC);
                        break;
                }
            }
        }
        var args = new UseActionArgs{
            Source = character,
        };
        OnUseAction?.Invoke(args);
    }

    void ReadyAction()
    {
        character.ReadyWeapon();
        var args = new ReadyActionArgs{
            Source = character,
        };
        OnReadyAction?.Invoke(args);
    }

    void MiscInputs()
    {
        if (Input.GetMouseButtonUp(0))
        {
            PrimaryAction();
        }
        if (Input.GetMouseButtonUp(1))
        {
            SecondaryAction();
        }
        if (Input.GetKeyUp(KeyCode.E))
        {
            UseAction();
        }
        if (Input.GetKeyUp(KeyCode.R))
        {
            ReadyAction();
        }
    }

    void Debugging()
    {
        var charRay = new Ray(character.transform.position, character.transform.forward);
        var distance = character.SightRange;
        if (Physics.Raycast(charRay, out var hit, distance)){
            distance = hit.distance;
        }
        Debug.DrawRay(charRay.origin, charRay.direction * distance, Color.red);
        
        var lookRay = new Ray(playerCamera.transform.position, playerCamera.transform.forward);
        distance = character.SightRange;
        if (Physics.Raycast(lookRay, out hit, distance)){
            distance = hit.distance;
        }
        Debug.DrawRay(lookRay.origin, lookRay.direction * distance, Color.blue);
    }
}