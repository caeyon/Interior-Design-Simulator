using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BuildLoad : MonoBehaviour
{
    private void Start()
    {
        // ������Ʈ�� �ı����� �ʵ��� ����
        DontDestroyOnLoad(this.gameObject);

    }
}
