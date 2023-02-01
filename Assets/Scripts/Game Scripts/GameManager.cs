using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{

    public GameObject loadingScreen;
    // public string sceneToLoad;
    public CanvasGroup canvasGroup;

    public void Start(){
        DontDestroyOnLoad(gameObject);
    }

    public void GameOver(){
        StartCoroutine(StartLoad("MainGame"));
    }

    public void Load(string scene){
        StartCoroutine(StartLoad(scene));
    }

    IEnumerator StartLoad(string sceneToLoad)
    {
        loadingScreen.SetActive(true);
        yield return StartCoroutine(FadeLoadingScreen(1, 1));
        AsyncOperation operation = SceneManager.LoadSceneAsync(sceneToLoad);
        while (!operation.isDone)
        {
            yield return null;
        }
        yield return StartCoroutine(FadeLoadingScreen(0, 1));
        loadingScreen.SetActive(false);
    }
    
    IEnumerator FadeLoadingScreen(float targetValue, float duration)
    {
        float startValue = canvasGroup.alpha;
        float time = 0;
        while (time < duration)
        {
            canvasGroup.alpha = Mathf.Lerp(startValue, targetValue, time / duration);
            time += Time.deltaTime;
            yield return null;
        }
        canvasGroup.alpha = targetValue;
    }

    // void Save(float score, int currency, int[] unlocks){

    // }

    // public void LoadLevel(int sceneIndex){
    //     StartCoroutine(AsyncLoad(sceneIndex));
    // }

    // IEnumerator AsyncLoad(int sceneIndex){

    //     AsyncOperation asyncLevel = SceneManager.LoadSceneAsync(sceneIndex);
        
    //     while(!asyncLevel.isDone){
    //         float progress = Mathf.Clamp01(asyncLevel.progress / .9f);
    //         Debug.Log(progress);

    //         yield return null;
    //     }

    // }
}
