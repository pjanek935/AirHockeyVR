using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TournamentAction : ButtonAction
{
    public override void Action()
    {
        GameStateManager manager = GameObject.Find("GameManager").GetComponent<GameStateManager>();
        manager.ChangeGameState(GameStateManager.GameState.GAME);
    }
}
