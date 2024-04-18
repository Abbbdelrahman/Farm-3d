using Palmmedia.ReportGenerator.Core.Parser.Analysis;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIManager : MonoBehaviour, ITimeTracker
{
    public static UIManager Instance { get; private set; }

    [Header("Status Bar")]
    //tool equip slot on the status bar
    public Image toolEquipSlot;
    //time UI
    public TextMeshProUGUI timeText;
    public TextMeshProUGUI dateText;

    [Header("Inventory System")]
    //The inventory panel
    public GameObject inventoryPanel;

    //the tool equip slot UI on the inventory panel
    public HandInventorySlot toolHandSlot;

    //The tool slot UIs
    public InventorySlot[] toolSlots;

    //the tool equip slot UI on the inventory panel
    public HandInventorySlot itemHandSlot;

    //The item slot UIs
    public InventorySlot[] itemSlots;

    //Item info box
    public Text itemNameText;
    public Text itemDescriptionText; 

    private void Awake()
    {
        //If there is more than one instance, destroy the extra
        if (Instance != null && Instance != this)
        {
            Destroy(this);
        }
        else
        {
            //Set the static instance to this instance
            Instance = this;
        }
    }

    private void Start()
    {
        RenderInventory();
        AssignSlotIndexes();

        //add UIManager to the list of objects TimeManager will notify when the tie updates
        TimeManager.Instance.RegisterTracker(this);
    }

    //iterate through the slot UI elements and assign it its reference slot index
    public void AssignSlotIndexes()
    {
        for (int i = 0; i < toolSlots.Length; i++)
        {
            toolSlots[i].AssignIndex(i);
            itemSlots[i].AssignIndex(i);
        }
    }

    //Render the inventory screen to reflect the Player's Inventory. 
    public void RenderInventory()
    {
        //Get the inventory tool slots from Inventory Manager
        ItemData[] inventoryToolSlots = InventoryManager.Instance.tools;

        //Get the inventory item slots from Inventory Manager
        ItemData[] inventoryItemSlots = InventoryManager.Instance.items;

        //Render the Tool section
        RenderInventoryPanel(inventoryToolSlots, toolSlots);

        //Render the Item section
        RenderInventoryPanel(inventoryItemSlots, itemSlots);

        //render the equipped slots
        toolHandSlot.Display(InventoryManager.Instance.equippedTool);
        itemHandSlot.Display(InventoryManager.Instance.equippedItem);

        //get tool equip from InventoryManager
        ItemData equippedTool = InventoryManager.Instance.equippedTool;

        //check if there is an item to display
        if (equippedTool != null)
        {
            //switch the thumbnail over
            toolEquipSlot.sprite = equippedTool.thumbnail;

            toolEquipSlot.gameObject.SetActive(true);
           
            return;
        }

        toolEquipSlot.gameObject.SetActive(false);
    }

    //Iterate through a slot in a section and display them in the UI
    void RenderInventoryPanel(ItemData[] slots, InventorySlot[] uiSlots)
    {
        for (int i = 0; i < uiSlots.Length; i++)
        {
            //Display them accordingly
            uiSlots[i].Display(slots[i]);
        }
    }

    public void ToggleInventoryPanel()
    {
        //If the panel is hidden, show it and vice versa
        inventoryPanel.SetActive(!inventoryPanel.activeSelf);

        RenderInventory();
    }

    //Display Item info on the Item infobox
    public void DisplayItemInfo(ItemData data)
    {
        //If data is null, reset
        if(data == null)
        {
            itemNameText.text = "";
            itemDescriptionText.text = "";

            return;
        }

        itemNameText.text = data.name;
        itemDescriptionText.text = data.description; 
    }

    //callback to handel the UI for time
    public void ClockUpdate(GameTimeStamp timestamp)
    {
        //handle the time
        
        //get the hours and minutes
        int hours = timestamp.hour;
        int minutes = timestamp.minute;

        // AM or PM
        string prefix = "AM ";

        //convert hours to 12 hour clock
        if(hours > 12)
        {
            //time becomes PM
            prefix = "PM ";
            hours -= 12;
        }
        
        //format it for the time text display
        timeText.text = prefix + hours + ":" + minutes.ToString("00");

        //handle the date
        int day = timestamp.day;
        string season = timestamp.season.ToString();
        string dayOfTheWeek = timestamp.GetDayOfTheWeek().ToString();

        //format it for the date text display
        dateText.text = season + " " + day + " (" + dayOfTheWeek + ")";
    }
}