using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.SceneManagement;
using System;

public class GameManager : MonoSingleton<GameManager>
{
    [Header("Actions")]
    public static Action onGamePaused;
    public static Action onGameResumed;

    [Header("for debug")]
    [SerializeField] PlayerController playerController;
    public PlayerController PlayerController
    {
        get {
            if (!playerController) playerController = FindObjectOfType<PlayerController>();
            return playerController;
        }
    }


    // Start is called before the first frame update
    void Start()
    {
        Application.targetFrameRate = 60;
        // SetGameState(GameState.MENU); // GameState 정의가 없어 주석처리
    }

    // public void StartGame()             => SetGameState(GameState.GAME); // GameState 정의가 없어 주석처리
    // public void StartWeaponSelection()  => SetGameState(GameState.WEAPONSELECTION); // GameState 정의가 없어 주석처리
    // public void StartShop()             => SetGameState(GameState.SHOP); // GameState 정의가 없어 주석처리

    // public void SetGameState(GameState gameState)
    // {
    //     IEnumerable<IGameStateListener> gameStateListeners = 
    //         FindObjectsByType<MonoBehaviour>(FindObjectsSortMode.None)
    //         .OfType<IGameStateListener>();

    //     foreach (IGameStateListener gameStateListener in gameStateListeners)
    //         gameStateListener.GameStateChangedCallback(gameState);
    // }

    public void WaveCompletedCallback()
    {
        // if(Player.instance.HasLeveledUp() || WaveTransitionManager.instance.HasCollectedChest())
        // {
        //     SetGameState(GameState.WAVETRANSITION);
        // }
        // else
        // {
        //     SetGameState(GameState.SHOP);
        // }
    }

    public void ManageGameover()
    {
        SceneManager.LoadScene(0);
    }

    public void PauseButtonCallback()
    {
        Time.timeScale = 0;
        onGamePaused?.Invoke();
    }

    public void ResumeButtonCallback()
    {
        Time.timeScale = 1;
        onGameResumed?.Invoke();
    }

    public void RestartFromPause()
    {
        Time.timeScale = 1;
        ManageGameover();
    }
}

// public interface IGameStateListener
// {
//     void GameStateChangedCallback(GameState gameState);
// }