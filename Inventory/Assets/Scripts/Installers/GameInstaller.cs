using UnityEngine;
using Zenject;

[CreateAssetMenu(fileName = "GameInstaller", menuName = "Installer/GameInstaller")]
public class GameInstaller : ScriptableObjectInstaller<GameInstaller>
{
  [field: SerializeField] public ItemDatabase ItemDatabase { get; private set; }
  [field: SerializeField] public InventorySettings InventorySettings { get; private set; }

  public override void InstallBindings()
  {
    Container.BindInstance(ItemDatabase).AsSingle();
    Container.BindInstance(InventorySettings).AsSingle();
  }
}