using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


[System.Serializable] public class LevelDataSerializable
{
    public int LevelIndex;
    public float time;
    public int Row;
    public int Column;
    public List<int> Data;
}
public class LevelMenu : MonoBehaviour
{
    [SerializeField] public LevelData _level;
    [SerializeField] private Sprite[] StarSprite;
    public Button[] buttons;
    private void Start()
    {
        int unLokedLv= PlayerPrefs.GetInt("UnLockedLevel",1);
        for(int i=0;i<buttons.Length;i++){
            buttons[i].interactable=false;
        }
        for(int i=0;i<Mathf.Min(unLokedLv, buttons.Length);i++){
            buttons[i].interactable=true;
            int stars = PlayerPrefs.GetInt("stars"+i.ToString(),0);
            buttons[i].transform.GetChild(1).GetComponent<Image>().sprite = StarSprite[stars];
        }
    }
    public void OpenLevel(int level){
        LevelData levelData = LoadLevel(level); 
        if (levelData != null)
        {
            _level.LevelIndex = levelData.LevelIndex;
            _level.time = levelData.time;
            _level.Row = levelData.Row;
            _level.Column = levelData.Column;
            _level.Data = new List<int>(levelData.Data);

            Debug.Log("Tải level " + level + " thành công!");
            SceneManager.LoadScene("Level");
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
            _level.time = serializableData.time;
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
