using System;

[Serializable]
public class InventorySlot
{
  public bool IsUnlocked;

  public InventoryItem Item;

  //===============================

  public bool IsEmpty => Item == null || Item.Count <= 0;

  //===============================

  public void Clear()
  {
    Item = null;
  }

  //===============================
}