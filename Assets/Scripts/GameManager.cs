using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager gm;
    PlayerController player;

    private void Awake()
    {
        if (gm == null)
        {
            gm = this;
        }
    }

    public enum GameState
    {
        Ready, Run, GameOver
    }

    public GameState gState;

    private void Start()
    {
        gState = GameState.Ready;
        StartCoroutine(ReadyToStart());
    }

    IEnumerator ReadyToStart()
    {
        yield return new WaitForSeconds(1.0f);
        gState = GameState.Run;
    }
}
