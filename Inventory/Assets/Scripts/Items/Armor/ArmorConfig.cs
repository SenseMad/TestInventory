using UnityEngine;

[CreateAssetMenu(menuName = "Inventory/ArmorConfig")]
public class ArmorConfig : ItemConfig
{
  [field: SerializeField] public int Defense { get; private set; }
}