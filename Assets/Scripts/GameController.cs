using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour
{
    Grid grid;
    float time;

    public float timeSecond;

    private GameState gameState;

    public enum GameState
    {
        Start,
        Pause,
        Over
    }

    private void Awake()
    {
        gameState = GameState.Over;
        grid = GameObject.Find("Grid").GetComponent<Grid>();
    }

    private void Start()
    {
        StartGame();
    }

    private void Update()
    {
        time += Time.deltaTime;
        if(timeSecond - time <= 0)
        {
            if(grid.IsMoving)   //若Grid 正在拖动或下坠操作则等待
            {
                time = timeSecond;
            }
            else
            {
                grid.GenerateNewLine();
                time = 0;
            }
        }
    }

    public void StartGame()
    {
        gameState = GameState.Start;
        time = 0;
        grid.Refresh();
    }

    public void GameOver()
    {
        gameState = GameState.Over;
    }
}
