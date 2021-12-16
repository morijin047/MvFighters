using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum MenuSelection
{
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
   BattleSetting,
   DisplaySetting,
   ButtonSetting
}
