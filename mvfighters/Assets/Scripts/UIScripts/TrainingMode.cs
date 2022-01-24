using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TrainingMode : MonoBehaviour
{
    public TMP_Dropdown enemyState;

    public Slider hpControl;

    public Slider enemyHpControl;

    public TMP_Text hpValue;

    public TMP_Text enemyHpValue;

    public void TrainingModeSettingApply()
    {
        hpValue.text = hpControl.value.ToString();
        enemyHpValue.text = enemyHpControl.value.ToString();
        if (!MainScript.instance.um.inGame.comboDisplay.IsComboHappening() && !MainScript.instance.um.inGame.comboDisplay.IsComboDisappearing())
        {
            MainScript.instance.fm.ForceHp(Int32.Parse(hpValue.text), Int32.Parse(enemyHpValue.text));
        }
        MainScript.instance.fm.p2Script.ForceAction(enemyState.options[enemyState.value].text);
    }

    public void ResetPositions()
    {
        MainScript.instance.fm.ResetPositions();
    }

    public void DropDownClick(string name)
    {
        string intValue = Regex.Match(name, @"\d").Value;
        enemyState.SetValueWithoutNotify(Int32.Parse(intValue));
    }
    
}