using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEditor;

public class Generator : MonoBehaviour
{
    public static Generator Instance;

    [SerializeField] private LevelData _level;
    [SerializeField] public int LevelIndex;
    [SerializeField] public float time;
    [SerializeField] private SpawnCell _cellPrefab;
    [SerializeField] private Transform Map;
    [SerializeField] private int _row, _col;
    private bool gameFinished;
    private SpawnCell[,] pipes;
    private List<SpawnCell> startPipes;


    
    private void Awake(){
        Instance = this;
        gameFinished =false;
        CreateLevelData();
        SpawnLevel();
    }

    private void CreateLevelData(){
        if( _level.Column == _col && _level.Row == _row) return;
        else{
            _level.Row = _row;
            _level.Column = _col;
            _level.Data = new List<int>();
            for(int i=0;i<_row;i++){
                for(int j=0;j<_col;j++){
                    _level.Data.Add(20);
                }
            }
        }
    }

    private void SpawnLevel(){
        pipes = new SpawnCell[_level.Row, _level.Column];
        startPipes = new List<SpawnCell>();

        for (int i=0; i<_level.Row;i++){
            for(int j=0;j<_level.Column;j++){
                Vector2 spawnPos = new Vector2(j+0.5f,i+0.5f);
                SpawnCell tempPipe = Instantiate(_cellPrefab,Map);
                tempPipe.transform.position = spawnPos;
                tempPipe.Init(_level.Data[i * _level.Column + j ]);
                pipes[i,j]= tempPipe;
                if(tempPipe.PipeType ==10||tempPipe.PipeType ==11||tempPipe.PipeType ==12){
                    startPipes.Add(tempPipe);
                }
            }
        }

        Camera.main.orthographicSize = Mathf.Max(_level.Row * 1.25f, _level.Column * 1.25f);
        Vector3 cameraPos = Camera.main.transform.position;
        cameraPos.x = _level.Column *0.5f;
        cameraPos.y = _level.Row * 0.5f;
        Camera.main.transform.position = cameraPos;
        StartCoroutine(ShowHint());
    }

    private void Update()
    {
        if(gameFinished) return;
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        int row = Mathf.FloorToInt(mousePos.y);
        int col = Mathf.FloorToInt(mousePos.x);
        if(row<0||col<0) return;
        if(row>=_level.Row) return;
        if(col>=_level.Column) return;
        if(Input.GetMouseButtonDown(0)){
            pipes[row,col].UpdateInput();
            
        }

        if(Input.GetKeyDown("[0]"))     pipes[row,col].Init(0);
        if(Input.GetKeyDown("[1]"))     pipes[row,col].Init(10);
        if(Input.GetKeyDown("[2]"))     pipes[row,col].Init(20);
        if(Input.GetKeyDown("[3]"))     pipes[row,col].Init(30);
        if(Input.GetKeyDown("[4]"))     pipes[row,col].Init(40);
        if(Input.GetKeyDown("[5]"))     pipes[row,col].Init(50);
        if(Input.GetKeyDown("[6]"))     pipes[row,col].Init(60);
        if(Input.GetKeyDown("[7]"))     pipes[row,col].Init(70);
        if(Input.GetKeyDown("[8]"))     pipes[row,col].Init(80);
        if(Input.GetKeyDown("[9]"))     pipes[row,col].Init(90);
        if(Input.GetKeyDown(KeyCode.UpArrow))     pipes[row,col].Init(100);
        if(Input.GetKeyDown(KeyCode.LeftArrow))     pipes[row,col].Init(110);
        if(Input.GetKeyDown(KeyCode.RightArrow))     pipes[row,col].Init(120);
        StartCoroutine(ShowHint());
    }
    private IEnumerator ShowHint(){
        yield return new WaitForSeconds(0.1f);
        ResetStartPipe();
        CheckFill();
        CheckWin();
        //SaveData();
    }

    private void CheckFill(){
        for(int i=0;i<_level.Row;i++){
            for(int j=0;j<_level.Column;j++){
                SpawnCell tempPipe= pipes[i,j];
                if(tempPipe.PipeType!=0){
                    tempPipe.IsFilled = false;
                }
            }
        }

        Queue<SpawnCell>check = new Queue<SpawnCell>();
        HashSet<SpawnCell>finished = new HashSet<SpawnCell>();
        foreach(var pipe in startPipes){
            check.Enqueue(pipe);
        }
        while(check.Count>0){
            SpawnCell pipe = check.Dequeue();
            finished.Add(pipe);
            List<SpawnCell> connected = pipe.ConnectedPipes();
            foreach (var connectedPipe in connected){
                if(!finished.Contains(connectedPipe)){
                    check.Enqueue(connectedPipe);
                }
            }
            
        }
        foreach (var filled in finished)
        {
            filled.IsFilled = true;
        }

        for (int i = 0; i < _level.Row; i++)
        {
            for (int j = 0; j < _level.Column; j++)
            {
               SpawnCell tempPipe = pipes[i, j];
                tempPipe.UpdateFilled();
            }
        }

      
    }

    private void ResetStartPipe(){
        startPipes= new List<SpawnCell>();

        for(int i=0;i<_level.Row;i++){
            for(int j=0;j<_level.Column;j++){
                if(pipes[i,j].PipeType==10||pipes[i,j].PipeType ==11||pipes[i,j].PipeType ==12){
                    startPipes.Add(pipes[i,j]);
                }
            }
        }
    }

    public void SaveData(){
        for (int i=0;i<_level.Row;i++){
            for(int j=0;j<_level.Column;j++){
                _level.Data[i*_level.Column + j ]= pipes[i,j].PipeData;
            }
        }
        SaveLevel(_level,LevelIndex,time);
        gameFinished =false;
    }
    private void CheckWin(){
        for(int i=0;i<_level.Row;i++){
            for(int j=0;j<_level.Column;j++){
                if(!pipes[i,j].IsFilled) return;
            }
        }
        gameFinished =true;
        StartCoroutine(GameFinished());
    }
    private IEnumerator GameFinished(){
        yield return new WaitForSeconds(2f);
        Debug.Log("win");
    }

    private string GetLevelFilePath(int levelIndex){
        return Path.Combine(Application.persistentDataPath, $"level_{levelIndex}.txt");
    }

    private void SaveLevel(LevelData levelData, int levelIndex,float Time)
    {
        levelData.LevelIndex= levelIndex;
        levelData.time = Time;
        string jsonData = JsonUtility.ToJson(levelData, true);
        string filePath = GetLevelFilePath(levelIndex); 
        File.WriteAllText(filePath, jsonData);
        Debug.Log("Level " + levelIndex + " đã được lưu tại: " + filePath);
    }

   

}

