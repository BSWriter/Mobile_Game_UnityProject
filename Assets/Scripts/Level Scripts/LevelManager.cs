using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour
{
    // #0 = Loop    #1 = NoTile     #2 = Generic    #3 = LeftOnly    #4 = RightOnly
    public GameObject[] Tiles;
    // #0 = Normal  #1 = LeftLean   #2 = RightLean
    public GameObject[] Ramps;
    [SerializeField] GameObject trigger;
    // [SerializeField] GameObject rampObject;
    [SerializeField] GameObject fog;
    [SerializeField] GameObject endCamTrigger;
    [SerializeField] GameObject gameManager;
    
    public Transform Player;
    public CinemachineFreeLook groundCine;
    public CinemachineFreeLook airCine;
    public CinemachineFreeLook endCam;

    GameManager _GMscript;
    CollectibleManager _collectibleScript;

    enum Level{Start, Normal, RightPit, LeftPit, GapJump};
    Level _previousLevel;
    GameObject _prevLevelLastTile;
    GameObject _brokenGlass;
    float _fogSpeed;
    int _levelIteration;

    List<GameObject> _currentLevel;
    List<GameObject> _nextLevel; 

    // Tiles for new level gen
    GameObject _firstTile;
    GameObject _secondTile;
    GameObject _thirdTile;
    GameObject _fourthTile;
    GameObject _lastTile;

    void Awake()
    {
        _fogSpeed = 30f;
        _levelIteration = 0;

        _currentLevel = new List<GameObject>();
        _nextLevel = new List<GameObject>();

        _previousLevel = Level.Normal;

        _collectibleScript = transform.Find("Collectibles").GetComponent<CollectibleManager>();
        _GMscript = gameManager.GetComponent<GameManager>();

        // Assign the next tiles
        LevelBuilder(Level.GapJump);

        // Create the next level's tiles and place them in environment
        PreloadLevel();
    }

    void Start(){

        _prevLevelLastTile = Tiles[0];

        _currentLevel.Add(GameObject.Find("Receptacle"));
        _currentLevel.Add(GameObject.Find("Second Tile"));
        _currentLevel.Add(GameObject.Find("Third Tile"));
        _currentLevel.Add(GameObject.Find("Fourth Tile"));
        _currentLevel.Add(GameObject.Find("End Tile"));
          
    }

    // Called whenever chaser passes through end glass
    public void NextLevel(){
        // Increment level iteration variable
        _levelIteration += 1;

        // Destroy all the current tiles in the level as well as Ramp object
        ResetEnvironment();

        // Assign the tiles for the next level
        LevelBuilder(_previousLevel);

        // Reset the chaser's position to be a the start of the level, keeping vertical and horizontal orientation
        Vector3 prevPos = Player.position;
        Player.position = new Vector3(Player.position.x, Player.position.y, -30);
        Vector3 dPos = Player.position - prevPos;
        // Have the cinemachines warp with the chaser to avoid interpolation (I think that is the right word)
        groundCine.OnTargetObjectWarped(Player, dPos);
        airCine.OnTargetObjectWarped(Player, dPos);
        // Position the end camera to warp with the others to continue zoom out effect
        endCam.transform.position = new Vector3(endCam.transform.position.x, endCam.transform.position.y, -95);

        // Move all of them objects assigned to the next level back for the current level
        CreateLevel();

        // Create the next level's environment object
        AssignEnvObjects(_previousLevel);

        // Reset the fog position and update speed
        FogReset(_fogSpeed);

        // Preload next level's tiles and place them in environment
        PreloadLevel();
    }
    
    // Destroy all of the gameObject in the current level
    void ResetEnvironment(){
        _prevLevelLastTile = _lastTile;
        foreach(GameObject obj in _currentLevel){
            Destroy(obj);
        }
    }

    // Change for Level Scripts
    // Method for chossing tiles before loading them into level
    void LevelBuilder(Level prev){

        // First tile will always be loop tile (may change later)
        _firstTile = Tiles[0];

        // Normal Level
        if(prev.Equals(Level.GapJump)){
            _secondTile = Tiles[2];
            _thirdTile = Tiles[2];
            _fourthTile = Tiles[2];
            _previousLevel = Level.Normal;
        }
        // Right Pit
        if(prev.Equals(Level.Normal)){
            _secondTile = Tiles[2];
            _thirdTile = Tiles[4];
            _fourthTile = Tiles[2];   
            _previousLevel = Level.RightPit;
        }
        // Left Pit
        if(prev.Equals(Level.RightPit)){
            _secondTile = Tiles[2];
            _thirdTile = Tiles[3];
            _fourthTile = Tiles[2];
            _previousLevel = Level.LeftPit;
        }
        // Gap Jump
        if(prev.Equals(Level.LeftPit)){
            _secondTile = Tiles[2];
            _thirdTile = Tiles[1];
            _fourthTile = Tiles[2];
            _previousLevel = Level.GapJump;
        }

        // Last tile will always be loop tile (may change later)
        _lastTile = Tiles[0];
    }

    // 
    // Create the next level's tiles and place them in the environment
    void PreloadLevel(){
        // Instantiate five tiles for the next level and add them to the next level list
        GameObject startTile = Instantiate(_firstTile, new Vector3(0,0,300), Quaternion.Euler(-90, 0, 0)) as GameObject;
        startTile.transform.parent = this.transform;
        _nextLevel.Add(startTile);

        GameObject secondTile = Instantiate(_secondTile, new Vector3(0,0,360), Quaternion.Euler(-90, 0, 0)) as GameObject;
        secondTile.transform.parent = this.transform;
        _nextLevel.Add(secondTile);

        GameObject thirdTile = Instantiate(_thirdTile, new Vector3(0,0,420), Quaternion.Euler(-90, 0, 0)) as GameObject;
        thirdTile.transform.parent = this.transform;
        _nextLevel.Add(thirdTile);

        GameObject fourthTile = Instantiate(_fourthTile, new Vector3(0,0,480), Quaternion.Euler(-90, 0, 0)) as GameObject;
        fourthTile.transform.parent = this.transform;
        _nextLevel.Add(fourthTile);

        GameObject endTile = Instantiate(_lastTile, new Vector3(0,0,540), Quaternion.Euler(-90, 0, 0)) as GameObject;
        endTile.transform.parent = this.transform;
        _nextLevel.Add(endTile);
    }

    void AssignEnvObjects(Level lv){
        // According to the level, instantiate appropriate ramps and collectibles
        if(lv.Equals(Level.RightPit)){
            GameObject r = Instantiate(Ramps[2], new Vector3(0, 0, 390), Quaternion.Euler(-135, 90, 90)) as GameObject;
            r.transform.parent = this.transform;
            r.transform.localScale = new Vector3(r.transform.localScale.x * 2, r.transform.localScale.y, r.transform.localScale.z);
            _nextLevel.Add(r);

            List<GameObject> collectibles = _collectibleScript.CreateCollectibles(lv.ToString());
            _nextLevel.AddRange(collectibles);
        }
        else if(lv.Equals(Level.LeftPit)){
            GameObject r = Instantiate(Ramps[1], new Vector3(0, 0, 390), Quaternion.Euler(-45, 90, 90)) as GameObject;
            r.transform.parent = this.transform;
            r.transform.localScale = new Vector3(r.transform.localScale.x * 2, r.transform.localScale.y, r.transform.localScale.z);
            _nextLevel.Add(r);
        }
        else if(lv.Equals(Level.GapJump)){
            GameObject r = Instantiate(Ramps[0], new Vector3(0, 0, 390), Quaternion.Euler(-90, 90, 90)) as GameObject;
            r.transform.parent = this.transform;
            r.transform.localScale = new Vector3(r.transform.localScale.x * 2, r.transform.localScale.y, r.transform.localScale.z);
            _nextLevel.Add(r);
        }
        else if(lv.Equals(Level.Normal)){
            List<GameObject> collectibles = _collectibleScript.CreateCollectibles(lv.ToString());
            _nextLevel.AddRange(collectibles);
        }


        // Get the collectibles made for the level
        
    }

    // Move all of the object in the next level to the correct place for the current level
    void CreateLevel(){
        foreach(GameObject obj in _nextLevel){
            Vector3 pos = obj.transform.position;
            obj.transform.position = new Vector3(pos.x, pos.y, pos.z - 300);            

            _currentLevel.Add(obj);
        }

        _nextLevel = new List<GameObject>();

        // Instantiate the previous level's end tile behind the first tile
        GameObject prevTile = Instantiate(_prevLevelLastTile, new Vector3(0,0,-60), Quaternion.Euler(-90,0,0)) as GameObject;
        prevTile.transform.parent = this.transform;
        _currentLevel.Add(prevTile);
        
        // Add broken glass to player spawn and add to current level list
        _brokenGlass = Instantiate(trigger, new Vector3(0, 0, -30), Quaternion.Euler(0,0,0)) as GameObject;
        _brokenGlass.transform.parent = this.transform;
        _currentLevel.Add(_brokenGlass);
    }

    void FogReset(float speed){
        // Ensure the fog is currently active and cancel all of it's movement
        fog.SetActive(true);
        LeanTween.cancel(fog);

        // Start having the fog move toward the end at the predefined speed
        fog.transform.position = new Vector3(0, 8.1f, -150);
        LeanTween.moveZ(fog, 300, speed);
    }

    public Transform getPlayer(){
        return Player;
    }

    public CinemachineFreeLook getGroundCine(){
        return groundCine;
    }

    public CinemachineFreeLook getAirCine(){
        return airCine;
    }

    // Play game over animation, reset environment.
    public void GameOver(){
        // SceneManager.LoadScene("MainGame");
        _GMscript.GameOver();
    }
}
