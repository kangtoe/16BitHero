using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WaveManager : MonoBehaviour
{
    [Header("UIs")]
    [SerializeField] Text waveText;
    [SerializeField] Text timerText;
    
    [Header("Settings")]
    [SerializeField] int waveDuration = 10;
    [SerializeField] int leftWaveDuration;
    [SerializeField] int currentWave = 1;    
    
    void Start()
    {
        leftWaveDuration = waveDuration;  
        UpdateTimerUI();      
        StartCoroutine(StartWave());
    }

    void UpdateTimerUI()
    {
        timerText.text = string.Format("{0:D2}:{1:D2}", leftWaveDuration/60, leftWaveDuration%60);
    }

    IEnumerator StartWave()
    {
        waveText.text = "Wave " + currentWave;

        while (true)
        {            
            yield return new WaitForSeconds(1f);
            leftWaveDuration--;
            UpdateTimerUI();
            if(leftWaveDuration <= 0) break;
        }

        // currentWave++;
        // leftWaveDuration = waveDuration;
        // StartCoroutine(StartWave());
    } 

    // public void GameStateChangedCallback(GameState gameState)
    // {
    //     switch(gameState)
    //     {
    //         case GameState.GAME:
    //             StartNextWave();
    //             break;

    //         case GameState.GAMEOVER:
    //             isTimerOn = false;
    //             DefeatAllEnemies();
    //             break;
    //     }
    // }
}

