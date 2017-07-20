using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialText : MonoBehaviour {

    public BallScript ball;

    private float timer = 0;
    private float limiter = 5;
    private bool textVisible = false;

	// Use this for initialization
	void Start () {
        HideText();
        timer = -5;
	}
	
	// Update is called once per frame
	void Update () {

        if (GameObject.Find("GameManager").GetComponent<GameStateManager>().gameState
            == GameStateManager.GameState.BALL_ATTACK)
            return;

        if (ball.ballState == BallScript.BallState.CATCHED_BY_P1)
        {
            timer += Time.deltaTime;
            if (timer >= limiter && !textVisible)
                ShowText();
        }
        else if (textVisible)
        {
            timer = 0;
            HideText();
        }
	}

    public void ShowText()
    {
        textVisible = true;
        StartCoroutine("ShowTextCoroutine");
    }

    public void HideText()
    {
        textVisible = false;
        StartCoroutine("HideTextCoroutine");
    }

    private IEnumerator ShowTextCoroutine()
    {
        Color color = GetComponent<TextMesh>().color;
        while (color.a < 1)
        {
            color.a += 0.01f;
            GetComponent<TextMesh>().color = color;
            yield return null;
        }
    }

    private IEnumerator HideTextCoroutine()
    {
        Color color = GetComponent<TextMesh>().color;
        while (color.a > 0)
        {
            color.a -= 0.02f;
            GetComponent<TextMesh>().color = color;
            yield return null;
        }
    }
}
