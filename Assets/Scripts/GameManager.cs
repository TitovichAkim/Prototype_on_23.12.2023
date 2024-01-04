using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public enum GameState
    {
        MainMenu,
        LevelRedactor,
        Game
    }
    [SerializeField]private static GameState _currentGameState;
    public static GameState currentGameState
    {
        get
        {
            return (_currentGameState);
        }
        set
        {
            _currentGameState = value;
        }
    }
    public void Start ()
    {
        currentGameState = GameState.LevelRedactor;
    }
}
