using System;
using System.Linq;
using UnityEngine;

public class InventoryManager : SingletonMonobehaviour<InventoryManager>
{
    public GameObject InventoryMenu;
    private bool _menuActivated = false;
    public ItemSlot[] ItemSlot;

    private void Start()
    {
        InventoryMenu.SetActive(false);
    }

    public int AddItem(string name, int quantity, string description, Sprite sprite)
    {
        foreach (var itemSlot in ItemSlot)
        {
            if (itemSlot.IsFull is false && itemSlot.name == name || itemSlot._quantity is 0)
            {
                var leftOverItems = itemSlot.AddItem(name, quantity, description, sprite);
                
                if (leftOverItems > 0)
                    leftOverItems = AddItem(name, leftOverItems, description, sprite);
                    
                    return leftOverItems;
            }
        }

        return quantity;
        
        ItemSlot
            .FirstOrDefault(x => x.IsFull is false)?
            .AddItem(name, quantity, description, sprite);
    }

    private void OnEnable()
    {
        GameManager.Instance.InputEvents.OnInventoryTogglePressed += InventoryTogglePressed;
    }
    
    private void OnDisable()
    {
        GameManager.Instance.InputEvents.OnInventoryTogglePressed -= InventoryTogglePressed;
    }

    private void InventoryTogglePressed()
    {
        _menuActivated = !_menuActivated;
        InventoryMenu.SetActive(_menuActivated);
    }
    
    public void DeselectAllSlots()
    {
        //ItemSlot.ToList().ForEach(x => x.SelectedShader.SetActive(false));
        foreach (var itemSlot in ItemSlot)
        {
            itemSlot.SelectedShader.SetActive(false);
            itemSlot.ItemSelected = false;
        }
    }
}
