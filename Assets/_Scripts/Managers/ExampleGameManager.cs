using System;
using System.Collections;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.InputSystem;

// Keeps tracks of all of the stages within the game. A State Machine is the more complex alternative for better scaling.
public class ExampleGameManager : Singleton<ExampleGameManager>
{

    [SerializeField] PlayerInputManager playerInputManager;
    public static event Action<GameState> OnBeforeStateChanged;
    public static event Action<GameState> OnAfterStateChanged;

    [SerializeField] private GameStarter gameStarter;
    [SerializeField] private GameObject potato, fireworks;
    public Transform[] players = new Transform[4];

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
        State = newState;
        OnBeforeStateChanged?.Invoke(newState);

        switch (newState) 
        {
            case GameState.Lobby:
                ReactivePlayers();
                gameStarter.Reset();
                break;
            case GameState.Starting:
                HandleStarting();
                break;
            case GameState.LoopingChase:
                fireworks.SetActive(false);
                SpawnPotato();
                break;
            case GameState.FinalCountDown:
                StartCoroutine(ChorusBuildUp());
                break;
            case GameState.PlayerDeath:
                fireworks.SetActive(true);
                StartCoroutine(HotPotatoExplodeSong());
                break;
            case GameState.Win:
                break;
            case GameState.Lose:
                break;
        }

        OnAfterStateChanged?.Invoke(newState);

        //Debug.Log($"New state: {newState}");
    }

    private void HandleStarting()
    {
        if (players[3] != null)
        {
            playerInputManager.DisableJoining();
        }
        AudioSystem.Instance.StopMusic();
        isFirstLoop = true;
        ChangeState(GameState.LoopingChase);
    }
    IEnumerator PlayLoopXTimes()
    {
        if (isFirstLoop) 
        {
            AudioSystem.Instance.PlayMusic(AudioSystem.Instance.songs[1], firstLoopDelay);
            isFirstLoop = false;
            loopCount = UnityEngine.Random.Range(1, 4);
            //Debug.Log("Loop count set: " + loopCount);
            yield return new WaitForSeconds(AudioSystem.Instance.GetMusicSource().clip.length - firstLoopDelay);
        }

        if (isAfterChorus)
        {
            AudioSystem.Instance.PlayMusic(AudioSystem.Instance.songs[1], 5.5f);
            isAfterChorus = false;
            loopCount = UnityEngine.Random.Range(0, 4);
            //Debug.Log("Loop count set: " + loopCount);
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
        Debug.Log("Dance party!");
        AudioSystem.Instance.PlayMusic(AudioSystem.Instance.songs[3]);
        Debug.Log("Wait length: " +AudioSystem.Instance.GetMusicSource().clip.length);
        yield return new WaitForSeconds(AudioSystem.Instance.GetMusicSource().clip.length - 0.15f);
        CheckPlayersRemaining();
    }

    private void SpawnPotato()
    {
        GameObject player;
        int randomPlayer = 0;
        Debug.Log("SpawnPotato");
        while (true)
        {
            randomPlayer = UnityEngine.Random.Range(0, 4);
            player = ExampleGameManager.Instance.players[randomPlayer].gameObject;
            Debug.Log("RandomCheck1:" + randomPlayer);
            if (player.activeInHierarchy)
            {
                Debug.Log("RandomCheck2:" + randomPlayer);
                Debug.Log("Spawn potato at " + player.name);
                Instantiate(potato, player.transform.position, Quaternion.identity);
                StartCoroutine(PlayLoopXTimes());
                break;
            }
            else
            {
                //Debug.Log("Player was disabled: " + ExampleGameManager.Instance.players[randomPlayer].gameObject.name);
            }
        }
    }

    private void CheckPlayersRemaining()
    {
        int playerCount = 0;
        for(int i = 0; ExampleGameManager.Instance.players.Length > i; i++)
        {
            if (ExampleGameManager.Instance.players[i].gameObject.activeInHierarchy)
            {
                playerCount++;
            }
        }
        //Debug.Log("CheckPlayersRemaing playercount: " +  playerCount);
        if (playerCount == 1) 
        {
            // Win!
            Debug.Log("Restart level");
            ChangeState(GameState.Lobby);
        }
        else
        { 
            // Still players left.
            isAfterChorus = true;
            ChangeState(GameState.LoopingChase);
        }
    }

    private void ReactivePlayers()
    {
        //Debug.Log("Reactivate players start");
        for (int i = 0; ExampleGameManager.Instance.players.Length > i; i++)
        {
            //Debug.Log("Player reactivated: " + i);
            ExampleGameManager.Instance.players[i].gameObject.SetActive(true);
        }
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
