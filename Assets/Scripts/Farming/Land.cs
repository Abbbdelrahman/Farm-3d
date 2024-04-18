using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Land : MonoBehaviour, ITimeTracker
{
    public enum LandStatus
    {
        Soil, Farmland, Watered
    }

    public LandStatus landStatus;

    public Material soilMat, farmlandMat, wateredMat;
    new Renderer renderer;

    //cache the time the land was watered
    GameTimeStamp timeWatered;
    [Header("Crops")]
    // the cropPrefab for instaiate
    public GameObject cropPrefab;

    // the crop currently planted on the land
    CropBehaviour cropPlanted = null;

    //The selection gameobject to enable when the player is selecting the land
    public GameObject select;

    // Start is called before the first frame update
    void Start()
    {
        //Get the renderer component
        renderer = GetComponent<Renderer>();

        //Set the land to soil by default
        SwitchLandStatus(LandStatus.Soil);

        //Deselect the land by default
        Select(false);

        //add this to TimeManager's Listener list
        TimeManager.Instance.RegisterTracker(this);
    }

    public void SwitchLandStatus(LandStatus statusToSwitch)
    {
        //Set land status accordingly
        landStatus = statusToSwitch;

        Material materialToSwitch = soilMat;

        //Decide what material to switch to
        switch (statusToSwitch)
        {
            case LandStatus.Soil:
                //Switch to the soil material
                materialToSwitch = soilMat;
                break;
            case LandStatus.Farmland:
                //Switch to farmland material 
                materialToSwitch = farmlandMat;
                break;

            case LandStatus.Watered:
                //Switch to watered material
                materialToSwitch = wateredMat;

                //cache the time it was watered
                timeWatered = TimeManager.Instance.GetGameTimestamp();
                break;

        }

        //Get the renderer to apply the changes
        renderer.material = materialToSwitch;
    }

    public void Select(bool toggle)
    {
        select.SetActive(toggle);
    }

    //When the player presses the interact button while selecting this land
    public void Interact()
    {

        //check the player's tool slot
        ItemData toolSlot = InventoryManager.Instance.equippedTool;

        // there is nothing equtied returen
        if (toolSlot == null)
        {
            return;
        }

        //try casting the itemdata in the toolslot as equipmentdata
        EquipmentData equipmentTool = toolSlot as EquipmentData;

        //check if it is of type equipmentdata
        if (equipmentTool != null)
        {
            //get the tool type
            EquipmentData.ToolType toolType = equipmentTool.toolType;

            switch (toolType)
            {
                case EquipmentData.ToolType.Hoe:
                    SwitchLandStatus(LandStatus.Farmland);
                    break;

                case EquipmentData.ToolType.WateringCan:
                    SwitchLandStatus(LandStatus.Watered);
                    break;

                // remove crop from land
                case EquipmentData.ToolType.Shovel:
                    if(cropPlanted != null)
                    {
                        Destroy(cropPlanted.gameObject);
                    }
                    break;
            }

            // we don't need to check for seeds if we have already confirmed the tool to be equiment
            return;

        }

        // try casting the itemdata in the toolslot as SeedData
        SeedData seedTool = toolSlot as SeedData;
        /// condtions to player to plant the seeds
        /// 1. he is holding a tool of a type SeedData
        /// 2. the land state must be either watered or farmiend
        /// 3. there is'nt already a crop that has been planted

        if (seedTool != null && landStatus != LandStatus.Soil && cropPlanted == null)
        {
            // the crop object paranted to the land
            GameObject cropObject = Instantiate(cropPrefab, transform);

            // move the crop object to the top land game object
            cropObject.transform.position = new Vector3(transform.position.x, 0.15f, transform.position.z);

            // access the crop behaviour of the crop we're going to plant
            cropPlanted = cropObject.GetComponent<CropBehaviour>();

            //plant it with seed's information
            cropPlanted.Plant(seedTool);
        }
    }

    public void ClockUpdate(GameTimeStamp timestamp)
    {
        //checked if 24 hours has passed since last watered
        if (landStatus == LandStatus.Watered)
        {
            //hours since the land was watered
            int hoursElapsed = GameTimeStamp.CompareTimestamps(timeWatered, timestamp);
            Debug.Log(hoursElapsed + " hours since this was watered");

            // grow the planted crop
            if(cropPlanted != null)
            {
                cropPlanted.Grow();
            }

            if (hoursElapsed > 24)
            {
                //dry up (switch back to farmland)
                SwitchLandStatus(LandStatus.Farmland);
            }
        }

        if(landStatus != LandStatus.Watered && cropPlanted != null)
        {
            if(cropPlanted.cropState != CropBehaviour.CropState.Seed)
            {
                cropPlanted.Wither();
            }
        }
    }
}