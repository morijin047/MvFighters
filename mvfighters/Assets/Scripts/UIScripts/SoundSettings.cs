using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SoundSettings : MonoBehaviour
{
    public Slider masterVolume;

    public TMP_Text masterValue;
   
    public Slider bgmVolume;

    public TMP_Text bgmValue;
   
    public Slider sfxVolume;

    public TMP_Text sfxValue;
   
    public Slider voiceVolume;

    public TMP_Text voiceValue;
    
    public void OpenSoundSettings()
    {
        bgmVolume.value = BGMusic.bgmInstance.audio.volume;
        sfxVolume.value = SFXManager.sfxInstance.audio.volume;
      
    }
    
    public void UpdateSoundTextUI()
    {
        int bgmInteger = (int)(bgmVolume.value * 100);
        bgmValue.text = bgmInteger.ToString();
        int sfxInteger = (int)(sfxVolume.value * 100);
        sfxValue.text = sfxInteger.ToString();
        int masterInteger = (int)(masterVolume.value * 100);
        masterValue.text = masterInteger.ToString();
        int voiceInteger = (int)(voiceVolume.value * 100);
        voiceValue.text = voiceInteger.ToString();
    }
    
    public void ApplySoundSettings()
    {
        BGMusic.bgmInstance.audio.volume = bgmVolume.value;

        SFXManager.sfxInstance.audio.volume = sfxVolume.value;
      
        //Apply Master volume
      
        //Apply Voice volume
    }
    
    public void MuteSoundSettings()
    {
        bgmVolume.value = 0f;
        sfxVolume.value = 0f;
    }
}
