using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Zenject;
using Random = UnityEngine.Random;

public class InventoryManager : MonoBehaviour
{
  private Inventory inventory;

  private int coins;

  //===============================

  public int Coins
  {
    get => coins;
    private set
    {
      coins = value;
      OnChangeCoins?.Invoke(value);
    }
  }

  //===============================

  [Inject]
  private void Construct(Inventory parInventory)
  {
    inventory = parInventory;
  }

  //===============================

  public event Action<int> OnChangeCoins;
  public event Action OnInventoryChanged;

  //===============================

  private void Awake()
  {
    inventory.Initialize();
  }

  //===============================

  public void Initialize()
  {
    OnInventoryChanged?.Invoke();
  }

  public void InitializeDefaultInventory()
  {
    Coins = 0;

    OnInventoryChanged?.Invoke();
  }

  public void AddCoins(int parAmount)
  {
    Coins += parAmount;
  }

  public void SetCoins(int value)
  {
    Coins = value;
  }

  //===============================

  public InventorySlot GetSlot(int parIndex)
  {
    return inventory.Slots[parIndex];
  }

  public void NotifyInventoryChanged()
  {
    OnInventoryChanged?.Invoke();
  }

  public bool SpendCoins(int parAmount)
  {
    if (Coins >= parAmount)
    {
      Coins -= parAmount;
      return true;
    }

    return false;
  }

  public bool TryShoot()
  {
    var weapons = inventory.UnlockedSlots
      .Where(s => !s.IsEmpty && s.Item.Config is WeaponConfig)
      .Select(s => s.Item.Config as WeaponConfig)
      .ToList();

    if (weapons.Count == 0)
    {
      Debug.LogWarning("The shot is impossible: there is no weapon");
      return false;
    }

    var weapon = weapons[Random.Range(0, weapons.Count)];
    var ammoType = weapon.AmmoType;

    foreach (var slot in inventory.UnlockedSlots)
    {
      if (!slot.IsEmpty && slot.Item.Config is AmmoConfig ammo && ammo.AmmoType == ammoType)
      {
        slot.Item.Count--;

        if (slot.Item.Count <= 0)
          slot.Clear();

        Debug.Log($"A shot from {weapon.ItemName}, ammo: {ammo.ItemType}, damage: {weapon.Damage}");

        OnInventoryChanged?.Invoke();
        return true;
      }
    }

    Debug.LogWarning("The shot is impossible: there are no suitable cartridges.");
    return false;
  }

  public bool TryAddItem(ItemConfig parItemConfig, int parCount = 1)
  {
    bool addedSomething = false;
    List<string> logEntries = new();

    for (int i = 0; i < inventory.Slots.Count; i++)
    {
      var slot = inventory.Slots[i];
      if (!slot.IsUnlocked || slot.IsEmpty)
        continue;

      if (slot.Item.CanStackWith(parItemConfig))
      {
        int spaceLeft = parItemConfig.MaxStack - slot.Item.Count;
        int toAdd = Mathf.Min(spaceLeft, parCount);

        slot.Item.Count += toAdd;
        parCount -= toAdd;
        addedSomething = true;

        logEntries.Add($"Added {toAdd} x {parItemConfig.ItemName} into the slot {i + 1}");

        if (parCount <= 0)
          break;
      }
    }

    for (int i = 0; i < inventory.Slots.Count; i++)
    {
      var slot = inventory.Slots[i];
      if (!slot.IsUnlocked || !slot.IsEmpty)
        continue;

      int toAdd = Mathf.Min(parItemConfig.MaxStack, parCount);

      slot.Item = new InventoryItem
      {
        Config = parItemConfig,
        Count = toAdd
      };

      parCount -= toAdd;
      addedSomething = true;

      logEntries.Add($"Added {toAdd} x {parItemConfig.ItemName} into the slot {i + 1}");

      if (parCount <= 0)
        break;
    }

    if (addedSomething)
    {
      foreach (var log in logEntries)
        Debug.Log(log);

      OnInventoryChanged?.Invoke();
    }

    // If there is not enough space
    return parCount <= 0;
  }

  public bool TryRemoveItemFromRandomSlot()
  {
    var nonEmptySlots = inventory.UnlockedSlots
      .Select((slot, index) => new { slot, index })
      .Where(pair => !pair.slot.IsEmpty)
      .ToList();

    if (nonEmptySlots.Count == 0)
    {
      Debug.LogWarning("All slots are empty - there is nothing to delete");
      return false;
    }

    var selected = nonEmptySlots[Random.Range(0, nonEmptySlots.Count)];
    string name = selected.slot.Item.Config.ItemName;
    Debug.Log($"Item deleted: {name} from the slot {selected.index + 1}");
    selected.slot.Clear();

    OnInventoryChanged?.Invoke();

    return true;
  }

  public bool TryUnlockNextSlot(int parCostPerSlot)
  {
    var locked = inventory.Slots.FirstOrDefault(s => !s.IsUnlocked);
    if (locked == null)
    {
      Debug.Log("All slots are already unlocked");
      return false;
    }

    if (SpendCoins(parCostPerSlot))
    {
      locked.IsUnlocked = true;
      Debug.Log($"The slot is unlocked for {parCostPerSlot} coins");
      return true;
    }

    Debug.LogWarning("Not enough coins to unlock");
    return false;
  }

  public float GetTotalWeight() => inventory.GetTotalWeight();

  public List<InventorySlot> GetAllSlots() => inventory.Slots;

  //===============================
}