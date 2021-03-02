using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    [Header("Camera Settings")]
    [SerializeField] private float movementSpeed;
    [SerializeField] private float fastMoveFactor;
    [SerializeField] private float climbSpeed;
    [SerializeField] private float cameraSensitivity;
    [SerializeField] private float waterheight;
    [SerializeField] private float worldBottomLevel;

    private float spawnHeight;
    private float _rotationX;
    private float _rotationY;

    void Start()
    {
        _rotationX = transform.eulerAngles.y;
        spawnHeight = transform.position.y;
    }

    void Update()
    {
        if (Input.GetMouseButton(1))
        {
            ToggleCursor(false);
            _rotationX += Input.GetAxis("Mouse X") * cameraSensitivity * Time.deltaTime;
            _rotationY += Input.GetAxis("Mouse Y") * cameraSensitivity * Time.deltaTime;
            _rotationY = Mathf.Clamp(_rotationY, -90, 90);
        }
        if (Input.GetMouseButtonUp(1)) { ToggleCursor(true); }

        Quaternion targetRotation = Quaternion.AngleAxis(_rotationX, Vector3.up);
        targetRotation *= Quaternion.AngleAxis(_rotationY, Vector3.left);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 4f);

        float speedFactor = 1f;
        if (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift)) speedFactor = fastMoveFactor;
        if (Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl)) speedFactor = fastMoveFactor;

        transform.position += transform.forward * movementSpeed * speedFactor * Input.GetAxis("Vertical") * Time.deltaTime;
        transform.position += transform.right * movementSpeed * speedFactor * Input.GetAxis("Horizontal") * Time.deltaTime;
        float upAxis = 0;
        if (Input.GetKey(KeyCode.Q)) upAxis = -0.5f;
        if (Input.GetKey(KeyCode.E)) upAxis = 0.5f;
        transform.position += transform.up * movementSpeed * speedFactor * upAxis * Time.deltaTime;

        //If the player is underwater then enable the underwater fog
        if (transform.position.y <= waterheight) { RenderSettings.fog = true; } else
        {
            RenderSettings.fog = false;
            if (transform.position.y <= worldBottomLevel) { transform.position = new Vector3(transform.position.x, spawnHeight, transform.position.z); }
        }
    }


    private void ToggleCursor(bool value)
    {
        if (value) { Cursor.lockState = CursorLockMode.None; } else { Cursor.lockState = CursorLockMode.Locked; }
        Cursor.visible = value;
    }
}
