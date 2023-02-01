using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Cinemachine;

public class SphereCollision : MonoBehaviour
{
    public GameObject scoreText;
    public GameObject thresholdText;
    public GameObject topStartText;
    public GameObject botStartText;
    public GameObject Canvas;


    [SerializeField] GameObject Level;
    [SerializeField] GameObject EndCamTrigger;
    [SerializeField] CinemachineBrain cine;
    [SerializeField] CinemachineFreeLook groundCam;
    [SerializeField] CinemachineFreeLook airCam;
    [SerializeField] CinemachineFreeLook backCam;
    [SerializeField] CinemachineFreeLook endCam;

    PlayerController _parentController;
    LevelManager _levelManager;
    GeneralGUIManager _guiManager;
    Rigidbody _rb;
    int _score;
    float _speedThreshold;

    void Start()
    {
        _parentController = transform.parent.gameObject.GetComponent<PlayerController>();
        _levelManager = Level.GetComponent<LevelManager>();
        _guiManager = Canvas.GetComponent<GeneralGUIManager>();
        _rb = transform.GetComponent<Rigidbody>();
        _score = 0;
        _speedThreshold = 0;

        // Begin the start lettering tween
        LeanTween.scale(botStartText, new Vector3(1.2f,1.5f,1), 1f).setEaseInOutSine().setLoopPingPong();
    }

    void OnCollisionEnter(Collision collision)
    {
        // If the player collides with an object while launching, stop the launch
        if(_parentController.getState().Equals("launching")){
            _parentController.launchCollide();
        }
    }

    void OnTriggerEnter(Collider other) {
        // If the chaser collides with a collectible, increment score and destroy collectible
        if (other.tag == "Collectible") {
            _score += 1;
            scoreText.GetComponent<TextMeshProUGUI>().text = _score.ToString();
            Destroy(other.gameObject);

            // Start the UI fade process
            _guiManager.CollectedStar();
        }
        // If the chaser collides with the end trigger, change main camera to endcam
        else if(other.tag == "EndTrigger"){
            endCam.Priority = 20;

            // Initiate transfer of player stats to level script

        }
        // If the chaser collides with the glass pane at the end, trigger the the process to reset and iterate level
        else if(other.tag == "NextLevel"){
            if(_rb.velocity.z > _speedThreshold){
                _speedThreshold += 5f;
                _levelManager.NextLevel();  
                thresholdText.GetComponent<TextMeshProUGUI>().text = "" + _speedThreshold;
                LeanTween.cancel(botStartText);
                topStartText.SetActive(false);
                botStartText.SetActive(false);
                _parentController.newLevel();
            }
            else{
                _rb.velocity = new Vector3(_rb.velocity.x, _rb.velocity.y, -(_rb.velocity.z/2));
                transform.position = new Vector3(transform.position.x, transform.position.y, 269.2f);
            }
        }
    }

    void OnTriggerExit(Collider other){
        if(other.tag == "EndTrigger"){
            endCam.Priority = 1;
            StartCoroutine(repositionEndCam());
        }
    }

    IEnumerator repositionEndCam(){
        // while(cine.ActiveVirtualCamera.Equals(endCam)){
        //     yield return new WaitForEndOfFrame();
        // }
        yield return new WaitForSeconds(1);

        endCam.transform.position = new Vector3(endCam.transform.position.x, endCam.transform.position.y, 175);
    }

    public int getScore(){
        return _score;
    } 
}
