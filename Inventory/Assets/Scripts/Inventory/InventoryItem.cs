using System;

[Serializable]
public class InventoryItem
{
  public ItemConfig Config;

  public int Count;

  //===============================

  public float TotalWeight => Config != null ? Config.Weight * Count : 0;

  public bool IsStackable => Config != null && Config.MaxStack > 1;

  public bool IsFull => Config != null && Count >= Config.MaxStack;

  //===============================

  public bool CanStackWith(ItemConfig parConfig)
  {
    return Config == parConfig && IsStackable && !IsFull;
  }

  //===============================
}