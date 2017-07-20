using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReturnToMainMenuAction : ButtonAction
{
	public override void Action()
    {
        GameStateManager manager = GameObject.Find("GameManager").GetComponent<GameStateManager>();
        manager.ChangeGameState(GameStateManager.GameState.MAIN_MENU);
    }
}
