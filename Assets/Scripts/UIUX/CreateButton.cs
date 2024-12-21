using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;

public class CreateButton : MonoBehaviour
{
    public GameObject uiCanvas;
    public TMP_InputField inputWidth;
    public TMP_InputField inputHeight;
    public GameObject targetObject;

    float width;
    float height;

    private void Start()
    {
        uiCanvas.gameObject.SetActive(false);

        inputWidth.onEndEdit.AddListener(HandleInputWidth);
        inputHeight.onEndEdit.AddListener(HandleInputHeight);
    }

    private void OnDestroy()
    {
        inputWidth.onEndEdit.RemoveListener(HandleInputWidth);
        inputHeight.onEndEdit.RemoveListener(HandleInputHeight);
    }

    public void ForCreateButton()
    {
        float wy = targetObject.transform.localScale.y;
        float wz = targetObject.transform.localScale.z;

        Debug.Log("Now Width is" + width);
        targetObject.transform.localScale = new Vector3(width, wy, wz);

        float hx = targetObject.transform.localScale.x;
        float hy = targetObject.transform.localScale.y;

        Debug.Log("Now height is" + height);
        targetObject.transform.localScale = new Vector3(hx, hy, height);

        uiCanvas.gameObject.SetActive(false);
    }

    //�ٴ� ��ư �̺�Ʈ
    public void ButtonEvent1()
    {
        uiCanvas.gameObject.SetActive(true);
    }

    //�� ��ư �̺�Ʈ
    public void ButtonEvent2()
    {
        transform.GetComponent<ObjectSpawner>().SpawnWall();
    }

    //â�� ��ư �̺�Ʈ
    public void ButtonEvent3()
    {
        transform.GetComponent<ObjectSpawner>().SpawnWindow();
    }

    //�� ��ư �̺�Ʈ
    public void ButtonEvent4()
    {
        transform.GetComponent<ObjectSpawner>().SpawnDoor();
    }

    void HandleInputWidth(string inputWitdth)
    {
        width = float.Parse(inputWitdth);
    }

    void HandleInputHeight(string inputHeight)
    {
        height  = float.Parse(inputHeight);
    }
}
