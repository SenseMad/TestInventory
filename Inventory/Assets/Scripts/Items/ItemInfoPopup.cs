using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ItemInfoPopup : MonoBehaviour, IPointerClickHandler
{
  [SerializeField] private Image _iconImage;
  [SerializeField] private TextMeshProUGUI _nameText;
  [SerializeField] private TextMeshProUGUI _descriptionText;
  [SerializeField] private GameObject _panel;

  //===============================

  public void Show(ItemConfig parItemConfig, int parCount)
  {
    if (parItemConfig == null)
    {
      Hide();
      return;
    }

    _panel.SetActive(true);
    _iconImage.sprite = parItemConfig.Icon;
    _nameText.text = parItemConfig.ItemName;

    var sb = new StringBuilder();
    sb.AppendLine($"Вес: {parItemConfig.Weight * parCount:0.00} кг");

    if (parItemConfig is WeaponConfig weapon)
    {
      sb.AppendLine($"Тип: Оружие");
      sb.AppendLine($"Урон: {weapon.Damage}");
      sb.AppendLine($"Патроны: {weapon.AmmoType}");
    }
    else if (parItemConfig is ArmorConfig armor)
    {
      sb.AppendLine($"Тип: Броня");
      sb.AppendLine($"Защита: {armor.Defense}");
    }
    else if (parItemConfig is AmmoConfig ammo)
    {
      sb.AppendLine($"Тип: Патроны");
      sb.AppendLine($"Макс. стак: {ammo.MaxStack}");
    }

    if (parCount > 1 && parItemConfig.MaxStack > 1)
      sb.AppendLine($"Количество: {parCount}");

    _descriptionText.text = $"{sb}";
  }

  public void Hide()
  {
    _panel.SetActive(false);
  }

  public void OnPointerClick(PointerEventData eventData)
  {
    if (eventData.button == PointerEventData.InputButton.Left)
    {
      Hide();
    }
  }

  //===============================
}