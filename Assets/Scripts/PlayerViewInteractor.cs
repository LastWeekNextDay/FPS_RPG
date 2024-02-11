using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerViewInteractor : MonoBehaviour
{
    private Camera _camera;
    private Character player;
    private Vector3 _camera_position;
    private Quaternion _camera_rotation;
    public static Action OnMoveAround;
    public static Action OnReset;

    private float _sensitivity;

    void Start()
    {
        if (_camera == null)
        {
            if (UnityEssential.TryFindObject("PlayerInvCam", out GameObject player_inv_cam))
            {
                _camera = player_inv_cam.GetComponent<Camera>();
                _camera_position = _camera.transform.localPosition;
                _camera_rotation = _camera.transform.localRotation;
            }
        }
        if (player == null)
        {
            if (UnityEssential.TryFindObject("Player", out GameObject player_obj))
            {
                player = player_obj.GetComponent<Character>();
            }
        }
    }

    public void MoveAround()
    {
        var mouse_x = Input.GetAxis("Mouse X");
        _sensitivity = PlayerPrefs.GetFloat("MouseSensitivity", 1f);
        _camera.transform.RotateAround(player.transform.position, player.transform.up, mouse_x * Time.deltaTime * _sensitivity);

        OnMoveAround?.Invoke();    
    }

    public void Reset()
    {
        _camera.transform.SetLocalPositionAndRotation(_camera_position, _camera_rotation);
        
        OnReset?.Invoke();
    }
}
