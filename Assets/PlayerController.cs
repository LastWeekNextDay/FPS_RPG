using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public Camera playerCamera;
    public Character character;
    private readonly float _cameraUpLimit = -70f;
    private readonly float _cameraDownLimit = 55f;
    private Vector3 _localEulerAngles;
    private float _mouseSensitivity;

    void Awake()
    {
        _mouseSensitivity = PlayerPrefs.GetFloat("MouseSensitivity");
    }

    void Start()
    {
        _localEulerAngles = playerCamera.transform.localEulerAngles;
    }

    void Update()
    {
        Debugging();
        CameraRotation();
        Movement();
        MiscInputs();

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

    void MiscInputs()
    {
        if (Input.GetMouseButtonUp(1))
        {
            UIManager.Instance.ToggleInventory();
            UIManager.Instance.AllowCursor(UIManager.Instance.IsInventoryOpen);
        }
        if (Input.GetKeyUp(KeyCode.E))
        {
            var ray = new Ray(playerCamera.transform.position, playerCamera.transform.forward);
            var distance = 2f;
            if (Physics.Raycast(ray, out var hit, distance))
            {
                if (hit.collider != null)
                {
                    var item = hit.collider.gameObject;
                    switch(hit.collider.tag)
                    {
                        case "Item":
                            var invItem = UIManager.Instance.MakeItemIntoInventoryItem(item);
                            character.backpack.TryAddItem(invItem);
                            break;
                    }
                    
                }
            }
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