using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PickupManager : MonoBehaviour
{

    public GameObject scoreText;
    public GameObject frameRate;
    public static int score;

    void Update(){
        scoreText.GetComponent<Text>().text = "Score: "+score;
        int fr = (int)(1.0f / Time.smoothDeltaTime);
        frameRate.GetComponent<Text>().text = fr.ToString();
    }


}
