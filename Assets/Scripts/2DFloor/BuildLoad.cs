using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BuildLoad : MonoBehaviour
{
    private void Start()
    {
        // 오브젝트를 파괴되지 않도록 설정
        DontDestroyOnLoad(this.gameObject);

    }
}
