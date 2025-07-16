using UnityEngine;

[CreateAssetMenu(fileName = "InventorySettings", menuName = "Inventory/InventorySettings")]
public class InventorySettings : ScriptableObject
{
  [field: Header("Inventory")]
  [field: SerializeField, Min(1)] public int TotalSlots { get; private set; } = 30;
  [field: SerializeField, Min(0)] public int DefaultUnlockedSlots { get; private set; } = 15;

  [field: Header("Coins")]
  [field: SerializeField, Min(0)] public int CoinsPerSlotUnlock { get; private set; } = 200;
}
