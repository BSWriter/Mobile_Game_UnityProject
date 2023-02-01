using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    public static bool Paused = false;

    [SerializeField] GameObject pauseUI;
    [SerializeField] GameObject pauseTransition;
    [SerializeField] GameObject pauseButton;
    [SerializeField] GameObject gameManager;
    GameManager _GMscript;

    void Start(){
        _GMscript = gameManager.GetComponent<GameManager>();
    }

    public void Resume(){
        // Commence transition to resume the game
        StartCoroutine(ResumeTransition());
    }

    public void Pause(){
        // Stop game
        Time.timeScale = 0;

        // Commence the transition to the pause menu
        StartCoroutine(PauseTransition());
    }

    public void Restart(){
        // Return Time scale to normal
        Time.timeScale = 1;

        // Load the main game
        _GMscript.Load("MainGame");
    }

    IEnumerator PauseTransition(){
        // Deactivate pause button
        pauseButton.SetActive(false);

        // Start transition animation
        pauseTransition.LeanScale(new Vector3(10,15,1), 0.5f).setIgnoreTimeScale(true).setEaseInOutCubic();

        // Wait for transition to finish
        yield return new WaitForSecondsRealtime(0.5f);

        // Deactivate and activate appropriate UI elements
        pauseUI.SetActive(true);
        pauseTransition.SetActive(false);

        Paused = true;
    }

    IEnumerator ResumeTransition(){
        // Deactivate and activate appropriate UI elements
        pauseUI.SetActive(false);
        pauseTransition.SetActive(true);

        pauseTransition.LeanScale(new Vector3(1,1,1), 0.5f).setIgnoreTimeScale(true).setEaseInOutCubic();

        // Wait for trasition to finish
        yield return new WaitForSecondsRealtime(0.5f);

        // Reactivate pause button
        pauseButton.SetActive(true);

        // Return time to normal
        Time.timeScale = 1;

        Paused = false;
    }
}
