using UnityEngine;

public class PlayerCamera : MonoBehaviour
{
    //マウス感度
    public float mouseSensitivity = 2.0f;
    //カメラの高さ
    public float standingHeight = 1.8f;
    //カメラ角度
    private float verticalRotation = 0f;
    private float cameraHeight;

    void Start()
    {
        cameraHeight = standingHeight;
    }

    void Update()
    {
        // 右クリックを押しているとき
        if (Input.GetMouseButton(1))
        {
            float mouseX = -Input.GetAxis("Mouse X") * mouseSensitivity; // マウス感度
            float mouseY = -Input.GetAxis("Mouse Y") * mouseSensitivity;

            verticalRotation -= mouseY;
            verticalRotation = Mathf.Clamp(verticalRotation, -90f, 90f);

            transform.Rotate(Vector3.up * mouseX);
            Camera.main.transform.localRotation = Quaternion.Euler(verticalRotation, 0, 0);
        }
    }
}
