using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public GameState gameState;
    public GameObject characterPrefab;

    public void OnLandedOnMattress(Character character)
    {
        if (gameState.mainState == GameState.MainState.GamePlaying)
        {
            character.Destroy();
        }
    }
}
