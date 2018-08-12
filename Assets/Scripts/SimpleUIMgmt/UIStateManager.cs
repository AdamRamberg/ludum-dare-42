using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using ScriptableObjectVariables;

public class UIStateManager : MonoBehaviour
{
    public StringVariable uiState;
    private Dictionary<string, float> accumlatedUIStateTimes = new Dictionary<string, float>();

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
    }

    void Update()
    {
        if (!accumlatedUIStateTimes.ContainsKey(uiState.Value))
        {
            accumlatedUIStateTimes.Add(uiState.Value, 0f);
        }
        else
        {
            accumlatedUIStateTimes[uiState.Value] += Time.deltaTime;
        }
    }

    public static UIStateManager instance = null; // Static ref

    // Get accumalated state time. Returns -1f if state doesn't exist. 
    public float GetAccumlatedStateTime(string state)
    {
        return accumlatedUIStateTimes.ContainsKey(state) ? accumlatedUIStateTimes[state] : -1f;
    }

    // Reset accumalted state time for a particular state
    public void ResetGameStateTime(string state)
    {
        if (accumlatedUIStateTimes.ContainsKey(state))
            accumlatedUIStateTimes[state] = 0f;
    }
}
