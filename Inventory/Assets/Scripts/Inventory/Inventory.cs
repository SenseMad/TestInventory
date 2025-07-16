using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class Inventory : MonoBehaviour
{
  private InventorySettings inventorySettings;

  //===============================

  public List<InventorySlot> Slots { get; private set; } = new();

  //===============================

  [Inject]
  private void Construct(InventorySettings parInventorySettings)
  {
    inventorySettings = parInventorySettings;
  }

  //===============================

  public void Initialize()
  {
    Slots.Clear();

    for (int i = 0; i < inventorySettings.TotalSlots; i++)
	{
      var slot = new InventorySlot
      {
        IsUnlocked = i < inventorySettings.DefaultUnlockedSlots
      };

      Slots.Add(slot);
	}
  }

  //===============================

  public IEnumerable<InventorySlot> UnlockedSlots => Slots.FindAll(s => s.IsUnlocked);

  public int GetUnlockedSlotCound() => Slots.FindAll(s => s.IsUnlocked).Count;

  public float GetTotalWeight()
  {
    float total = 0;
    foreach (var slot in Slots)
    {
      if (!slot.IsEmpty)
        total += slot.Item.TotalWeight;
    }

    return total;
  }

  //===============================
}