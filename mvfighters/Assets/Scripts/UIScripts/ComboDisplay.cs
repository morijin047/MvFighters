using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ComboDisplay
{
    private bool combo;
    private float currentCombo;
    private float currentDamage;
    private float timeBeforeReset;
    private bool comboDisappearing = false;
    public float maxDamage;

    public ComboDisplay()
    {
        ResetComboDisplay();
    }
    public void UpdateComboCounter(TMP_Text comboCounter, TMP_Text damageCounter)
    {
        if (!combo) {return;}

        if (currentCombo > 1)
        {
            comboCounter.text = currentCombo.ToString() + " HIT COMBO";
            damageCounter.text = currentDamage.ToString() + " DAMAGE";
        }
        timeBeforeReset--;
        if (timeBeforeReset <= 0)
        {
            if (MainScript.instance.state == GameState.TrainingCombat)
            {
                SaveMaxDamage();
            }
            ResetComboDisplay();
            MainScript.instance.um.inGame.StartComboDisappearCoroutine(2f);
        }
    }

    public void SaveMaxDamage()
    {
        if (maxDamage > currentDamage) { return;}

        maxDamage = currentDamage;
    }

    public bool IsComboHappening()
    {
        return combo;
    }
    
    public bool IsComboDisappearing()
    {
        return comboDisappearing;
    }

    public void ComboTrigger(DamageEventArg arg)
    {
        currentCombo++;
        currentDamage += arg.move.damage;
        timeBeforeReset = arg.move.hitstun;
        combo = true;
        
    }

    public void ResetComboDisplay()
    {
        combo = false;
        currentCombo = 0;
        currentDamage = 0;
        timeBeforeReset = 0;
    }

    public void SetComboDisappearingBoolean(bool newValue)
    {
        comboDisappearing = newValue;
    }
}
