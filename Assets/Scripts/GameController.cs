using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameController : MonoBehaviour
{
    Grid grid;
    float time;

    public float timeSecond;

    private GameState gameState;
    private int score;
    private Slider slider;
    private Text scoreText;
    private Button pauseButton;

    public enum GameState
    {
        Start,
        Pause,
        Over
    }

    public int Score
    {
        get { return score; }
        set
        {
            score = value;
            scoreText.text = score.ToString();
        }
    }

    private void Awake()
    {
        gameState = GameState.Over;
        grid = GameObject.Find("Grid").GetComponent<Grid>();
        slider = GameObject.Find("TimeSlider").GetComponent<Slider>();
        scoreText = GameObject.Find("ScoreText").GetComponent<Text>();
        pauseButton = GameObject.Find("PauseButton").GetComponent<Button>();
    }

    private void Start()
    {
        StartGame();
    }

    private void Update()
    {
        if (gameState != GameState.Start)
        {
            return;
        }
        time += Time.deltaTime;
        if(timeSecond - time <= 0)
        {
            if(grid.IsMovingItem)   //若Grid 正在拖动或下坠操作则等待
            {
                time = timeSecond;
            }
            else
            {
                grid.GenerateNewLine();
                time = 0;
            }
        }
        slider.value = time / timeSecond;
    }

    public void StartGame()
    {
        if(gameState == GameState.Pause)
        {
            
        }
        else
        {
            time = 0;
            grid.Refresh();
        }
        gameState = GameState.Start;
    }

    public void GameOver()
    {
        gameState = GameState.Over;
    }

    public void PauseGame()
    {
        gameState = GameState.Pause;
        pauseButton.Select();
    }

    public void ClickPauseButton()
    {
        if(gameState == GameState.Start)
        {
            PauseGame(); 
        }
        else
        {
            StartGame();
        }
    }
    
}
