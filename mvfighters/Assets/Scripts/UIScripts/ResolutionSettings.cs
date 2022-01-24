using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ResolutionSettings : MonoBehaviour
{
   public Toggle fullscreenToggle;

   public TMP_Dropdown resolution;
   
   public Toggle vsyncToggle;

   public void DefaultSettings()
   {
      fullscreenToggle.isOn = false;
      resolution.value = 1;
      vsyncToggle.isOn = true;
   }

   public void ApplySetting()
   {
      bool fullscreenBool = fullscreenToggle.isOn;
      Screen.fullScreen = fullscreenBool;
      Screen.fullScreenMode = fullscreenBool ? FullScreenMode.ExclusiveFullScreen : FullScreenMode.Windowed;
      QualitySettings.vSyncCount = vsyncToggle.isOn ? 1 : 0;
      switch (resolution.value)
      {
         case 0 :
            Screen.SetResolution(800,600, fullscreenBool);  
            break;
         case 1 :
            Screen.SetResolution(1280,1024,fullscreenBool);  
            break;
         case 2 :
            Screen.SetResolution(1600,1200,fullscreenBool);  
            break;
         case 3 :
            Screen.SetResolution(1680,1050,fullscreenBool);  
            break;
         case 4 :
            Screen.SetResolution(1920,1200,fullscreenBool);  
            break;
      }
      
   }

   public void ClickDropdown()
   {
      resolution.SetValueWithoutNotify(resolution.value);
      MainScript.instance.um.eventSystem.SetSelectedGameObject(MainScript.instance.um.displayUI.GetComponentInChildren<TMP_Dropdown>()
         .gameObject);
      resolution.Hide();
   }
}
