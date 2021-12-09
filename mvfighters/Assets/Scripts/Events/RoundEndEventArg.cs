using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoundEndEventArg
{
   public int playerWin;
   public RoundEndEventArg(int playerWin)
   {
      this.playerWin = playerWin;
   }
}

public delegate void RoundEndListener(RoundEndEventArg eventArg);

