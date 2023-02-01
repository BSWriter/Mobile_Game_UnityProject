using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Cinemachine;
using System.Threading.Tasks;
using TMPro;

public class PlayerController : MonoBehaviour
{
    // Public Variables
        // GUI text components
        public GameObject speedText;
        public GameObject maxVelText;
        public GameObject frameRateText;
        public Image speedMeterFill;
        public Image speedMeterNeedle;


    // Serialized Variables
        // Audio variables
        // [SerializeField] AudioSource _rollingSound;

        // Cinemachine reference variables
        [SerializeField] CinemachineFreeLook groundCam;
        [SerializeField] CinemachineFreeLook airCam;
        [SerializeField] CinemachineFreeLook backCam;

        // Particle Effects
        [SerializeField] GameObject speedRibbon;
        
        // Touch --> chaser speed amount
        [SerializeField] float _pushForce = 100f;


    // Private Variables
        // Child object compenent storage
        Rigidbody _playerRb;
        Transform _alignment;
        GameObject _chaser;
        LineDraw _launchLineScript;
        AlignmentController _alignmentScript;
        //ParticleSystem _ps;
        
        // Image component storage
        Transform _smnTransform;

        // Touch variables
        Vector2 _startTouchPos;
        float _startTouchTime;
        Vector2 _prevTouchPos;
        Vector2 _maxTouchPos;
        Queue<float> _consecutiveSwipes;

        // Velocity limit variables
        float _maxVelocity = 25f;
        float _maxLaunchVelocity = 50f;
        float _maxBoostVelocity = 75f;
        float _decayRate = 0.01f;
        bool _decaying;

        // Launch variables
        int _launchCooldownTime;
        Vector3 _velBeforeLaunch;
        List<Vector3> _launchLinePoints;
        bool _launchReady;
        bool _velIncreased;

        // Chaser state variables
        enum State {grounded, airborne, launching}
        State _playerState;
        float _distToGround;
        bool _movingBack;

        // Player stats
        float _avgSpeedSum;
        int _avgSpeedTotal;
        float _highestSpeed;
        float _levelPerformance;

        // Miscellaneous
        bool _playerLock;


    // Unsure Variables
        // Once part of the swipe calculation method
        Vector2 _maxVector;


    void Awake(){
        _playerLock = false;
        _launchReady = false;
        _velIncreased = false;
        _movingBack = false;
        _decaying = false;

        _velBeforeLaunch = Vector3.zero;

        _consecutiveSwipes = new Queue<float>();
        _launchLinePoints = new List<Vector3>();

        _avgSpeedSum = 0f;
        _avgSpeedTotal = 0;
        _highestSpeed = 0f;
        
        _launchCooldownTime = 10;
    }

    void Start()
    {
        _smnTransform = speedMeterNeedle.transform;
        _chaser = transform.Find("Body").gameObject;
        _playerRb = transform.Find("Body").GetComponent<Rigidbody>();
        _distToGround = transform.Find("Body").GetComponent<Collider>().bounds.extents.y;      
        _launchLineScript = transform.Find("Launch Line").GetComponent<LineDraw>();
        _alignmentScript = transform.Find("Alignment Reference").GetComponent<AlignmentController>();
        _alignment = transform.Find("Alignment Reference").transform;

        // _ps = transform.Find("Sphere").Find("LaunchSparks").GetComponent<ParticleSystem>();
        // _ps.Stop();
    }

    
    void Update()
    {  
        // Store chaser velocity direction and magnitude
        Vector3 chaserDir = _playerRb.velocity;
        float chaserSpeed = chaserDir.magnitude;

        // Check if current speed is greater than highest speed and assign if necessary
        if(chaserSpeed > _highestSpeed){
            _highestSpeed = chaserSpeed;
        }

        

        // Change the fill amount to correspond with the current speed
        speedMeterFill.fillAmount = Mathf.Lerp(speedMeterFill.fillAmount, (0.5f + 0.5f * (_maxVelocity/100f)), 10 * Time.deltaTime); // 0.5f + (0.5f * (75/100f));
        // print("Fill amount: " + speedMeterFill.fillAmount + "  Actual Amount: " + num);
        // Change the speed meter needle to point the max velocity threshold
        _smnTransform.rotation = Quaternion.Slerp(_smnTransform.rotation, Quaternion.Euler(0, 0, 180f + (180f * (chaserSpeed/100f))), 10 * Time.deltaTime);

        // var slopeRotation = Quaternion.FromToRotation(transform.up, hit.normal);
        // transform.rotation = Quaternion.Slerp(transform.rotation, slopeRotation * transform.rotation, 10 * Time.deltaTime);

        // Display framerate
        int fr = (int)(1.0f / Time.smoothDeltaTime);
        frameRateText.GetComponent<TextMeshProUGUI>().text = fr.ToString();

        // Update the player state (grounded, airborne, or launching)
        Vector3 fallingVel = new Vector3(_playerRb.velocity.x, _playerRb.velocity.y, 0);
        checkState(fallingVel);

        // Allow the player to move if not locked (Hopefully helps with performance time)
        if(!_playerLock ){    
            if(Input.touchCount == 1){
                calculateSwipe(Input.GetTouch(0));
            }
            // Adding for local test (remove after NN work)
            // if (Input.GetKeyDown("w")){
            //     direction = _alignment.forward;
            // }
            // else if (Input.GetKeyDown("s")){
            //     direction = -1 * _alignment.forward;
            // }
            // else if (Input.GetKeyDown("d")){
            //     direction = _alignment.right;
            // }
            // else if (Input.GetKeyDown("a")){
            //     direction = -1 * _alignment.right;
            // }
            // Vector3 direction = Vector3.zero;
            // _playerRb.AddForce(direction * _pushForce);
        }
        
        if(chaserSpeed >= 30){
            // If the chaser's velocity goes over the max velocity, reduce the velocity to the max.
            curtailVelocity();
        }

        // Update Speed ribbon to face direction of chaser movement
        // var targetRotation = Quaternion.LookRotation(chaserDir);  
        // speedRibbon.transform.rotation = Quaternion.Slerp(speedRibbon.transform.rotation, targetRotation, Time.deltaTime * 2);
    }

    void ChangeState(State state){
        // If new state is current player state ignore all other conditions
        if(state.Equals(_playerState)){

        }
        // if the player's state is changed to grounded, remove abilty to launch, turn gravity back on, and change to grounded cam
        else if(state.Equals(State.grounded)){
            _playerState = State.grounded;
            _playerRb.useGravity = true;
            changeCamera("grounded");
        }
        // If the player's state is changed to airborne, allow launches if conditions are met, change to airborne cam, and start the launch ready countdown
        else if(state.Equals(State.airborne)){
            _playerState = State.airborne;
            StartCoroutine(gettingLaunchReady());
            changeCamera("airborne");
        }
        // If the player's state is changed to launching, remove gravity, stop all velocity, remove ability to launch, start launching process
        // Maybe have another camera?
        else if(state.Equals(State.launching)){
            _playerState = State.launching;
            // Turn time to normal (having the launch line drawn slows down time in the line script)
            Time.timeScale = 1;
            // Turn the player lock so further screen touches do not interfere with the launch
            _playerLock = true;
            _playerRb.useGravity = false;
            _playerRb.velocity = Vector3.zero;
            _launchReady = false;
        }
    }

    void checkState(Vector3 vel){
        Vector3 playerPos = _chaser.transform.position;

        // Check if there any terrain below the chaser
        bool AlignToGround = Physics.Raycast(playerPos, -_alignment.up, _distToGround + 0.2f);
        bool BotToGround = false;
        
        // Check if there is any terrain above the chaser if not below
        if(!AlignToGround){
            BotToGround = Physics.Raycast(playerPos, Vector3.down, _distToGround + 0.1f);
            if(!BotToGround){
                _alignment.up = -vel;
            }
            else{
                _alignment.up = Vector3.up;
            }
        }

        // If the player's state is launching, ignore the bottom conditions as it doesn't matter if the ball is on the ground or in the air unless colliding
        if(_playerState.Equals(State.launching)){
            
        }
        // If there is any terrain either above or below the chaser then set the player-state to grounded
        else if(AlignToGround || BotToGround){
            ChangeState(State.grounded);
            // Ensure that the launch ready timer restarts
            _launchReady = false;
            // Stop the launch ready coroutine so the time doesn't run while grounded
            StopCoroutine(gettingLaunchReady());
        }
        else{
            ChangeState(State.airborne);
        }


        // Different returns, might have possible use later?
            // return AlignToGround || BotToGround;

            // bool AlignToGround = Physics.Raycast(_player.position, -_alignment.up, _distToGround + 1f);
            // return AlignToGround;
    }

    // Infinite loop continuously calculate avg speed of chaser for the level
    IEnumerator calculateAvgSpeed(){
        while(true){
            _avgSpeedSum += _playerRb.velocity.magnitude;
            _avgSpeedTotal += 1;
            // print(_avgSpeedSum/_avgSpeedTotal);
            yield return new WaitForSeconds(0.5f);
        }
    }

    // Trigger when chaser is airborne, stops immediate launchs when leaving the ground
    IEnumerator gettingLaunchReady(){
        yield return new WaitForSeconds(1);
        _launchReady = true;
    }

    IEnumerator launchCooldown(){
        _launchLineScript.setLaunchCapable(false);
        yield return new WaitForSeconds(_launchCooldownTime);
        _launchLineScript.setLaunchCapable(true);
    }


    IEnumerator DecayMaxVelocity(){
        _decaying = true;
        while(_maxVelocity > 30){
            float chaserSpeed = _playerRb.velocity.magnitude;
            _maxVelocity -= (Mathf.Pow(2, (Mathf.Abs(_maxVelocity - chaserSpeed)) * _decayRate) - 1)*3;
            yield return new WaitForSeconds(0.5f);
        }
        _decaying = false;
    }



    // The launch method. Uses a linked list to store the points from the launch line and have the ball gain ramping velocity towards each point in sequence.
    IEnumerator launchBall(){
        // Store line points in local variable
        List<Vector3> vp = _launchLinePoints;

        // Record the chaser's velocity prior to launching
        _velBeforeLaunch = _playerRb.velocity;

        // change player state to launching
        ChangeState(State.launching);

        LinkedList<Vector3> points = new LinkedList<Vector3>();
        
        // Add all points in line to a Linked list as nodes
        foreach(Vector3 v in vp){
            LinkedListNode<Vector3> p = new LinkedListNode<Vector3>(v);
            points.AddLast(p);
        }
        
        LinkedListNode<Vector3> currNode = points.First;

        // float launchPower = 10;
        float launchPower = _velBeforeLaunch.magnitude / 2;
        float rampUp = 1;
        // Need to implement: Add onto the chaser's prelaunch velocity
        while(currNode.Next != null){
            // Get direction to next point
            Vector3 dir = (currNode.Value - _chaser.transform.position).normalized;
            
            // If the chaser is within a certain threshold from the current point, switch the current point to the next point in the sequence.
            if(Vector3.Distance(_chaser.transform.position, currNode.Value) < 1f){
                currNode = currNode.Next;
            } 
            // Else modify the chaser's velocity to move towards the current point
            else{
                _playerRb.velocity = dir * launchPower * rampUp;
                if(_playerRb.velocity.magnitude > _maxVelocity && !_velIncreased){
                    curtailVelocity();
                    _maxVelocity += 5;
                    _velIncreased = true;
                }
                // print(_playerRb.velocity + " " + _playerRb.velocity.magnitude);
                else if(!_velIncreased){
                    rampUp += 0.1f;
                }
                
            }

            yield return new WaitForEndOfFrame();
        }        

        yield return new WaitForEndOfFrame();
    }

    
    void calculateSwipe(Touch t){
        // Ground Controls
        if(_playerState.Equals(State.grounded)){

            // Record the initial area the player touched
            if (t.phase == TouchPhase.Began){
                _startTouchPos = t.position;
                _maxTouchPos = t.position;
            }

            // If the touch position is moved, make small incremental changes to speed based on how much distance the touch travelled
            if (t.phase == TouchPhase.Moved){
                Vector2 touchVector = _startTouchPos - t.position;
                Vector2 pushDir = t.position - _maxTouchPos;

                // Not sure what this does, delete if nothing noticeable occurs
                // if (touchVector.magnitude >= _maxVector.magnitude){
                //     print("Testing");
                //     _maxTouchPos = t.position;
                // }
                _maxTouchPos = t.position;

                Vector3 direction = new Vector3(pushDir.x, 0, pushDir.y);
                direction = Quaternion.AngleAxis(_alignment.eulerAngles.y, Vector3.up)*direction;
                direction = Quaternion.AngleAxis(_alignment.eulerAngles.x, Vector3.right)*direction;

                // If moving backwards, invert the alignment's z angle 
                if(_movingBack) {
                    direction = Quaternion.AngleAxis(-(_alignment.eulerAngles.z), Vector3.forward)*direction;
                }
                else{ 
                    direction = Quaternion.AngleAxis(_alignment.eulerAngles.z, Vector3.forward)*direction;
                }

                _playerRb.AddForce(direction*_pushForce);
            }
            
            // If the touch ends, apply a large burst of movement to the chaser
            if (t.phase == TouchPhase.Ended){
                Vector2 pushDir = t.position - _maxTouchPos;
                Vector3 direction = new Vector3(pushDir.x, 0, pushDir.y);
                direction = Quaternion.AngleAxis(_alignment.eulerAngles.y, Vector3.up)*direction;
                direction = Quaternion.AngleAxis(_alignment.eulerAngles.x, Vector3.right)*direction;

                if(_movingBack) {
                    direction = Quaternion.AngleAxis(-(_alignment.eulerAngles.z), Vector3.forward)*direction;
                }
                else{ 
                    direction = Quaternion.AngleAxis(_alignment.eulerAngles.z, Vector3.forward)*direction;
                }

                _playerRb.AddForce(direction*(_pushForce*2));

                // Get the overall up/down direction of the swipe
                Vector2 netDir = (_startTouchPos - t.position).normalized;
                float swipeAngle = Vector2.Angle(Vector2.right, netDir);

                // If the direction of the swipe is positive(down), add it to the queue
                if(Mathf.Sign(netDir.y) != -1 && 70 < swipeAngle && swipeAngle < 110)
                {
                    _consecutiveSwipes.Enqueue(Mathf.Sign(netDir.y));
                }
                // Else empty the queue
                else{
                    _consecutiveSwipes = new Queue<float>();
                }
            }
        }

        // If the player makes 3 consecutive swipes backwards, swap the state of the ground cam
        if(_consecutiveSwipes.Count == 3){
            _movingBack = !_movingBack;
            changeCamera("grounded");
            _alignmentScript.SwapDirection();

            // In the end, empty the queue
            _consecutiveSwipes = new Queue<float>();
        }
       
    
    }

    void changeCamera(string state){
        if(state.Equals("grounded")){
            if(_movingBack){ 
                // Reset the position of the back Cam
                backCam.m_RecenterToTargetHeading.m_enabled = true;
                backCam.m_YAxisRecentering.m_enabled = true;
                backCam.m_XAxis.m_Recentering.m_enabled = true;

                // Disable the camera centering after it completes
                // so it doesn't stay locked
                // Multiply the completion timeout for some padding
                int completionTimeout = (int) backCam.m_RecenterToTargetHeading.m_RecenteringTime * 1000; // 0.25 *1000
                Task.Delay(completionTimeout * 2).ContinueWith(t => {
                    backCam.m_RecenterToTargetHeading.m_enabled = false;
                    backCam.m_YAxisRecentering.m_enabled = false;
                    backCam.m_XAxis.m_Recentering.m_enabled = false;
                });

                groundCam.Priority = 10;
                airCam.Priority = 11;
                backCam.Priority = 12;
            }
            else{
                // Reset the position of the back Cam
                groundCam.m_RecenterToTargetHeading.m_enabled = true;
                groundCam.m_YAxisRecentering.m_enabled = true;
                groundCam.m_XAxis.m_Recentering.m_enabled = true;

                // Disable the camera centering after it completes
                // so it doesn't stay locked
                // Multiply the completion timeout for some padding
                int completionTimeout = (int) groundCam.m_RecenterToTargetHeading.m_RecenteringTime * 1000; // 0.25 *1000
                Task.Delay(completionTimeout * 2).ContinueWith(t => {
                    groundCam.m_RecenterToTargetHeading.m_enabled = false;
                    groundCam.m_YAxisRecentering.m_enabled = false;
                    groundCam.m_XAxis.m_Recentering.m_enabled = false;
                });

                groundCam.Priority = 11;
                airCam.Priority = 10;
                backCam.Priority = 9;
            }
        }
        else if(state.Equals("airborne")){
            groundCam.Priority = 10;
            airCam.Priority = 11;
            backCam.Priority = 9;
        }
    }

    // Stop the chasers velocity from going over the set maximum
    void curtailVelocity(){
        if(Mathf.Abs(_playerRb.velocity.x) >= _maxVelocity){
            _playerRb.velocity = new Vector3(Mathf.Sign(_playerRb.velocity.x) * _maxVelocity, _playerRb.velocity.y, _playerRb.velocity.z);
        }
        if(Mathf.Abs(_playerRb.velocity.y) >= _maxVelocity){
            _playerRb.velocity = new Vector3(_playerRb.velocity.x, Mathf.Sign(_playerRb.velocity.y) * _maxVelocity ,_playerRb.velocity.x);
        }
        if(Mathf.Abs(_playerRb.velocity.z) >= _maxVelocity){
            _playerRb.velocity = new Vector3(_playerRb.velocity.x, _playerRb.velocity.y, Mathf.Sign(_playerRb.velocity.z) * _maxVelocity);
        }
    }

    // Getters and Setters
    public string getState(){
        return _playerState.ToString();
    }

    public bool isLaunchReady(){
        return _launchReady;
    }

    public float getAvgSpeed(){
        return _avgSpeedSum/_avgSpeedTotal;
    }

    // If the chaser collides with another Rigidbody while launching, stop the launch and change the player state to grounded.
    public void launchCollide(){
        _playerLock = false;
        ChangeState(State.grounded);
        // StopAllCoroutines();
        StopCoroutine(launchBall());
        if(_velIncreased && !_decaying){
            StartCoroutine(DecayMaxVelocity());
        }
        _velIncreased = false;
        StartCoroutine(launchCooldown());
        // StopCoroutine(gettingLaunchReady());
    }

    // Public method to allow line script to commennce a launch
    public void commenceLaunch(List<Vector3> points) {
        // StartCoroutine(launchBall(points));
        _launchLinePoints = points;
        StartCoroutine(launchBall());
    }

    // If the player enters a new level, reset the player stat variables
    public void newLevel(){
        StopCoroutine(calculateAvgSpeed());
        _avgSpeedSum = 0f;
        _avgSpeedTotal = 0;
        StartCoroutine(calculateAvgSpeed());
    }


}


// Launch variation that utilize LeanTween spline to perform launch. Does not affect chaser velocity so unsure if necessary.
// IEnumerator launchBall(List<Vector3> vp){
//     // change player state to launching
//     ChangeState(State.launching);

//     // Create a new list of points from the pervious listg
//     List<Vector3> points = new List<Vector3>();
//         // Add beginning and end points to list for spline movement 
//     points.Add(vp[0]);
//     foreach(Vector3 v in vp){
//         points.Add(v);
//     }
//     points.Add(vp[vp.Count-1]);

//     // move the player along the spline
//     LeanTween.moveSpline(_player, points.ToArray(), 2f).setOrientToPath(true).setEase(LeanTweenType.easeInQuad);

//     yield return new WaitForEndOfFrame();
// }