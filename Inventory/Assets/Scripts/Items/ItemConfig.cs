using UnityEngine;

public abstract class ItemConfig : ScriptableObject
{
  [field: SerializeField] public string ID { get; private set; }

  [field: SerializeField] public string ItemName { get; private set; }

  [field: SerializeField] public ItemType ItemType { get; private set; }

  [field: SerializeField] public Sprite Icon { get; private set; }

  [field: SerializeField, Min(0)] public float Weight { get; private set; }

  [field: SerializeField, Min(0)] public int MaxStack { get; private set; }
}