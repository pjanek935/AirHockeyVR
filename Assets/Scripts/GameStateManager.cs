using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameStateManager : MonoBehaviour {

    public GameState gameState = GameState.MAIN_MENU;
    public GameObject stage;
    public GameObject menu;
    public GameObject buildings;
    public GameObject paddle1, paddle2;
    public GameObject ball;
    public GameObject stageText;
    public GameObject backWall;
    public GameObject ballSpawner;
    public TutorialText tutorialText;

    private ScoreManager score;
    private int currentStage = 1;

    public enum GameState { MAIN_MENU, GAME, TRAINING, BALL_ATTACK};

	// Use this for initialization
	void Start () {
        score = GetComponent<ScoreManager>();
        ball.GetComponent<BallScript>().SetActive(false);
        paddle1.GetComponent<Move>().controlMode = Move.ControlMode.NONE;
        paddle2.GetComponent<Move>().controlMode = Move.ControlMode.NONE;
    }
	
	// Update is called once per frame
	void Update () {
       
    }

    public void ChangeGameState(GameState newGamState)
    {
        switch (newGamState)
        {
            case GameState.MAIN_MENU:
                StartCoroutine("SwitchToMainMenuState");
                break;

            case GameState.GAME:
                StartCoroutine("SwitchToGameState");
                break;

            case GameState.TRAINING:
                StartCoroutine("SwitchToTrainingState");
                break;

            case GameState.BALL_ATTACK:
                StartCoroutine("SwitchToBallAttackState");
                break;

            default:
                break;
        }
    }

    public void GoToNextStage()
    {
        currentStage++;
        if (currentStage >= 7)
        {
            Won();
        }
        else
        {
            stageText.GetComponent<TextMesh>().text = "STAGE " + currentStage;
            paddle2.GetComponent<AI>().NextAI();
            StartCoroutine("NextStage");
        }
    }

    private void Won()
    {
        stageText.GetComponent<TextMesh>().text = "YOU WON!";
        StartCoroutine("EndGameCoroutine");
    }

    public void Lost()
    {
        stageText.GetComponent<TextMesh>().text = "YOU LOST!";
        StartCoroutine("EndGameCoroutine");
    }

    IEnumerator EndGameCoroutine()
    {
        ball.GetComponent<BallScript>().ballState = BallScript.BallState.CATCHED_BY_P1;
        ball.GetComponent<BallScript>().SetActive(false);
        paddle1.GetComponent<Move>().controlMode = Move.ControlMode.NONE;
        paddle2.GetComponent<Move>().controlMode = Move.ControlMode.NONE;
        stageText.SetActive(true);
        tutorialText.HideText();

        while (stageText.GetComponent<TextMesh>().color.a < 1)
        {
            Color color = stageText.GetComponent<TextMesh>().color;
            color.a += 0.02f;
            stageText.GetComponent<TextMesh>().color = color;
            yield return null;
        }
        float timer = 0;
        while (timer < 60)
        {
            timer++;
            yield return null;
        }

        StartCoroutine("SwitchToMainMenuState");
    }

    

    IEnumerator NextStage()
    {
        ball.GetComponent<BallScript>().ballState = BallScript.BallState.CATCHED_BY_P1;
        ball.GetComponent<BallScript>().SetActive(false);
        paddle1.GetComponent<Move>().controlMode = Move.ControlMode.NONE;
        paddle2.GetComponent<Move>().controlMode = Move.ControlMode.NONE;
        stageText.SetActive(true);
        tutorialText.HideText();

        while (stageText.GetComponent<TextMesh>().color.a < 1)
        {
            Color color = stageText.GetComponent<TextMesh>().color;
            color.a += 0.02f;
            stageText.GetComponent<TextMesh>().color = color;
            yield return null;
        }
        float timer = 0;
        while (timer < 30)
        {
            timer++;
            yield return null;
        }

        paddle1.GetComponent<Move>().controlMode = Move.ControlMode.MOUSE;
        while (stageText.GetComponent<TextMesh>().color.a > 0)
        {
            Color color = stageText.GetComponent<TextMesh>().color;
            color.a -= 0.02f;
            stageText.GetComponent<TextMesh>().color = color;
            yield return null;
        }

        stageText.SetActive(false);

        gameState = GameState.GAME;
        score.ResetScore();
        score.SetVisible(true);
        ball.GetComponent<BallScript>().SetActive(true);
        paddle2.GetComponent<Move>().controlMode = Move.ControlMode.NEURAL;
    }

    IEnumerator SwitchToMainMenuState()
    {
        stageText.SetActive(false);
        score.SetVisible(false);
        ballSpawner.SetActive(false);
        ball.GetComponent<BallScript>().SetActive(false);
        paddle1.GetComponent<Move>().controlMode = Move.ControlMode.NONE;
        paddle2.GetComponent<Move>().controlMode = Move.ControlMode.NONE;
        StartCoroutine("MoveBuildingsDown");
        tutorialText.HideText();

        while (menu.transform.position.y > 0)
        {
            stage.transform.position -= new Vector3(0, 1 * Time.deltaTime, 0);
            menu.transform.position -= new Vector3(0, 5 * Time.deltaTime, 0);
            yield return null;
        }
        gameState = GameState.MAIN_MENU;
    }

    IEnumerator SwitchToGameState()
    {
        StartCoroutine("MoveBuildingsUp");
        stageText.GetComponent<TextMesh>().text = "STAGE 1";
        paddle1.transform.localPosition = new Vector3(0, 0, paddle1.transform.position.z);
        tutorialText.HideText();
        while (stage.transform.position.y < 0)
        {
            stage.transform.position += new Vector3(0, 1 * Time.deltaTime, 0);
            menu.transform.position += new Vector3(0, 5 * Time.deltaTime, 0);
            yield return null;
        }

        stageText.SetActive(true);
        
        float timer = 0;
        while (timer < 60)
        {
            timer++;
            yield return null;
        }
        paddle1.GetComponent<Move>().controlMode = Move.ControlMode.MOUSE;

        while (stageText.GetComponent<TextMesh>().color.a > 0)
        {
            Color color = stageText.GetComponent<TextMesh>().color;
            color.a -= 0.02f;
            stageText.GetComponent<TextMesh>().color = color;
            yield return null;
        }
        stageText.SetActive(false);

        gameState = GameState.GAME;
        score.ResetScore();
        score.SetVisible(true);
        ball.GetComponent<BallScript>().SetActive(true);
        ball.GetComponent<BallScript>().ballState = BallScript.BallState.CATCHED_BY_P1;
        paddle2.SetActive(true);
        paddle2.GetComponent<Move>().controlMode = Move.ControlMode.NEURAL;
        backWall.SetActive(false);
    }

    IEnumerator SwitchToTrainingState()
    {
        StartCoroutine("MoveBuildingsUp");
        paddle2.SetActive(false);
        stageText.GetComponent<TextMesh>().text = "TRAINING";
        tutorialText.HideText();
        while (stage.transform.position.y < 0)
        {
            stage.transform.position += new Vector3(0, 1 * Time.deltaTime, 0);
            menu.transform.position += new Vector3(0, 5 * Time.deltaTime, 0);
            yield return null;
        }

        stageText.SetActive(true);
        float timer = 0;
        while (timer < 60)
        {
            timer++;
            yield return null;
        }
        while (stageText.GetComponent<TextMesh>().color.a > 0)
        {
            Color color = stageText.GetComponent<TextMesh>().color;
            color.a -= 0.02f;
            stageText.GetComponent<TextMesh>().color = color;
            yield return null;
        }
        stageText.SetActive(false);

        gameState = GameState.TRAINING;
        score.ResetScore();
        score.SetVisible(false);
        ball.GetComponent<BallScript>().SetActive(true);
        paddle1.GetComponent<Move>().controlMode = Move.ControlMode.MOUSE;
        backWall.SetActive(true);
    }

    IEnumerator SwitchToBallAttackState()
    {
        StartCoroutine("MoveBuildingsUp");
        paddle2.SetActive(false);
        stageText.GetComponent<TextMesh>().text = "BALL ATTACK";
        tutorialText.HideText();
        while (stage.transform.position.y < 0)
        {
            stage.transform.position += new Vector3(0, 1 * Time.deltaTime, 0);
            menu.transform.position += new Vector3(0, 5 * Time.deltaTime, 0);
            yield return null;
        }

        stageText.SetActive(true);
        float timer = 0;
        while (timer < 60)
        {
            timer++;
            yield return null;
        }
        while (stageText.GetComponent<TextMesh>().color.a > 0)
        {
            Color color = stageText.GetComponent<TextMesh>().color;
            color.a -= 0.02f;
            stageText.GetComponent<TextMesh>().color = color;
            yield return null;
        }
        stageText.SetActive(false);

        gameState = GameState.BALL_ATTACK;
        score.ResetScore();
        score.SetVisible(true);
        ball.GetComponent<BallScript>().SetActive(false);
        paddle1.GetComponent<Move>().controlMode = Move.ControlMode.MOUSE;
        backWall.SetActive(false);
        ballSpawner.SetActive(true);
    }



    IEnumerator MoveBuildingsUp()
    {
        while (buildings.transform.localPosition.y < -1)
        {
            buildings.transform.localPosition += new Vector3(0, 10 * Time.deltaTime, 0);
            yield return null;
        }
    }

    IEnumerator MoveBuildingsDown()
    {

        while (buildings.transform.localPosition.y > -20)
        {
            buildings.transform.localPosition += new Vector3(0, -10 * Time.deltaTime, 0);
            yield return null;
        }
    }

}
