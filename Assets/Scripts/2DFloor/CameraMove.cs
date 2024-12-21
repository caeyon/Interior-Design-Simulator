using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class CameraMove : MonoBehaviour
{
    public float moveSpeed = 3.0f;
    public float wheelspeed = 3.0f;

    void Update()
    {
        // WASD로 카메라 이동
        if (Input.GetKey(KeyCode.D)) transform.Translate(Vector2.right * moveSpeed * Time.deltaTime);
        if (Input.GetKey(KeyCode.A)) transform.Translate(-Vector2.right * moveSpeed * Time.deltaTime);
        if (Input.GetKey(KeyCode.W)) transform.Translate(Vector2.up * moveSpeed * Time.deltaTime);
        if (Input.GetKey(KeyCode.S)) transform.Translate(-Vector2.up * moveSpeed * Time.deltaTime);

        // 마우스 휠로 줌
        float scrollInput = Input.GetAxis("Mouse ScrollWheel");
        if (scrollInput != 0)
        {
            Camera.main.orthographicSize -= scrollInput * wheelspeed;
            Camera.main.orthographicSize = Mathf.Clamp(Camera.main.orthographicSize, 10.0f, 50.0f); // 줌 범위 설정
        }
    }
}
