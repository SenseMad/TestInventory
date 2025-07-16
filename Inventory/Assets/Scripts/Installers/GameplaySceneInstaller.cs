using Zenject;

public class GameplaySceneInstaller : MonoInstaller
{
  public override void InstallBindings()
  {
    Container.Bind<Inventory>().FromNewComponentOnNewGameObject().AsSingle().NonLazy();

    Container.Bind<InventoryManager>().FromNewComponentOnNewGameObject().AsSingle().NonLazy();

    Container.Bind<SaveManager>().FromNewComponentOnNewGameObject().AsSingle().NonLazy();

    Container.Bind<InventoryUI>().FromComponentInHierarchy().AsSingle().NonLazy();

    Container.Bind<ItemInfoPopup>().FromComponentInHierarchy().AsSingle().NonLazy();
  }
}