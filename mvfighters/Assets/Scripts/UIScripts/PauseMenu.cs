using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseMenu : MonoBehaviour
{
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
    }
}
