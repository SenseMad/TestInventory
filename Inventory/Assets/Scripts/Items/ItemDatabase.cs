using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ItemDatabase", menuName = "Inventory/ItemDatabase")]
public class ItemDatabase : ScriptableObject
{
  [field: SerializeField] public List<ItemConfig> AllItems { get; private set; }

  public ItemConfig GetRandomItem() => AllItems[Random.Range(0, AllItems.Count)];

  public ItemConfig GetByID(string id)
  {
    foreach (var item in AllItems)
    {
      if (item.ID == id)
        return item;
    }

    Debug.LogWarning($"Item ID '{id}' not found!");
    return null;
  }
}