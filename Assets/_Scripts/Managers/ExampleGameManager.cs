using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Audio;

// Keeps tracks of all of the stages within the game. A State Machine is the more complex alternative for better scaling.
public class ExampleGameManager : Singleton<ExampleGameManager>
{
    public static event Action<GameState> OnBeforeStateChanged;
    public static event Action<GameState> OnAfterStateChanged;

    [SerializeField] private GameStarter gameStarter;

    public GameState State { get; private set; }

    private float firstLoopDelay = 2.6f;
    private bool isFirstLoop = false, isAfterChorus = false;
    private int loopCount;

    //private void Start() =>

    public void ChangeState(GameState newState)
    {
        //Debug.Log("New state: " + newState);
        //Debug.Log("Old state: " + State);
        if (State == newState) return;

        OnBeforeStateChanged?.Invoke(newState);

        switch (newState) 
        {
            case GameState.Lobby:
                gameStarter.Reset();
                break;
            case GameState.Starting:
                HandleStarting();
                break;
            case GameState.LoopingChase:
                StartCoroutine(PlayLoopXTimes());
                break;
            case GameState.FinalCountDown:
                StartCoroutine(ChorusBuildUp());
                break;
            case GameState.PlayerDeath:
                StartCoroutine(HotPotatoExplodeSong());
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
        AudioSystem.Instance.StopMusic();
        Debug.Log("PlayLinkin!");
        isFirstLoop = true;
        ChangeState(GameState.LoopingChase);
    }
    IEnumerator PlayLoopXTimes()
    {
        if (isFirstLoop) 
        {
            AudioSystem.Instance.PlayMusic(AudioSystem.Instance.songs[1], firstLoopDelay);
            isFirstLoop = false;
            loopCount = UnityEngine.Random.Range(0, 4);
            Debug.Log("Loop count set: " + loopCount);
            yield return new WaitForSeconds(AudioSystem.Instance.GetMusicSource().clip.length - firstLoopDelay);
        }

        if (isAfterChorus)
        {
            AudioSystem.Instance.PlayMusic(AudioSystem.Instance.songs[1], 5.5f);
            isAfterChorus = false;
            loopCount = UnityEngine.Random.Range(0, 4);
            Debug.Log("Loop count set: " + loopCount);
            yield return new WaitForSeconds(AudioSystem.Instance.GetMusicSource().clip.length - 5.5f);
        }



        for (int i = 0; i < loopCount; i++)
        {
            Debug.Log("Loop "+ i);
            AudioSystem.Instance.PlayMusic(AudioSystem.Instance.songs[1]);
            yield return new WaitForSeconds(AudioSystem.Instance.GetMusicSource().clip.length);
        }
        ChangeState(GameState.FinalCountDown);
    }

    IEnumerator ChorusBuildUp()
    {
        AudioSystem.Instance.PlayMusic(AudioSystem.Instance.songs[2]);
        yield return new WaitForSeconds(AudioSystem.Instance.GetMusicSource().clip.length);
        ChangeState(GameState.PlayerDeath);
    }

    IEnumerator HotPotatoExplodeSong()
    {
        AudioSystem.Instance.PlayMusic(AudioSystem.Instance.songs[3]);
        yield return new WaitForSeconds(AudioSystem.Instance.GetMusicSource().clip.length - 0.15f);
        isAfterChorus = true;
        ChangeState(GameState.LoopingChase);
    }
}

// An example of different gameState stages, organized via Enums.
[Serializable]
public enum GameState
{
    Lobby = 0,
    Starting = 1,
    LoopingChase = 2,
    FinalCountDown = 3,
    PlayerDeath = 4,
    Win = 5,
    Lose = 6,
}
