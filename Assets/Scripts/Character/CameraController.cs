using UnityEngine;

public class CameraController : MonoBehaviour
{
    public Transform playerBody; //�÷��̾��� Transform
    public float mouseSensitivity = 100.0f;
    public float rotationSpeed = 7.0f; //���콺 ȸ�� �ӵ�

    private float xRotation = 0.0f; //ī�޶� ���� ȸ�� ����

    void Start()
    {
        //Cursor.lockState = CursorLockMode.Locked; //���콺 Ŀ�� �����
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
        //���콺 �Է�
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f); //���� 90�� ������ ����

        //ī�޶� ���� ȸ��
        transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);

        //�÷��̾� �¿� ȸ��
        playerBody.Rotate(Vector3.up * mouseX * rotationSpeed);
    }
}

