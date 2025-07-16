using UnityEngine;

[CreateAssetMenu(menuName = "Inventory/WeaponConfig")]
public class WeaponConfig : ItemConfig
{
  [field: SerializeField] public AmmoType AmmoType { get; private set; }

  [field: SerializeField] public int Damage { get; private set; }
}