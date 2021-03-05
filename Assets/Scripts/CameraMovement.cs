using UnityEngine;

public class CameraMovement : MonoBehaviour
{

    [SerializeField] private float panSpeed = 20f;
    [SerializeField] private float rotSpeed = 10f;
    [SerializeField] private float zoomSpeed = 50f;
    [SerializeField] private float borderWidth = 10f;
    [SerializeField] private bool edgeScrolling = true;
    [SerializeField] private float zoomMin = 0.0f;
    [SerializeField] private float zoomMax = 49.0f;
    private float mouseX, mouseY;


    void Update()
    {
        Movement();
        Rotation();
        Zoom();
    }


    void Movement()
    {
        Vector3 pos = transform.position;
        Vector3 forward = transform.forward;
        forward.y = 0;

        forward.Normalize();
        Vector3 right = transform.right;
        right.y = 0;
        right.Normalize();

        if (Input.GetKey("w") || edgeScrolling == true && Input.mousePosition.y >= Screen.height - borderWidth)
        {
            pos += forward * panSpeed * Time.deltaTime;
        }

        if (Input.GetKey("s") || edgeScrolling == true && Input.mousePosition.y <= borderWidth)
        {
            pos -= forward * panSpeed * Time.deltaTime;
        }

        if (Input.GetKey("d") || edgeScrolling == true && Input.mousePosition.x >= Screen.width - borderWidth)
        {
            pos += right * panSpeed * Time.deltaTime;           
        }

        if (Input.GetKey("a") || edgeScrolling == true && Input.mousePosition.x <= borderWidth)
        {
            pos -= right * panSpeed * Time.deltaTime;
        }
        transform.position = pos;
    }


    void Rotation()
    {
        if (Input.GetMouseButton(1))
        {
            mouseX += Input.GetAxis("Mouse X") * rotSpeed;
            mouseY -= Input.GetAxis("Mouse Y") * rotSpeed;
            mouseY = Mathf.Clamp(mouseY, -30, 45);

            ToggleCursor(false);
            transform.rotation = Quaternion.Euler(mouseY, mouseX, 0);
        }
        else
        {
            ToggleCursor(true);
        }
    }


    void Zoom()
    {
        Vector3 camPos = transform.position;

        float distance = Vector3.Distance(transform.position, new Vector3(transform.position.x, 0, transform.position.z));

        if (Input.GetAxis("Mouse ScrollWheel") > 0f && distance > zoomMin)
        {
            camPos += transform.forward * zoomSpeed * Time.deltaTime;
        }

        if (Input.GetAxis("Mouse ScrollWheel") < 0f && distance < zoomMax)
        {
            camPos -= transform.forward * zoomSpeed * Time.deltaTime;
        }
        transform.position = camPos;
    }

    private void ToggleCursor(bool value)
    {
        if (value) { Cursor.lockState = CursorLockMode.None; } else { Cursor.lockState = CursorLockMode.Locked; }
        Cursor.visible = value;
    }
}

