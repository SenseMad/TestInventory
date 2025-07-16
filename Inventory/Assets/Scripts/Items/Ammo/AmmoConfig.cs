using UnityEngine;

[CreateAssetMenu(menuName = "Inventory/AmmoConfig")]
public class AmmoConfig : ItemConfig
{
  [field: SerializeField] public AmmoType AmmoType { get; private set; }
}