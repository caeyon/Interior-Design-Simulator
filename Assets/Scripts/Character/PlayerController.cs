using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float speed = 7.0f;
    public float collisionResetTime = 2.0f; //�浹 �缳�� �ð�

    private CharacterController controller;
    private Collider lastDoorCollider; //���������� �浹�� ��
    private Collider lastWallCollider; //���������� �浹�� ��
    private bool doorDetected = false; //�� ���� ����
    private float collisionTimer = 0f; //�浹 Ÿ�̸�

    void Start()
    {
        controller = GetComponent<CharacterController>();
    }

    void Update()
    {
        //�÷��̾� �̵�
        MovePlayer();

        //Ÿ�̸� ���� �� �浹 �缳��
        if (collisionTimer > 0f)
        {
            collisionTimer -= Time.deltaTime;
            if (collisionTimer <= 0f)
            {
                ResetCollision();
            }
        }
    }

    void MovePlayer()
    {
        float moveX = Input.GetAxis("Horizontal");
        float moveZ = Input.GetAxis("Vertical");
        Vector3 move = transform.right * moveX + transform.forward * moveZ;

        controller.Move(move * speed * Time.deltaTime);
    }

    void OnControllerColliderHit(ControllerColliderHit hit)
    {
        //���� �浹
        if (hit.collider.CompareTag("DOOR"))
        {
            doorDetected = true;
            lastDoorCollider = hit.collider; //�� �ݶ��̴� ����
            collisionTimer = collisionResetTime; //Ÿ�̸� �ʱ�ȭ
            Physics.IgnoreCollision(controller, hit.collider, true); //���� �浹 ����
            Debug.Log("Door detected, collision ignored.");
        }

        //���� �浹
        if (hit.collider.CompareTag("WALL"))
        {
            if (doorDetected)
            {
                //���� ������ ���¿��� �� ��� ���
                lastWallCollider = hit.collider; //�� �ݶ��̴� ����
                Physics.IgnoreCollision(controller, hit.collider, true); //���� �浹 ����
                Debug.Log("Wall detected with Door, collision ignored.");
            }
            else
            {
                //���� �������� ���� ���¿����� ���� �浹 ����
                Debug.Log("Wall detected, collision not ignored.");
            }
        }
    }

    void ResetCollision()
    {
        //���� �� �浹 �ٽ� Ȱ��ȭ
        if (lastDoorCollider != null)
        {
            Physics.IgnoreCollision(controller, lastDoorCollider, false);
            lastDoorCollider = null;
            Debug.Log("Door collision reset.");
        }

        if (lastWallCollider != null)
        {
            Physics.IgnoreCollision(controller, lastWallCollider, false);
            lastWallCollider = null;
            Debug.Log("Wall collision reset.");
        }

        doorDetected = false; //�� ���� ���� �ʱ�ȭ
    }
}