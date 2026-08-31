using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.SocialPlatforms;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [SerializeField] private LevelData _level;
    [SerializeField] private Pipe _cellPrefab;
    [SerializeField] private Transform Map;
    [SerializeField] private Timer timers;
     [SerializeField] private GameObject winPanel;
    [SerializeField] private GameObject losePanel;
    [SerializeField] private GameObject gameManager;
    private AudioControl AudioCtrl;


    public EndLevelCtl endLvCtr;
    public bool gameFinished;
    private Pipe[,] pipes;
    private List<Pipe> startPipes;
    private void Awake(){
        Instance = this;
        gameFinished = false;
        winPanel.SetActive(false);
        losePanel.SetActive(false);
        if(timers== null) timers = FindObjectOfType<Timer>();
        if(AudioCtrl==null) AudioCtrl =  FindObjectOfType<AudioControl>();
        timers.remainingTime = _level.time;
        SpawnLevel();
        StartCoroutine(ShowHint());

    }

    private void SpawnLevel(){
        pipes = new Pipe[_level.Row, _level.Column];
        startPipes = new List<Pipe>();

        for (int i=0; i<_level.Row;i++){
            for(int j=0;j<_level.Column;j++){
                Vector2 spawnPos = new Vector2(j+0.5f,i+0.5f);
                Pipe tempPipe = Instantiate(_cellPrefab,Map);
                tempPipe.transform.position = spawnPos;
                tempPipe.Init(_level.Data[i * _level.Column + j ]);
                pipes[i,j]= tempPipe;
                if(tempPipe.PipeType ==10||tempPipe.PipeType ==11||tempPipe.PipeType ==12){
                    startPipes.Add(tempPipe);
                }
            }
        }

        Camera.main.orthographicSize = Mathf.Min(_level.Row * 1.23f, _level.Column * 1.23f);;
        Vector3 cameraPos = Camera.main.transform.position;
        cameraPos.x = _level.Column *0.5f;
        cameraPos.y = _level.Row * 0.5f;
        Camera.main.transform.position = cameraPos;
    }

    private void Update()
    {
        if(gameFinished) return;
        if(Time.timeScale==0) return;
        Vector3 mousePos;
        #if UNITY_EDITOR
        mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        #elif UNITY_ANDROID || UNITY_IOS
        mousePos = Camera.main.ScreenToWorldPoint(Input.GetTouch(0).position);
        #endif
        
        int row = Mathf.FloorToInt(mousePos.y);
        int col = Mathf.FloorToInt(mousePos.x);
        if(row<0||col<0) return;
        if(row>=_level.Row) return;
        if(col>=_level.Column) return;
        if(timers.remainingTime<1){
            gameFinished =true;
            timers.remainingTime=0f;
            Debug.Log("Lose");
            LoseGame();
        }
        if(Input.GetMouseButtonDown(0)){
            AudioCtrl.PlayClick();
            pipes[row,col].UpdateInput();
            StartCoroutine(ShowHintDelay());
        }
    }
    private IEnumerator ShowHintDelay(){
        yield return new WaitForSeconds(0.25f);
        CheckFill();
        CheckWin();
    }

    private IEnumerator ShowHint(){
        yield return new WaitForSeconds(0.1f);
        CheckFill();
        CheckWin();
    }


    private void CheckFill(){
        for(int i=0;i<_level.Row;i++){
            for(int j=0;j<_level.Column;j++){
                Pipe tempPipe= pipes[i,j];
                if(tempPipe.PipeType!=0){
                    tempPipe.IsFilled = false;
                }
            }
        }

        Queue<Pipe>check = new Queue<Pipe>();
        HashSet<Pipe>finished = new HashSet<Pipe>();
        foreach(var pipe in startPipes){
            check.Enqueue(pipe);
        }
        while(check.Count>0){
            Pipe pipe = check.Dequeue();
            finished.Add(pipe);
            List<Pipe> connected = pipe.ConnectedPipes();
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
                Pipe tempPipe = pipes[i, j];
                tempPipe.UpdateFilled();
            }
        }

      
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
        yield return new WaitForSeconds(1f);
        Debug.Log("win");
        UnLockNewLevel();
        WinGame();
    }

    void UnLockNewLevel(){
        if(_level.LevelIndex>= PlayerPrefs.GetInt("ReachedIndex")){
            int reachedIndex = _level.LevelIndex-1;
            PlayerPrefs.SetInt("ReachedIndex",_level.LevelIndex+1);
            PlayerPrefs.SetInt("UnLockedLevel",PlayerPrefs.GetInt("UnLockedLevel",1)+1);
            if(timers.GetStar()>PlayerPrefs.GetInt("stars"+reachedIndex.ToString(),0)){
                PlayerPrefs.SetInt("stars"+reachedIndex.ToString(),timers.GetStar());
            }
            PlayerPrefs.Save();
        }
        else{
            int reachedIndex = _level.LevelIndex-1;
             if(timers.GetStar()>PlayerPrefs.GetInt("stars"+reachedIndex.ToString(),0)){
                PlayerPrefs.SetInt("stars"+reachedIndex.ToString(),timers.GetStar());
            }
            PlayerPrefs.Save();
        }
    }

    public void WinGame(){
        winPanel.SetActive(true);
        AudioCtrl.PlayWinMusic();
        endLvCtr.ShowPanel();
        endLvCtr.ShowStars(timers.GetStar());
        timers.ShowWinTime();
    }

    public void LoseGame(){
        losePanel.SetActive(true);
        AudioCtrl.PlayLoseMusic();
        endLvCtr.ShowPanel();
        timers.ShowLoseTime();
    }

    
    public void LoadNewMap(){
        gameFinished = false;
        winPanel.SetActive(false);
        losePanel.SetActive(false);
        timers.duration = _level.time;
        timers.remainingTime = _level.time;
        SpawnLevel();
        StartCoroutine(ShowHint());
        Debug.Log("Map mới đã được load");
    }

    public void ClearMap(){
        if (pipes != null){
            for (int i = 0; i < _level.Row; i++){
                for (int j = 0; j < _level.Column; j++){
                    if (pipes[i, j] != null){
                        Destroy(pipes[i, j].gameObject);
                        pipes[i, j] = null;
                    }
                }
            }
        }
        if (startPipes != null){
            startPipes.Clear();
        } 
        Debug.Log("Map cũ đã được xóa.");
    }

}
