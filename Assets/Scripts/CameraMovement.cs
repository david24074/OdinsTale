using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    [Header("Camera Settings")]
    [SerializeField] private float cameraMovementSpeed;
    [SerializeField] private float rotationSpeed;

    [Header("World Settings")]
    [SerializeField] private float waterheight;
    [SerializeField] private float worldBottomLevel;

    void Update()
    {
        transform.Translate(new Vector3(Input.GetAxis("Horizontal"), 0.0f, Input.GetAxis("Vertical")) * cameraMovementSpeed * Time.deltaTime);

        if (Input.GetKey(KeyCode.Q)) { transform.Rotate(-transform.up * rotationSpeed * Time.deltaTime); }
        if (Input.GetKey(KeyCode.E)) { transform.Rotate(transform.up * rotationSpeed * Time.deltaTime); }
    }

    private void ToggleCursor(bool value)
    {
        //if (value) { Cursor.lockState = CursorLockMode.None; } else { Cursor.lockState = CursorLockMode.Locked; }
        Cursor.visible = value;
    }

}


/*      //If the player is underwater then enable the underwater fog
  if (transform.position.y <= waterheight) { RenderSettings.fog = true; }
  else
  {
      RenderSettings.fog = false;
      if (transform.position.y <= worldBottomLevel) { transform.position = new Vector3(transform.position.x, spawnHeight, transform.position.z); }
  }*/

