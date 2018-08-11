using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public GameState gameState;
    public UIManager uiManager;

    public void CharacterDied()
    {
        gameState.mainState = GameState.MainState.GameOver;
        uiManager.SetState(GameState.MainState.GameOver.ToString());
    }

}
