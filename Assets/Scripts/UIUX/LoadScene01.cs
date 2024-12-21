using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LoadScene01 : MonoBehaviour
{
    public void SceneChange01()
    {
        SceneManager.LoadScene("2DFloor");

    }

    public void SceneChange10()
    {
        SceneManager.LoadScene("StartScene");
    }

    public void SceneChange12()
    {
        SceneManager.LoadScene("Scene_UI_Size");
    }
}
