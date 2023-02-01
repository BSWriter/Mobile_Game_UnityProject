using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GeneralGUIManager : MonoBehaviour
{
    public GameObject StarUI;
    public GameObject StarAmount;
    public GameObject ScoreUI;
    public GameObject SpeedMeterSweetSpot;

    [SerializeField] GameObject gameManager;
    GameManager _GMscript;


    Image StarUIImg;
    TextMeshProUGUI StarAmountText;

    float _scoreAlpha;
    bool _newCollected;

    void Start()
    {
        StarUIImg = StarUI.GetComponent<Image>();
        StarAmountText = StarAmount.GetComponent<TextMeshProUGUI>();

        _scoreAlpha = StarUIImg.color.a;
        _newCollected = false;

        _GMscript = gameManager.GetComponent<GameManager>();

        LeanTween.scale(SpeedMeterSweetSpot, new Vector3(1.07f, 1.025f, 1), 1.5f).setEaseInOutSine().setLoopPingPong();
        LeanTween.color(SpeedMeterSweetSpot, new Color(1, 0.7f, 0.3f, 1), 1.5f).setLoopPingPong();

        fadeIn();
    }

    void Update()
    {
        
    }

    public void CollectedStar(){
        // Stop all animations on the score ui
        LeanTween.cancel(ScoreUI);

        // Stop future invokes
        CancelInvoke();
        
        // Begin Score UI fade in/out process
        fadeIn();
    }

    void fadeIn(){
        // Have the score ui fade in quickly 
        ScoreUI.LeanValue(_scoreAlpha, 1, 0.1f).setOnUpdate((float val) => {
            // Set the color of the Score image to the new alpha
            Color c = StarUIImg.color;
            c.a = val;
            StarUIImg.color = c;

            // Store current alpha value in global variable
            _scoreAlpha = val;

            // Have the text follow the same alpha
            StarAmountText.color = new Color(StarAmountText.color.r, StarAmountText.color.g, StarAmountText.color.b, val);            
        });

        // Pop out effect
        ScoreUI.transform.localScale = new Vector3(0.9f, 0.9f, 1);
        LeanTween.scale(ScoreUI, new Vector3(1f, 1f, 1), 0.2f).setEaseInOutElastic().setLoopOnce();

        // Wait for 2 second before starting fade out process
        Invoke("fadeOut", 2);
    }

    void fadeOut(){
        ScoreUI.LeanValue(_scoreAlpha, 0, 3f).setOnUpdate((float val) => {
            // Set the color of the Score image to the new alpha
            Color c = StarUIImg.color;
            c.a = val;
            StarUIImg.color = c;

            // Store current alpha value in global variable
            _scoreAlpha = val;

            // Have the text follow the same alpha
            StarAmountText.color = new Color(StarAmountText.color.r, StarAmountText.color.g, StarAmountText.color.b, val);         
        });

        // Diminish effect 
        LeanTween.scale(ScoreUI, new Vector3(0.75f, 0.75f, 1), 3f);
    }

}
