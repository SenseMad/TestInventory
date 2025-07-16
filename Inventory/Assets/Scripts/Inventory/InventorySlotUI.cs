using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class InventorySlotUI : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IPointerClickHandler
{
  [SerializeField] private Image _iconImage;

  [SerializeField] private TextMeshProUGUI _countText;

  [SerializeField] private GameObject _lockOverlay;

  //-------------------------------

  private InventoryUI inventoryUI;

  //===============================

  public InventorySlot Slot { get; private set; }

  //===============================

  public void Setup(InventorySlot parSlot, InventoryUI parInventoryUI)
  {
    Slot = parSlot;
    inventoryUI = parInventoryUI;

    UpdateVisual();
  }

  public void UpdateVisual()
  {
    if (Slot != null && !Slot.IsEmpty)
    {
      _iconImage.sprite = Slot.Item.Config.Icon;
      _iconImage.enabled = true;
      _countText.text = Slot.Item.Count > 1 ? Slot.Item.Count.ToString() : "";
    }
    else
    {
      _iconImage.enabled = false;
      _countText.text = "";
    }

    _lockOverlay.SetActive(Slot != null && !Slot.IsUnlocked);
  }

  public void SetSlot(InventorySlot parSlot)
  {
    Slot = parSlot;
    Refresh();
  }

  public void Refresh()
  {
    if (Slot == null)
    {
      _iconImage.enabled = false;
      _countText.text = "";
      _lockOverlay.SetActive(false);
      return;
    }

    _lockOverlay.SetActive(!Slot.IsUnlocked);

    if (Slot.IsEmpty)
    {
      _iconImage.enabled = false;
      _countText.text = "";
    }
    else
    {
      _iconImage.enabled = true;
      _iconImage.sprite = Slot.Item.Config.Icon;
      _countText.text = Slot.Item.Count > 1 ? $"{Slot.Item.Count}" : "";
    }
  }

  public void RestoreIconVisibility()
  {
    bool hasItem = Slot != null && !Slot.IsEmpty;
    _iconImage.enabled = hasItem;
    _countText.text = (hasItem && Slot.Item.Count > 1) ? $"{Slot.Item.Count}" : "";
  }

  //===============================

  public Sprite GetIcon()
  {
    return _iconImage.sprite;
  }

  //===============================

  public void OnBeginDrag(PointerEventData eventData)
  {
    if (Slot == null || Slot.IsEmpty)
      return;

    inventoryUI.StartDrag(this, eventData);

    _iconImage.enabled = false;
    _countText.text = "";
  }

  public void OnDrag(PointerEventData eventData)
  {
    inventoryUI.UpdateDrag(eventData);
  }

  public void OnEndDrag(PointerEventData eventData)
  {
    var results = new List<RaycastResult>();
    EventSystem.current.RaycastAll(eventData, results);

    InventorySlotUI toSlotUI = null;
    foreach (var result in results)
    {
      toSlotUI = result.gameObject.GetComponent<InventorySlotUI>();
      if (toSlotUI != null)
        break;
    }

    inventoryUI.EndDrag(toSlotUI, eventData);
  }

  public void OnPointerClick(PointerEventData eventData)
  {
    if (eventData.button == PointerEventData.InputButton.Left)
      inventoryUI.ShowItemInfoPopup(Slot.Item);
  }

  //===============================
}