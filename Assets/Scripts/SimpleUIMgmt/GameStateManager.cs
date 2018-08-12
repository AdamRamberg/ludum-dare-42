using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GameStateManager : MonoBehaviour
{

    public enum GameState
    {
        None,
        State1,
        State2,
        State3,
        State4,
        State5,
        State6,
        State7,
        State8,
        State9,
        State10
    }

    [System.Serializable]
    public class GameStateNameEntry
    {
        public GameState gameState;
        public string name;
    }

    private Dictionary<GameState, float> accumlatedStateTime = new Dictionary<GameState, float>();
    private GameState previousGameState = GameState.None;

    public string GetGameStateName(GameState state)
    {
        if (gameStateNames == null)
            throw new UnityException("No game states defined.");

        foreach (GameStateNameEntry entry in gameStateNames)
        {
            if (entry.gameState == state)
            {
                return entry.name;
            }
        }

        throw new UnityException("Could not find name for game state: " + state.ToString());
    }

    public GameState GetGameStateFromName(string name)
    {
        if (gameStateNames == null)
            throw new UnityException("No game states defined.");

        foreach (GameStateNameEntry entry in gameStateNames)
        {
            if (entry.name == name)
            {
                return entry.gameState;
            }
        }

        throw new UnityException("Could not find game state for name: " + name);
    }

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != this)
        {
            Destroy(gameObject);
        }

        CurrentGameState.Value = GetGameStateFromName(startGameState);

        foreach (GameStateNameEntry gse in gameStateNames)
        {
            accumlatedStateTime.Add(gse.gameState, 0f);
        }
    }

    void Update()
    {
        // Update accumlated time
        accumlatedStateTime[CurrentGameState.Value] += Time.deltaTime;
    }

    // Publicly available varibles, properties and methods
    public static GameStateManager instance = null; // Static ref
    public GameStateNameEntry[] gameStateNames;
    public string startGameState;
    public ObservableProp<GameState> CurrentGameState = new ObservableProp<GameState>(GameState.State1);
    public GameState PreviousGameState { get { return previousGameState; } }

    // Use this method to change game state
    public void ChangeGameState(string newState)
    {
        previousGameState = CurrentGameState.Value;
        CurrentGameState.Value = GameStateManager.instance.GetGameStateFromName(newState);
    }

    // Get accumalated state time. Returns -1f if state doesn't exist. 
    public float GetAccumlatedStateTime(GameState state)
    {
        return accumlatedStateTime.ContainsKey(state) ? accumlatedStateTime[state] : -1f;
    }

    // Reset currently active game state's time
    public void ResetCurrentGameStateTime()
    {
        ResetGameStateTime(CurrentGameState.Value);
    }

    // Reset accumalted state time for a particular state
    public void ResetGameStateTime(GameState state)
    {
        if (accumlatedStateTime.ContainsKey(state))
            accumlatedStateTime[state] = 0f;
    }
}
