using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class InventorySlot : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    ItemData itemToDisplay;

    public Image itemDisplayImage; 
    public enum InventoryType
    {
        Item,
        Tool
    }

    //determines which inventory section this slot is apart of
    public InventoryType inventoryType;

    int slotIndex;

    public void Display(ItemData itemToDisplay)
    {
        //Check if there is an item to display
        if(itemToDisplay != null)
        {
            //Switch the thumbnail over
            itemDisplayImage.sprite = itemToDisplay.thumbnail;
            this.itemToDisplay = itemToDisplay;

            itemDisplayImage.gameObject.SetActive(true);

            return; 
        }

        itemDisplayImage.gameObject.SetActive(false);

        
    }

    public virtual void OnPointerClick(PointerEventData eventData)
    {
        //move item from inventory to hand
        InventoryManager.Instance.InventoryToHand(slotIndex, inventoryType);
    }

    //set the slot index
    public void AssignIndex(int slotIndex)
    {
        this.slotIndex = slotIndex;
    }

    //Display the item info on the item info box when the player mouses over
    public void OnPointerEnter(PointerEventData eventData)
    {
        UIManager.Instance.DisplayItemInfo(itemToDisplay);
    }

    //Reset the item info box when the player leaves
    public void OnPointerExit(PointerEventData eventData)
    {
        UIManager.Instance.DisplayItemInfo(null);
    }
}