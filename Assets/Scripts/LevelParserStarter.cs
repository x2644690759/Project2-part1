using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Object = UnityEngine.Object;

public class LevelParserStarter : MonoBehaviour
{
    public string filename;

    public GameObject Rock;

    public GameObject Brick;

    public GameObject QuestionBox;

    public GameObject Stone;

    public float GridSize = 1;

    public Transform StartGameObject;
    
    public Transform parentTransform;

    public int GameTotalTime = 375;
    
    
    //ShowUI
    public TextMeshProUGUI TimeLeftText;
    public TextMeshProUGUI ScoreText;
    public TextMeshProUGUI CoinText;

    public int PerCoinScore = 100;
    
    // score
    private int _score = 0;
    private int _coin = 0;
    // startTime
    private DateTime _gameEndTime;
    private Camera _checkCam;
    
    // Start is called before the first frame update
    void Start()
    {
        if (StartGameObject != null)
        {
            StartGameObject.gameObject.SetActive(false);
        }
        _checkCam = Camera.main;
        RefreshParse();
        var now = System.DateTime.Now;
        _gameEndTime = now + TimeSpan.FromSeconds(GameTotalTime);
        
    }


    private void FileParser()
    {
        string fileToParse = string.Format("{0}{1}{2}.txt", Application.dataPath, "/Resources/", filename);

        using (StreamReader sr = new StreamReader(fileToParse))
        {
            string line = "";
            int row = 0;

            Vector3 startPoint = Vector3.zero;
            if (StartGameObject!= null)
            {
                startPoint = StartGameObject.position;
            }
            while ((line = sr.ReadLine()) != null)
            {
                int column = 0;
                row++;
                char[] letters = line.ToCharArray();
                foreach (var letter in letters)
                {
                    column++;
                    Vector3 position = startPoint + new Vector3(column * GridSize, -row * GridSize, 0);
                    //Call SpawnPrefab
                    SpawnPrefab(letter, position);
                }
                

            }

            sr.Close();
        }
    }

    private void Update()
    {
        CountDownTime();
        CatchInput();
    }

    private void CatchInput()
    {
        if (Input.GetMouseButtonUp(0))
        {
            // get ray from screen
            Ray ray = _checkCam.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit))
            {
                if (hit.collider.CompareTag("Brick"))
                {
                    DoBrickOperation(hit.collider.gameObject);
                }
                else if (hit.collider.CompareTag("QuestionBox"))
                {
                    DoQuestionBoxOperation(hit.collider.gameObject);
                }
            }
        }
    }

    private void DoQuestionBoxOperation(GameObject colliderGameObject)
    {
        _score += PerCoinScore;
        if (ScoreText != null)
        {
            ScoreText.text = _score.ToString().PadLeft(6, '0');
        }
    }

    private void DoBrickOperation(GameObject colliderGameObject)
    {
        Object.Destroy(colliderGameObject);
    }

    private void CountDownTime()
    {
        if (TimeLeftText == null)
        {
            return;
        }
        var nowTime = System.DateTime.Now;
        if (nowTime >= _gameEndTime)
        {
            TimeLeftText.text = "0";
        }
        else
        {
            TimeLeftText.text = ((int)(_gameEndTime - nowTime).TotalSeconds).ToString();
        }
    }

    private void SpawnPrefab(char spot, Vector3 positionToSpawn)
    {
        GameObject ToSpawn = null;

        switch (spot)
        {
            case 'b':
                ToSpawn = Brick;
                Debug.Log("Spawn Brick"); 
                break;
            case '?': 
                Debug.Log("Spawn QuestionBox");
                ToSpawn = QuestionBox;
                break;
            case 'x': 
                Debug.Log("Spawn Rock");
                ToSpawn = Rock;
                break;
            case 's': 
                Debug.Log("Spawn Stone");
                ToSpawn = Stone;
                break;
            //default: Debug.Log("Default Entered"); break;
            default: 
                //ToSpawn = Brick; 
                break;
                // return;
        }

        if (ToSpawn != null)
        {
            ToSpawn = GameObject.Instantiate(ToSpawn, parentTransform);
            ToSpawn.transform.localPosition = positionToSpawn;
        }
    }

    public void RefreshParse()
    {
        GameObject newParent = new GameObject();
        newParent.name = "Environment";
        newParent.transform.position = parentTransform.position;
        newParent.transform.parent = this.transform;
        
        if (parentTransform) Destroy(parentTransform.gameObject);

        parentTransform = newParent.transform;
        FileParser();
    }
}
