using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class InGame : MonoBehaviour
{
    private HealthBar p1;
    private HealthBar p2;

    private FighterS player1;
    private FighterS player2;

    public Image hp1;
    public Image hp2;

    public List<Sprite> characterIcons;
    public Image icon1;
    public Image icon2;

    public TMP_Text timerUI;
    private Timer timer;
    public float timerStart;
    public float timerDelay;

    public AudioClip stageMusic;

    public GameObject RoundIcon1;
    public GameObject RoundIcon2;

    public GameObject MatchIcon1;
    public GameObject MatchIcon2;

    private IEnumerator roundEndCoroutine;
    public float timeBeforeRoundReset;

    private bool eventAdded = false;

    public TMP_Text messageOnScreen;
    private IEnumerator roundTextCoroutine;
    private IEnumerator delayedRoundTextCoroutine;

    public List<AudioClip> narratorVoices;

    [HideInInspector] public ComboDisplay comboDisplay;
    public TMP_Text comboCounter;
    public TMP_Text damageCounter;
    private bool comboEvent = false;
    public IEnumerator comboDisplayCoroutine;

    public TMP_Text trainingInfo;

    public void InitiateInGameUI()
    {
        MainS.instance.um.inGameUI.SetActive(true);
        if (player1 == null)
            player1 = MainS.instance.fm.p1Script;
        p1 = new HealthBar(player1.stats.maxHp);
        if (player2 == null)
            player2 = MainS.instance.fm.p2Script;
        p2 = new HealthBar(player2.stats.maxHp);
        icon1.sprite = FindCharacterIcon(player1.stats.name);
        icon2.sprite = FindCharacterIcon(player2.stats.name);
        if (!eventAdded)
        {
            EventManager.AddRoundEndListener(GiveRoundWin);
            eventAdded = true;
        }
        BGMusic.bgmInstance.audio.clip = MainS.instance.um.inGame.stageMusic;
        BGMusic.bgmInstance.audio.Play();
        RoundIcon1.SetActive(false);
        RoundIcon2.SetActive(false);
        MatchIcon1.SetActive(false);
        MatchIcon2.SetActive(false);
        if (!comboEvent)
        {
            comboDisplay = new ComboDisplay();
            EventManager.AddDamageListener(comboDisplay.ComboTrigger);
            comboEvent = true;
        }
        //icon1.sprite = MainS.instance.um.css.cp1.sprite;
        timer = new Timer(timerStart);
        messageOnScreen.enabled = false;
    }

    public IEnumerator RoundStart()
    {
        while (true)
        {
            yield return new WaitForSeconds(timeBeforeRoundReset);
            if (MatchIcon1.activeInHierarchy)
            {
                MainS.instance.fm.MatchOver();
                MainS.instance.um.GoToResultScreen(player1, player2);
            } else if (MatchIcon2.activeInHierarchy)
            {
                MainS.instance.fm.MatchOver();
                MainS.instance.um.GoToResultScreen(player2, player1);
            }
            else
            {
                timer = new Timer(timerStart);
                MainS.instance.fm.RoundStart();
            }
            StopCoroutine(roundEndCoroutine);  
        }
    }

    public void DisplayRoundText(string message)
    {
        messageOnScreen.enabled = true;
        messageOnScreen.text = message;
    }

    public void StartCoroutineRoundText(float time)
    {
        roundTextCoroutine = RoundTextDisappearAfterTime(time);
        StartCoroutine(roundTextCoroutine);
    }
    
    public void StartDelayedCoroutineRoundText(float delayTime, float time, int port, AudioClip clip)
    {
        delayedRoundTextCoroutine = DelayedRoundTextDisappearAfterTime(delayTime, time, port, clip);
        StartCoroutine(delayedRoundTextCoroutine);
    }
    
    public IEnumerator DelayedRoundTextDisappearAfterTime(float delayTime, float time, int port, AudioClip clip)
    {
        while (true)
        {
            yield return new WaitForSeconds(delayTime);
            messageOnScreen.enabled = true;
            messageOnScreen.text = "PLAYER " + port + " WINS!";
            if (port == 1)
                MainS.instance.fm.p1Script.PlayRoundWinAnimation();
            if (port == 2)
                MainS.instance.fm.p2Script.PlayRoundWinAnimation();
            SFXManager.sfxInstance.audio.PlayOneShot(clip);
            StartCoroutineRoundText(time);
            StopCoroutine(delayedRoundTextCoroutine);
        }
    }

    public IEnumerator RoundTextDisappearAfterTime(float time)
    {
        while (true)
        {
            yield return new WaitForSeconds(time);
            messageOnScreen.enabled = false;
            StopCoroutine(roundTextCoroutine);
        }
    }

    public void TurnOffIngameUI()
    {
        MainS.instance.um.inGameUI.SetActive(false);
    }

    public void UpdateInGameUI()
    {
        p1.currentHp = player1.currentHp;
        p2.currentHp = player2.currentHp;

        hp1.fillAmount = p1.GetFillAmount();
        hp2.fillAmount = p2.GetFillAmount();

        if (MainS.instance.fm.roundStart && MainS.instance.state != GameState.TrainingCombat)
            timer.DecreaseTimer(timerDelay);

        if (timer != null)
        {
            int secondLeft = (int) timer.current;
            timerUI.text = secondLeft.ToString();
        }

        if (comboDisplay.IsComboHappening() || comboDisplay.IsComboDisappearing())
        {
            comboCounter.enabled = true;
            damageCounter.enabled = true;
            comboDisplay.UpdateComboCounter(comboCounter, damageCounter);
        }
        else
        {
            comboCounter.enabled = false;
            damageCounter.enabled = false;
        }

        if (MainS.instance.state == GameState.TrainingCombat)
        {
            trainingInfo.enabled = true;
            string maxDamageText = "Max Damage: " + comboDisplay.maxDamage;
            float frameAdvantage = 0;
            float frameAdvantageOnBlock = 0;
            float attackStartup = 0;
            float damage = 0;
            if (MainS.instance.fm.p1Script.currentMove != null)
            {
                Move currentMove = MainS.instance.fm.p1Script.currentMove;
                frameAdvantage = currentMove.hitstun - currentMove.endingFrame;
                frameAdvantageOnBlock = (currentMove.hitstun / 2) - currentMove.endingFrame;
                attackStartup = currentMove.startupFrame;
                damage = currentMove.damage;
            }
            string frameAdvantageBlockText = "Frame advantage (Block): " + frameAdvantageOnBlock;
            string frameAdvantageText = "Frame advantage: " + frameAdvantage;
            string attackStartupText = "Startup: " + attackStartup;
            String damageText = "Current move Damage: " + damage;

            trainingInfo.text = attackStartupText + "\n" + frameAdvantageText + "\n" + frameAdvantageBlockText + "\n" +
                                damageText + "\n" + maxDamageText;
            
            MainS.instance.um.pauseTraining.training.TrainingModeSettingApply();
        }
        else
        {
            trainingInfo.enabled = false;
        }
    }

    public void StartComboDisappearCoroutine(float delay)
    {
        comboDisplay.SetComboDisappearingBoolean(true);
        comboDisplayCoroutine = ComboDisappearCoroutine(delay);
        StartCoroutine(comboDisplayCoroutine);
    }

    private IEnumerator ComboDisappearCoroutine(float delay)
    {
        while (true)
        {
            yield return new WaitForSeconds(delay);
            comboCounter.text = "";
            damageCounter.text = "";
            comboDisplay.SetComboDisappearingBoolean(false);
            StopCoroutine(comboDisplayCoroutine);
        }
    }
    

    public void GiveRoundWin(RoundEndEventArg eventarg)
    {
        switch (eventarg.playerWin)
        {
            case 1:
                if (RoundIcon1.activeInHierarchy)
                    MatchIcon1.SetActive(true);
                else
                    RoundIcon1.SetActive(true);
                break;
            case 2:
                if (RoundIcon2.activeInHierarchy)
                    MatchIcon2.SetActive(true);
                else
                    RoundIcon2.SetActive(true);
                break;
        }
        
        roundEndCoroutine = RoundStart();
        StartCoroutine(roundEndCoroutine);
    }

    public Sprite FindCharacterIcon(string name)
    {
        foreach (var c in characterIcons)
        {
            if (c.name.Contains(name))
            {
                return c;
            }
        }

        return null;
    }
}