using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioControl : MonoBehaviour
{
    private AudioManager audioManager;
    void Update()
    {
         if(audioManager==null) audioManager = FindObjectOfType<AudioManager>();
    }

    public void PlayWinMusic(){
        audioManager.PlayWinMusic();
    }

    public void PlayLoseMusic(){
        audioManager.PlayLoseMusic();
    }

    public void ButtonClick(){
        audioManager.PlayButtonClick();
    }
    public void PlayClick(){
        audioManager.PlayClick();
    }
}
