using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;


public class settingMenu : MonoBehaviour
{
    [SerializeField] private AudioMixer myMixer;
    [SerializeField] private Slider musicSlider;
    [SerializeField] private Slider SFXSlider;
     [SerializeField] GameObject SettingMenu;
    
    private float MusicVolume, SFXVolume;
    

    public void Start()
    {
       
        if(PlayerPrefs.HasKey("musicVolume")) LoadVolume();
        // else {
        //     SetMusicVolume();
        //     SetSFXVolume();
        // }

    }
    public void SetMusicVolume(){
        MusicVolume = musicSlider.value;
        myMixer.SetFloat("music",Mathf.Log10(MusicVolume)*20);

    }

    public void SetSFXVolume(){
        SFXVolume = SFXSlider.value;
        myMixer.SetFloat("SFX",Mathf.Log10(SFXVolume)*20);

    }
    public void Setting(){
        SettingMenu.SetActive(true);
        Time.timeScale = 0;
    }
    public void Apply(){
        PlayerPrefs.SetFloat("musicVolume",MusicVolume);
        PlayerPrefs.SetFloat("SFXVolume",SFXVolume);
        Time.timeScale = 1;
        SettingMenu.SetActive(false);
    }


    public void LoadVolume(){
        musicSlider.value = PlayerPrefs.GetFloat("musicVolume");
        SFXSlider.value = PlayerPrefs.GetFloat("SFXVolume");
        SetMusicVolume();
        SetSFXVolume();
        Time.timeScale = 1;
        SettingMenu.SetActive(false);
    }
}
