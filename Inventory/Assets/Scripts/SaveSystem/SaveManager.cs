using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class SaveManager : MonoBehaviour
{
  private InventoryManager inventoryManager;
  private ItemDatabase itemDatabase;

  //===============================

  [Inject]
  private void Construct(InventoryManager parInventoryManager, ItemDatabase parItemDatabase)
  {
    inventoryManager = parInventoryManager;
    itemDatabase = parItemDatabase;
  }

  //===============================

  private void Start()
  {
    Load();
  }

  private void OnApplicationQuit()
  {
    Save();
  }

  private void OnApplicationPause(bool pause)
  {
    if (pause)
      Save();
  }

  //===============================

  public void Save()
  {
    var saveData = new SaveData
    {
      Coins = inventoryManager.Coins,
      Slots = new List<SlotData>()
    };

    foreach (var slot in inventoryManager.GetAllSlots())
    {
      var data = new SlotData
      {
        IsUnlocked = slot.IsUnlocked
      };

      if (!slot.IsEmpty)
      {
        data.ItemID = slot.Item.Config.ID;
        data.Count = slot.Item.Count;
      }

      saveData.Slots.Add(data);
    }

    JsonSaveHandler.Save(saveData);
    Debug.Log("Save completed");
  }

  public void Load()
  {
    var saveData = JsonSaveHandler.Load();
    if (saveData == null)
    {
      Debug.Log("No saves found — default initialization");
      inventoryManager.InitializeDefaultInventory();
      return;
    }

    var slots = inventoryManager.GetAllSlots();
    for (int i = 0; i < saveData.Slots.Count && i < slots.Count; i++)
    {
      var data = saveData.Slots[i];
      var slot = slots[i];
      slot.IsUnlocked = data.IsUnlocked;

      if (!string.IsNullOrEmpty(data.ItemID))
      {
        var item = itemDatabase.GetByID(data.ItemID);
        slot.Item = new InventoryItem
        {
          Config = item,
          Count = data.Count
        };
      }
      else
      {
        slot.Clear();
      }
    }

    inventoryManager.SetCoins(saveData.Coins);
    inventoryManager.Initialize();
    Debug.Log("Download completed");
  }

  //===============================
}