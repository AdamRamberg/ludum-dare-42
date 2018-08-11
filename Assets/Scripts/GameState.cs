using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Game State")]
public class GameState : ScriptableObject
{
    public enum MainState
    {
        Start,
        GamePlaying,
        GameOver,

    }
    public MainState mainState = MainState.GamePlaying;
}
