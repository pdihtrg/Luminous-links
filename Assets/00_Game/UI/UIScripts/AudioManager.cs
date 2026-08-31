using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class AudioManager : MonoBehaviour

{
    
    public static AudioManager instance;
    [Header("---------Audio Source---------")]
    [SerializeField] AudioSource musicSource;
    [SerializeField] AudioSource SFXSource;
    //[SerializeField] AudioSource SFXSource;
    [Header("---------Audio Clip---------")]
    public AudioClip background;
    public AudioClip click;
    public AudioClip win;
    public AudioClip lose;
    public AudioClip buttonClick;


    private void Awake(){
        if (instance == null){
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else{
            Destroy(gameObject);
            return;
        }

        musicSource.clip = background;
        musicSource.loop = true; 
        musicSource.Play(); 
    }

    public void PlaySFX(AudioClip clip){
        SFXSource.PlayOneShot(clip);
    }

     public void PlayWinMusic(){
        musicSource.DOFade(0.1f,1f);
        SFXSource.PlayOneShot(win);
        DOVirtual.DelayedCall(win.length, () => 
            {
                musicSource.DOFade(1f, 1f);
            });

    }

    public void PlayLoseMusic(){
        musicSource.DOFade(0.1f,1f);
        SFXSource.PlayOneShot(lose);
        DOVirtual.DelayedCall(lose.length, () => 
            {
                musicSource.DOFade(1f, 1f);
            });
    }
    public void PlayClick(){
        SFXSource.PlayOneShot(click);
    }

    public void PlayButtonClick(){
         SFXSource.PlayOneShot(buttonClick);
    }

   
    




}
