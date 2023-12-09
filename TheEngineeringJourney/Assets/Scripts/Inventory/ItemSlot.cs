using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class ItemSlot : MonoBehaviour, IPointerClickHandler
{
    // Item Slot
    [SerializeField] private TMP_Text _quantityText;
    [SerializeField] private Image _image;
    [SerializeField] private int _maxNumberOfItems;

    // Item Data
    public GameObject SelectedShader;
    public bool IsFull { get; private set; }
    public bool ItemSelected;
    public Sprite EmptySprite;
    
    public string _itemName { get; private set; }
    public int _quantity { get; private set; }
    private string _description;
    private Sprite _sprite;
   
    
    // Item description slot
    public Image ItemDescriptionImage;
    public TMP_Text ItemDescriptionNameText;
    public TMP_Text ItemDescriptionText;

    public int AddItem(string name, int quantity, string description, Sprite sprite)
    {
        if (IsFull) return quantity;
        
        _itemName = name;
        _description = description;
        _sprite = sprite;
        _image.sprite = _sprite;
        
        var itemQuantity =  HandleQuantity(quantity);
        _quantityText.text = _quantity.ToString();
        _quantityText.enabled = true;

        return itemQuantity;
    }
    
    /// <summary>
    /// Pretty bad logic -.-
    /// Create a overfilled or not. return 0 if not overfilled
    /// </summary>
    private int HandleQuantity(int incomingQuantity)
    {
        var itemQuantity = _quantity + incomingQuantity;

        if (itemQuantity >= _maxNumberOfItems)
        {
            _quantity = _maxNumberOfItems;
            IsFull = true;
            return itemQuantity - _maxNumberOfItems;
        }

        _quantity = itemQuantity;

        return 0;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        switch (eventData.button)
        {
            case PointerEventData.InputButton.Left:
                InventoryManager.Instance.DeselectAllSlots();
                OnLeftClick();
                break;
            case PointerEventData.InputButton.Right:
                OnRightClick();
                break;
        }
    }

    private void OnLeftClick()
    {
        SelectedShader.SetActive(true);
        ItemSelected = true;
        ShowItemDescription();
    }

    private void ShowItemDescription()
    {
       ItemDescriptionNameText.text = _itemName;
       ItemDescriptionText.text = _description;

       ItemDescriptionImage.sprite = _sprite
           ? _sprite
           : EmptySprite;
    }

    private void OnRightClick()
    {
        
    }

    
}
