using UnityEngine;

public class CameraDragZoom1 : MonoBehaviour
{
    public float minZoom = 15f;
    public float maxZoom = 90f;
    public float currentZoom;
    public float zoomSpeed = 10f;

    public float rotateSensitivity = 3.5f;

    private void Start()
    {
        currentZoom = Camera.main.fieldOfView;
    }

    void Update()
    {
        // זום ע"י גלגלת
        currentZoom -= Input.GetAxis("Mouse ScrollWheel") * zoomSpeed;
        currentZoom = Mathf.Clamp(currentZoom, minZoom, maxZoom);
        Camera.main.fieldOfView = currentZoom;

        // סיבוב ע"י קליק שמאלי וגרירה
        if (Input.GetMouseButton(0))
        {
            transform.Rotate(new Vector3(Input.GetAxis("Mouse Y") * rotateSensitivity, -Input.GetAxis("Mouse X") * rotateSensitivity, 0));
            transform.rotation = Quaternion.Euler(transform.rotation.eulerAngles.x, transform.rotation.eulerAngles.y, 0);
        }
    }
}