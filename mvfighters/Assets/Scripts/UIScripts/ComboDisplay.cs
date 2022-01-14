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
            ResetComboDisplay();
            MainS.instance.um.inGame.StartComboDisappearCoroutine(2f);
        }
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
