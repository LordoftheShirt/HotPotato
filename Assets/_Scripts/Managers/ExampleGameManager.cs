using System;
using UnityEngine;

// Keeps tracks of all of the stages within the game. A State Machine is the more complex alternative for better scaling.
public class ExampleGameManager : Singleton<ExampleGameManager>
{
    public static event Action<GameState> OnBeforeStateChanged;
    public static event Action<GameState> OnAfterStateChanged;

    public GameState State { get; private set; }

    //private void Start() =>

    public void ChangeState(GameState newState)
    {
        if (State == newState) return;

        OnBeforeStateChanged?.Invoke(newState);

        switch (newState) 
        {
            case GameState.Starting:
                break;
            case GameState.SpawningHeroes:
                break;
            case GameState.SpawningEnemies:
                break;
            case GameState.HeroTurn:
                break;
            case GameState.EnemyTurn:
                break;
            case GameState.Win:
                break;
            case GameState.Lose:
                break;
        }

        OnAfterStateChanged?.Invoke(newState);

        Debug.Log($"New state: {newState}");
    }

    private void HandleStarting()
    {
        // Do some start set up.
        
        // Then enter the next state.
        ChangeState(GameState.SpawningHeroes);
    }
}



// An example of different gameState stages, organized via Enums.
[Serializable]
public enum GameState
{
    Starting = 0,
    SpawningHeroes = 1,
    SpawningEnemies = 2,
    HeroTurn = 3,
    EnemyTurn = 4,
    Win = 5,
    Lose = 6,
}
