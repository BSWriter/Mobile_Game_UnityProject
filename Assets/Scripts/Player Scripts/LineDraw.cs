using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LineDraw : MonoBehaviour
{
    public Transform _playerPos;
    Vector3 _midPoint;
    Vector3 _endPoint;
    LineRenderer _linerenderer;
    public float vertexCount = 12;
    public float Point2Yposition = 2;
    List<Vector3> _pointList = new List<Vector3>();

    PlayerController _parentController;

    int _layerMask;
    bool _launchPrimed;
    bool _launchCapable;
    Vector3 _lastTouchPos;

    void Start()
    {
        // _midPoint = new Vector3(0,10,30);
        // _endPoint = new Vector3(0,0,60);
        _linerenderer = GetComponent<LineRenderer>();
        _linerenderer.textureMode = LineTextureMode.Tile;
        _linerenderer.enabled = false;
        _parentController = transform.parent.gameObject.GetComponent<PlayerController>();
        _layerMask = 1 << 6;
        _launchPrimed = false;
        _launchCapable = true;
    }
    

    public List<Vector3> getVectorList(){
        return _pointList;
    }

    public bool lineHit(){
        return _linerenderer.enabled;
    }

    public void setLaunchCapable(bool val){
        _launchCapable = val;
    }

    void Update()
    {
        if(_launchCapable){
            if (Input.touchCount == 1 && _parentController.isLaunchReady()){
                LineControls(Input.GetTouch(0), _parentController.getState());
            }
        }
    }

    void LineControls(Touch t, string chaserState){

        if(chaserState.Equals("airborne") && !_launchPrimed){
            // If the player is touching the screen and the chaser is airborne, commence raycast
            if(t.phase == TouchPhase.Began || t.phase == TouchPhase.Moved || t.phase == TouchPhase.Stationary){
                // Cast a ray towards where the player is touching the screen
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                bool groundHit = Physics.Raycast(ray, out RaycastHit raycastHit, 50f, _layerMask);

                // If the ray hits, draw a curved line to the contact point
                if(groundHit){
                    Time.timeScale = 0;
                    _linerenderer.enabled = true;
                    _endPoint = raycastHit.point;

                    _midPoint = new Vector3((_playerPos.transform.position.x + _endPoint.x), Point2Yposition, (_playerPos.transform.position.z + _endPoint.z) / 2);
                    drawLine(_endPoint, _midPoint);
                }
            }
            else if(t.phase == TouchPhase.Ended && _linerenderer.enabled){
                _launchPrimed = true;
            }
        }
        // If the launch is primed, allow further inputs to influence the midpoint position
        else if(chaserState.Equals("airborne") && _launchPrimed){
            // Set where the player touches the screen to be the central point of manipulation
            if(t.phase == TouchPhase.Began){
                _lastTouchPos = new Vector3(t.position.x, 0, t.position.y);;
                _midPoint = _pointList[(_pointList.Count - 1)/2];
            }
            else if(t.phase == TouchPhase.Moved){
                Vector3 currTouchPos = new Vector3(t.position.x, 0, t.position.y);
                Vector3 moveVector = (_lastTouchPos - currTouchPos).normalized;
                // print("Current Touch: " + currTouchPos + "   Last Touch: " + _lastTouchPos + "   Vector: " + moveVector);
                _lastTouchPos = currTouchPos;

                _midPoint += moveVector * 0.5f;
                drawLine(_pointList[_pointList.Count - 1], _midPoint);
                // _pointList[(_pointList.Count-1)/2] = _midPoint;
                // _linerenderer.SetPositions(_pointList.ToArray());
            }
            else if(t.phase == TouchPhase.Ended){
                _parentController.commenceLaunch(_pointList);
                _linerenderer.enabled = false;
                _launchPrimed = false;
            }

        }
    }

    void drawLine(Vector3 endPoint, Vector3 midPoint){
        _pointList = new List<Vector3>();
        for(float ratio = 0;ratio<=1;ratio+= 1/vertexCount)
        {
            var tangent1 = Vector3.Lerp(_playerPos.position, _midPoint, ratio);
            var tangent2 = Vector3.Lerp(_midPoint, _endPoint, ratio);
            var curve = Vector3.Lerp(tangent1, tangent2, ratio);

            _pointList.Add(curve);
        }

        _linerenderer.positionCount = _pointList.Count;
        _linerenderer.SetPositions(_pointList.ToArray());
    }
}
