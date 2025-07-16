using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Zenject;

public class InventoryUI : MonoBehaviour
{
  [SerializeField] private InventorySlotUI _slotUIPrefab;

  [SerializeField] private Transform _slotsContainer;

  [Space]
  [SerializeField] private RectTransform _dragIconParent;
  [SerializeField] private Image _dragIcon;

  [Header("Texts")]
  [SerializeField] private TextMeshProUGUI _balanceText;
  [SerializeField] private TextMeshProUGUI _weightText;

  //-------------------------------

  private Inventory inventory;
  private InventoryManager inventoryManager;

  private ItemInfoPopup itemInfoPopup;

  private List<InventorySlotUI> slotUIs = new();

  private InventorySlotUI draggingFromSlot;

  //===============================

  [Inject]
  private void Construct(Inventory parInventory, InventoryManager parInventoryManager, ItemInfoPopup parItemInfoPopup)
  {
    inventory = parInventory;
    inventoryManager = parInventoryManager;
    itemInfoPopup = parItemInfoPopup;
  }

  //===============================

  private void Start()
  {
    RefreshUI();
  }

  private void OnEnable()
  {
    inventoryManager.OnChangeCoins += InventoryManager_OnChangeCoins;

    inventoryManager.OnInventoryChanged += InventoryManager_OnInventoryChanged;
  }

  private void OnDisable()
  {
    inventoryManager.OnChangeCoins -= InventoryManager_OnChangeCoins;

    inventoryManager.OnInventoryChanged += InventoryManager_OnInventoryChanged;
  }

  //===============================

  public void RefreshUI()
  {
	Clear();

    foreach (var slot in inventory.Slots)
    {
      var slotUI = Instantiate(_slotUIPrefab, _slotsContainer);
      slotUI.Setup(slot, this);
      slotUIs.Add(slotUI);
    }
  }

  public void UpdateAll()
  {
	foreach (var slotUI in slotUIs)
	{
	  if (slotUI == null)
		continue;

	  slotUI.Refresh();
    }
  }

  public void StartDrag(InventorySlotUI parFromSlotUI, PointerEventData parEventData)
  {
    draggingFromSlot = parFromSlotUI;
    _dragIcon.sprite = parFromSlotUI.GetIcon();
    _dragIcon.gameObject.SetActive(true);
    _dragIcon.enabled = true;

    RectTransformUtility.ScreenPointToLocalPointInRectangle(
        _dragIconParent, parEventData.position, parEventData.pressEventCamera, out Vector2 localPoint);

    _dragIcon.rectTransform.localPosition = localPoint;
  }

  public void UpdateDrag(PointerEventData parEventData)
  {
    if (!_dragIcon.enabled)
      return;

    RectTransformUtility.ScreenPointToLocalPointInRectangle(
        _dragIconParent, parEventData.position, parEventData.pressEventCamera, out Vector2 localPoint);

    _dragIcon.rectTransform.localPosition = localPoint;
  }

  public void EndDrag(InventorySlotUI parToSlotUI, PointerEventData parEventData)
  {
    _dragIcon.gameObject.SetActive(false);
    _dragIcon.enabled = false;

    if (draggingFromSlot == null)
      return;

    bool moved = false;

    if (parToSlotUI != null && parToSlotUI != draggingFromSlot)
      moved = HandleItemTransfer(draggingFromSlot, parToSlotUI);

    if (!moved)
      draggingFromSlot.RestoreIconVisibility();

    draggingFromSlot = null;
  }

  public void ShowItemInfoPopup(InventoryItem parItem)
  {
    Debug.Log($"{parItem}");

    if (parItem == null || parItem.Config == null)
      return;

    itemInfoPopup.Show(parItem.Config, parItem.Count);
  }

  //===============================

  private void Clear()
  {
	foreach (var slotUI in slotUIs)
	{
      if (slotUI == null)
        continue;

      Destroy(slotUI.gameObject);
	}

	slotUIs.Clear();
  }

  private void InventoryManager_OnChangeCoins(int parValue)
  {
    _balanceText.text = $"Баланс: {parValue} монет";
  }

  private void InventoryManager_OnInventoryChanged()
  {
    _weightText.text = $"Вес: {inventoryManager.GetTotalWeight():0.00} кг";
  }

  private bool HandleItemTransfer(InventorySlotUI parFromSlotUI, InventorySlotUI parToSlotUI)
  {
    var fromSlot = parFromSlotUI.Slot;
    var toSlot = parToSlotUI.Slot;

    if (fromSlot.IsEmpty)
      return false;

    if (!toSlot.IsUnlocked)
      return false;

    if (toSlot.IsEmpty)
    {
      toSlot.Item = fromSlot.Item;
      fromSlot.Clear();
    }
    else if (toSlot.Item.CanStackWith(fromSlot.Item.Config))
    {
      int space = fromSlot.Item.Config.MaxStack - toSlot.Item.Count;
      int moveCount = Mathf.Min(space, fromSlot.Item.Count);

      toSlot.Item.Count += moveCount;
      fromSlot.Item.Count -= moveCount;

      if (fromSlot.Item.Count <= 0)
        fromSlot.Clear();
    }
    else
    {
      var temp = fromSlot.Item;
      fromSlot.Item = toSlot.Item;
      toSlot.Item = temp;
    }

    parFromSlotUI.Refresh();
    parToSlotUI.Refresh();

    inventoryManager.NotifyInventoryChanged();

    return true;
  }

  //===============================
}