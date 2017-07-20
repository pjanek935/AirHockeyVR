using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScoreManager : MonoBehaviour {

    public TextMesh scoreMesh;

    private int p1Score = 0;
    private int p2Score = 0;

	// Use this for initialization
	void Start () {
        SetVisible(false);
	}
	
	// Update is called once per frame
	void Update () {
        if (GetComponent<GameStateManager>().gameState == GameStateManager.GameState.TRAINING ||
            GetComponent<GameStateManager>().gameState == GameStateManager.GameState.BALL_ATTACK)
            return;

        if (p1Score >= 6)
        {
            if (p1Score - p2Score < 2)
                return;
            ResetScore();
            GetComponent<GameStateManager>().GoToNextStage();
        }
        if (p2Score >= 6)
        {
            if (p2Score - p1Score < 2)
                return;
            ResetScore();
            GetComponent<GameStateManager>().Lost();
        }
	}

    private void UpdateScore()
    {
        scoreMesh.text = p1Score + ":" + p2Score;
    }

    public void P1Scored()
    {
        p1Score++;
        UpdateScore();
    }

    public void P2Scored()
    {
        p2Score++;
        UpdateScore();
    }
    
    public void ResetScore()
    {
        p1Score = 0;
        p2Score = 0;
        UpdateScore();
    }

    public void SetVisible(bool visible)
    {
        scoreMesh.gameObject.SetActive(visible);
    }
}
