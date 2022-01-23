using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum MenuSelection
{
   None,
   MainMenu,
   Versus,
   Online,
   Training,
   Gallery,
   Options,
}

public enum VersusSelection
{
   None,
   VsPlayer,
   VsCom,
   ComVsCom
}

public enum OnlineSelection
{
   None,
   RankedMode,
   CreateRoom,
   JoinRoom
}

public enum TrainingSelection
{
   None,
   FreeTraining,
   Tutorial,
   Mission
}

public enum OptionSelection
{
   None,
   SoundSetting,
   SaveVolume,
   MuteVolume,
   BattleSetting,
   DisplaySetting,
   ResetResolution,
   ApplyResolution,
   ButtonSetting
   
}

public enum PauseSelection
{
   None,
   Resume,
   TrainingSetting,
   Reset,
   ButtonSettingPause,
   MoveList,
   SoundSettingPause,
   SaveVolumePause,
   MuteVolumePause,
   CharacterSelect,
   MainMenu
}

public enum ResultSelection
{
   Retry,
   RCharacterSelect,
   RMainMenu
}