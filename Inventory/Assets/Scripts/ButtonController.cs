using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

public class ButtonController : MonoBehaviour
{
  [Header("Buttons")]
  [SerializeField] private Button _shootButton;
  [SerializeField] private Button _addAmmoButton;
  [SerializeField] private Button _addItemButton;
  [SerializeField] private Button _deleteItemButton;
  [SerializeField] private Button _addCoinsButton;
  [SerializeField] private Button _unlockSlotButton;

  [Header("Text")]
  [SerializeField] private TextMeshProUGUI _cointPerSlotText;

  //-------------------------------

  private InventoryManager inventoryManager;
  private InventoryUI inventoryUI;

  private ItemDatabase itemDatabase;

  private InventorySettings inventorySettings;

  //===============================

  [Inject]
  private void Construct(InventoryManager parInventoryManager, InventoryUI parInventoryUI, ItemDatabase parItemDatabase, InventorySettings parInventorySettings)
  {
    inventoryManager = parInventoryManager;
    inventoryUI = parInventoryUI;
    itemDatabase = parItemDatabase;
    inventorySettings = parInventorySettings;
  }

  //===============================

  private void Start()
  {
    _cointPerSlotText.text = $"Разблокировать слот {inventorySettings.CoinsPerSlotUnlock} монет";
  }

  private void OnEnable()
  {
    _shootButton.onClick.AddListener(OnShoot);
    _addAmmoButton.onClick.AddListener(OnAddAmmo);
    _addItemButton.onClick.AddListener(OnAddRandomItem);
    _deleteItemButton.onClick.AddListener(OnRemoveRandomItem);
    _addCoinsButton.onClick.AddListener(OnAddCoins);
    _unlockSlotButton.onClick.AddListener(OnUnlockSlot);
  }

  private void OnDisable()
  {
    _shootButton.onClick.RemoveListener(OnShoot);
    _addAmmoButton.onClick.RemoveListener(OnAddAmmo);
    _addItemButton.onClick.RemoveListener(OnAddRandomItem);
    _deleteItemButton.onClick.RemoveListener(OnRemoveRandomItem);
    _addCoinsButton.onClick.RemoveListener(OnAddCoins);
    _unlockSlotButton.onClick.RemoveListener(OnUnlockSlot);
  }

  //===============================

  private void OnAddCoins()
  {
    inventoryManager.AddCoins(50);
  }

  private void OnShoot()
  {
    bool success = inventoryManager.TryShoot();

    if (!success)
      Debug.LogWarning("The shot was not executed");

    inventoryUI.UpdateAll();
  }

  private void OnAddAmmo()
  {
    foreach (var ammo in itemDatabase.AllItems)
    {
      if (!(ammo as AmmoConfig))
        continue;

      bool success = inventoryManager.TryAddItem(ammo, 30);

      if (success)
        Debug.Log($"Added 30 ammo: {ammo.ItemType}");
      else
        Debug.LogWarning($"There's no room for ammo: {ammo.ItemName}");
    }

    inventoryUI.UpdateAll();
  }

  private void OnAddRandomItem()
  {
    var item = itemDatabase.GetRandomItem();
    bool success = inventoryManager.TryAddItem(item, 1);

    if (success)
      Debug.Log($"Item added: {item.ItemName}");
    else
      Debug.LogWarning($"There is no place for the item: {item.ItemName}");

    inventoryUI.UpdateAll();
  }

  private void OnRemoveRandomItem()
  {
    bool success = inventoryManager.TryRemoveItemFromRandomSlot();

    if (success)
      Debug.Log($"Random item deleted");
    else
      Debug.LogWarning("Deletion is not possible — all slots are empty");

    inventoryUI.UpdateAll();
  }

  private void OnUnlockSlot()
  {
    bool success = inventoryManager.TryUnlockNextSlot(inventorySettings.CoinsPerSlotUnlock);

    if (success)
    {
      Debug.Log("The slot is unlocked");
      inventoryUI.RefreshUI();
    }
    else
    {
      Debug.LogWarning("Couldn't unlock the slot");
    }
  }

  //===============================
}