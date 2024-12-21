using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float speed = 7.0f;
    public float collisionResetTime = 2.0f; //충돌 재설정 시간

    private CharacterController controller;
    private Collider lastDoorCollider; //마지막으로 충돌한 문
    private Collider lastWallCollider; //마지막으로 충돌한 벽
    private bool doorDetected = false; //문 감지 상태
    private float collisionTimer = 0f; //충돌 타이머

    void Start()
    {
        controller = GetComponent<CharacterController>();
    }

    void Update()
    {
        //플레이어 이동
        MovePlayer();

        //타이머 감소 및 충돌 재설정
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
        //문과 충돌
        if (hit.collider.CompareTag("DOOR"))
        {
            doorDetected = true;
            lastDoorCollider = hit.collider; //문 콜라이더 저장
            collisionTimer = collisionResetTime; //타이머 초기화
            Physics.IgnoreCollision(controller, hit.collider, true); //문과 충돌 무시
            Debug.Log("Door detected, collision ignored.");
        }

        //벽과 충돌
        if (hit.collider.CompareTag("WALL"))
        {
            if (doorDetected)
            {
                //문이 감지된 상태에서 벽 통과 허용
                lastWallCollider = hit.collider; //벽 콜라이더 저장
                Physics.IgnoreCollision(controller, hit.collider, true); //벽과 충돌 무시
                Debug.Log("Wall detected with Door, collision ignored.");
            }
            else
            {
                //문이 감지되지 않은 상태에서는 벽과 충돌 유지
                Debug.Log("Wall detected, collision not ignored.");
            }
        }
    }

    void ResetCollision()
    {
        //문과 벽 충돌 다시 활성화
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

        doorDetected = false; //문 감지 상태 초기화
    }
}