using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryManager : MonoBehaviour
{
    public static InventoryManager Instance { get; private set; }

    private void Awake()
    {
        //If there is more than one instance, destroy the extra
        if(Instance != null && Instance != this)
        {
            Destroy(this);
        }
        else
        {
            //Set the static instance to this instance
            Instance = this; 
        }
    }

    [Header("Tools")]
    //Tool Slots
    public ItemData[] tools = new ItemData[8];
    //Tool in the player's hand
    public ItemData equippedTool = null; 

    [Header("Items")]
    //Item Slots
    public ItemData[] items = new ItemData[8];
    //Item in the player's hand
    public ItemData equippedItem = null;
    // make the player to hand item
    public Transform handPoint;

    //Equipping

    //Handles movement of item from Inventory to Hand
    public void InventoryToHand(int slotIndex, InventorySlot.InventoryType inventoryType)
    {
        if(inventoryType == InventorySlot.InventoryType.Item)
        {
            //cache the inventory slot ItemData from InventoryManager
            ItemData itemToEquip = items[slotIndex];

            //change the inventory slot to the hand's
            items[slotIndex] = equippedItem;

            //change the hand's slot to the inventory slot
            equippedItem = itemToEquip;

            // update the changes on the scene
            RenderHand();
        }
        else
        {
            //cache the inventory slot ItemData from InventoryManager
            ItemData toolToEquip = tools[slotIndex];

            //change the inventory slot to the hand's
            tools[slotIndex] = equippedTool;

            //change the hand's slot to the inventory slot
            equippedTool = toolToEquip;
        }

        //update the changes to the UI
        UIManager.Instance.RenderInventory();
    }

    //Handles movement of itme from Hand to Inventory
    public void HandToInventory(InventorySlot.InventoryType inventoryType)
    {
        if(inventoryType == InventorySlot.InventoryType.Item)
        {
            //iterate through each inventory slot and find an empty slot
            for(int i = 0; i < items.Length; i++)
            {
                if (items[i] == null)
                {
                    //send the equipped item over
                    items[i] = equippedItem;
                    //remove the item from the hand
                    equippedItem = null;
                    break;
                }
            }

            // update chanegs on the scene
            RenderHand();
        }
        else
        {
            //iterate through each inventory slot and find an empty slot
            for (int i = 0; i < tools.Length; i++)
            {
                if (tools[i] == null)
                {
                    //send the equipped item over
                    tools[i] = equippedTool;
                    //remove the item from the hand
                    equippedTool = null;
                    break;
                }
            }


        }
        // update changes in the inventory
        UIManager.Instance.RenderInventory();
    }

    // render the player's equipped item in the scene
    public void RenderHand()
    {
        // reset object on hand
        if(handPoint.childCount > 0)
        {
            Destroy(handPoint.GetChild(0).gameObject);
        }

        if(equippedItem != null)
        {
            // instatiate gamemodel on the player's hand and put it on the scene
            Instantiate(equippedItem.gameModel, handPoint);
        }
        
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}