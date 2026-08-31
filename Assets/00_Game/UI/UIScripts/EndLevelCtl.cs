using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using DG.Tweening;
using UnityEngine.UI;
using System.IO;
   
public class EndLevelCtl : MonoBehaviour
{
    [SerializeField] public LevelData _level;
    [SerializeField] public GameManager gameManager;
    
    [SerializeField] private Transform[] stars;
    [SerializeField] private Sprite starGray;
    [SerializeField] private Sprite starGold; 

    public CanvasGroup canvasGroup;
    public Transform panelTransform;
    
    //anim
    public void ShowPanel(){
        canvasGroup.alpha = 0;
        panelTransform.localScale = Vector3.zero;
        Debug.Log("Chạy anim");
        Sequence sequence = DOTween.Sequence();
        sequence.AppendInterval(0.2f);
        sequence.Append(panelTransform.DOScale(1f, 0.6f).SetEase(Ease.OutBack));
        // Hiệu ứng fade in
        sequence.Join(canvasGroup.DOFade(1, 0.5f));
        Debug.Log("Chạy xong anim");
    }
    
    //anim star
    public void ShowStars(int starCount){
        if (starCount > stars.Length)
            starCount = stars.Length; 
        Debug.Log("Chạy anim star");
        StartCoroutine(StarsRoutine(starCount));
    }

    // Hiển thị sao theo số lượng
    public IEnumerator StarsRoutine(int starCount){

        for(int i = 0; i < starCount; i++){
            Image starImage = stars[i].GetComponent<Image>();
            starImage.sprite = starGold;
        }
        for (int i = 0; i < 3; i++){
            Image starImage = stars[i].GetComponent<Image>(); 
            stars[i].localScale = Vector3.zero; 
           
            stars[i].gameObject.SetActive(true);
            starImage.DOFade(1, 0.7f);
             stars[i].DOScale(4f, 0.9f).SetEase(Ease.OutBack);

            yield return new WaitForSeconds(0.4f);
        }
        Debug.Log("Chạy xong ainm star");
    }
   
    
    
    
    
    //button
    public void Home(){
        SceneManager.LoadScene("MainMenu");
        Time.timeScale = 1;
    }

    public void Next(){
        gameManager.ClearMap();
        OpenLevel(_level.LevelIndex+1);
    }
    public void Restart(){
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        Time.timeScale = 1;
    }
    public void TryAgain(){
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

     public void OpenLevel(int level){
        Debug.Log("Chuẩn bị Load level " + level );
        LevelData levelData = LoadLevel(level);
        if (levelData != null)
        {
            _level.LevelIndex = levelData.LevelIndex;
            _level.time = levelData.time;
            _level.Row = levelData.Row; 
            _level.Column = levelData.Column;
            _level.Data = new List<int>(levelData.Data);

            Debug.Log("Tải level " + level + " thành công!");
            gameManager.LoadNewMap();
        }
        else
        {
            Debug.LogError("Không thể tải level " + level + "!");
        }
            
    }

     public string GetLevelFilePath(int levelIndex){
        return Path.Combine(Application.persistentDataPath, $"level_{levelIndex}.txt");
        
        
    }
    public LevelData LoadLevel(int levelIndex){

        string filePath = GetLevelFilePath(levelIndex);
        
        if (File.Exists(filePath))
        {
            string jsonData = File.ReadAllText(filePath);
            LevelDataSerializable serializableData = JsonUtility.FromJson<LevelDataSerializable>(jsonData);
            _level.LevelIndex = serializableData.LevelIndex;
            _level.time= serializableData.time;
            _level.Row = serializableData.Row;
            _level.Column = serializableData.Column;
            _level.Data = new List<int>(serializableData.Data);
            Debug.Log("Level " + levelIndex + " đã được tải từ: " + filePath);
            return _level; 
        }
        else
        {
            TextAsset mapFile = Resources.Load<TextAsset>("Maps/level_" + levelIndex);
            if (mapFile != null)
            {
                File.WriteAllText(filePath, mapFile.text);
                Debug.Log("Đã copy map từ Resources vào: " + filePath);
                
                string jsonData = File.ReadAllText(filePath);
                LevelDataSerializable serializableData = JsonUtility.FromJson<LevelDataSerializable>(jsonData);
                _level.LevelIndex = serializableData.LevelIndex;
                _level.time= serializableData.time;
                _level.Row = serializableData.Row;
                _level.Column = serializableData.Column;
                _level.Data = new List<int>(serializableData.Data);
                Debug.Log("Level " + levelIndex + " đã được tải từ: " + filePath);
                return _level; 
                
            }
            else
            {
                Debug.LogError("Không tìm thấy file map trong Resources!");
            }
            return null;
        }
    }
    
}
