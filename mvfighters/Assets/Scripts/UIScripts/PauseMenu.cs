using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseMenu : MonoBehaviour
{
    public GameObject soundPauseUI;
    public GameObject trainingWindow;
    public GameObject buttonPauseUI;
    [HideInInspector] public TrainingMode training;
    [HideInInspector] private string lastSelection;


    public void UpdatePause()
    {
        if (lastSelection == null)
            lastSelection = MainS.instance.um.eventSystem.currentSelectedGameObject.name;

        if (MainS.instance.um.eventSystem.currentSelectedGameObject.name != lastSelection)
        {
            lastSelection = MainS.instance.um.eventSystem.currentSelectedGameObject.name;
            SFXManager.sfxInstance.PlayMoveSound();
        }
        
        if (soundPauseUI.activeInHierarchy)
            MainS.instance.settings.soundPauseSettings.UpdateSoundTextUI();
    }
}