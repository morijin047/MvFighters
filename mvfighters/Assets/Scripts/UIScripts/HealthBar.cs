using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthBar
{
   public int currentHp;

   public int maxHp;

   public HealthBar(int maxHp)
   {
      this.maxHp = maxHp;
      currentHp = this.maxHp;
   }

   public float GetFillAmount()
   {
      return (float) currentHp / maxHp;
   }
}
