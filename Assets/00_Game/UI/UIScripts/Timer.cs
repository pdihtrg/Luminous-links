using System.Collections;
using System.Collections.Generic;
using TMPro;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class Timer : MonoBehaviour
{
   [SerializeField] TextMeshProUGUI timerText;
   [SerializeField] TextMeshProUGUI winTimeText;
   [SerializeField] TextMeshProUGUI loseTimeText;
   [SerializeField] public float remainingTime;
   [SerializeField] private Image uiFillImage;
   [SerializeField] private Image origianlImage;
   [SerializeField] private GameManager gameManager;
    public float duration;
    void Start(){
        duration = remainingTime;
        if (gameManager == null)  gameManager = FindObjectOfType<GameManager>();
    
    }

   void Update()
   {    
    if(remainingTime>=1) {
        if(!gameManager.gameFinished){
        remainingTime -=Time.deltaTime;
         float targetFill = Mathf.Clamp(remainingTime / duration, 0, 1);
        uiFillImage.DOFillAmount(targetFill, 0.4f).SetEase(Ease.OutQuad);
        }
    }
    if(remainingTime<=duration/4) {
        Color doNhat = new Color(255f / 255f, 105f / 255f, 105f / 255f);
        Color doDam = new Color(109f / 255f, 35f / 255f, 35f / 255f);
        timerText.color = doNhat;
        uiFillImage.color = doNhat;
        origianlImage.color = doDam;
        }
    int minutes = Mathf.FloorToInt(remainingTime/60);
    int seconds = Mathf.FloorToInt(remainingTime%60);
    timerText.text= string.Format("{0:00}:{1:00}",minutes,seconds);

        
   }
   public int GetStar(){
        if(remainingTime>= (duration/2) ) return 3;
        if(remainingTime>= (duration/4) ) return 2;
        if(remainingTime>= (duration/6) ) return 1;
        return 0;
   }

    public void ShowWinTime(){
        float finalTime = duration-remainingTime;
        StartCoroutine(AnimTime(winTimeText,finalTime));

    }
    public void ShowLoseTime(){
        float finalTime = duration-remainingTime;
        StartCoroutine(AnimTime(loseTimeText,finalTime));
    }

    IEnumerator AnimTime(TMP_Text timerUI, float finalTime){
        float displayTime = 0;
        DOTween.To(() => displayTime, x => displayTime = x, finalTime, 3.5f)
       .OnUpdate(() => 
       {
           int minutes = Mathf.FloorToInt(displayTime / 60);
           int seconds = Mathf.FloorToInt(displayTime % 60);
           int milliseconds = Mathf.FloorToInt((displayTime * 100) % 100);
           timerUI.text = $"{minutes:00}:{seconds:00}:{milliseconds:00}";
       });
        yield return new WaitForSeconds(0.05f);
    }
}
