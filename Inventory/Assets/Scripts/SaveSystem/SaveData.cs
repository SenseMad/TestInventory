using System.Collections.Generic;
using System;

[Serializable]
public class SaveData
{
  public int Coins;

  public List<SlotData> Slots = new();
}