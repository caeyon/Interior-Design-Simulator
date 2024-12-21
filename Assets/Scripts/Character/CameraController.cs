using UnityEngine;

public class CameraController : MonoBehaviour
{
    public Transform playerBody; //플레이어의 Transform
    public float mouseSensitivity = 100.0f;
    public float rotationSpeed = 7.0f; //마우스 회전 속도

    private float xRotation = 0.0f; //카메라 상하 회전 각도

    void Start()
    {
        //Cursor.lockState = CursorLockMode.Locked; //마우스 커서 숨기기
    }

    void Update()
    {
        if (GameManager.gm.gState != GameManager.GameState.Run)
        {
            return;
        }

        MouseLook();
    }

    void MouseLook()
    {
        //마우스 입력
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f); //상하 90도 각도로 제한

        //카메라 상하 회전
        transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);

        //플레이어 좌우 회전
        playerBody.Rotate(Vector3.up * mouseX * rotationSpeed);
    }
}

