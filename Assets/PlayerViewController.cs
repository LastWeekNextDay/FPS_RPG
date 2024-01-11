using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerViewController : MonoBehaviour
{
    private Camera _camera;
    private Character player;
    private Vector3 _camera_position;
    private Quaternion _camera_rotation;

    private float _sensitivity;

    public void MoveAround()
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
        _sensitivity = PlayerPrefs.GetFloat("MouseSensitivity", 1f);
        var mouse_x = Input.GetAxis("Mouse X");
        UIManager.Instance.CursorVisible(false);
        _camera.transform.RotateAround(player.transform.position, player.transform.up, mouse_x * Time.deltaTime * _sensitivity);    
    }

    public void Reset()
    {
        UIManager.Instance.CursorVisible(true);
        UIManager.Instance.ResetCursor();
        _camera.transform.SetLocalPositionAndRotation(_camera_position, _camera_rotation);
    }
}
