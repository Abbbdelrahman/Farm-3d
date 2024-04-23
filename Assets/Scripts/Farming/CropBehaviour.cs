using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CropBehaviour : MonoBehaviour
{

    //The ID of the land the crop belongs to
    int landID;
    // information in what crop will grow into
    SeedData seedToGrow;

    [Header("Stages of Life")]
    public GameObject seed;
    public GameObject wilted;
    private GameObject seedling;
    private GameObject harvestable;

    // grow points of the crops
    int growth;
    // how many growth points take before it becomes harvestable
    int maxGrowth;

    int maxHealth = GameTimeStamp.HoursToMinutes(48);
    int health;
    public enum CropState
    {
        Seed, Seedling, Harvestable, wilted
    }

    // the current stage of crop state
    public CropState cropState;

    // initialistion for the crop GameObject
    // called when the player planets the seed
    public void Plant(int landID, SeedData seedToGrow)
    {
        LoadCrop(landID, seedToGrow, CropState.Seed, 0, 0);
        // save the seed information
        LandManager.Instance.RegisterCrop(landID, seedToGrow, cropState, growth, health);
    }
    public void LoadCrop(int landID, SeedData seedToGrow, CropState cropState, int growth, int health)
    {
        this.landID = landID;
        //Save the seed information
        this.seedToGrow = seedToGrow;

        //Instantiate the seedling and harvestable GameObjects
        seedling = Instantiate(seedToGrow.seedling, transform);

        //Access the crop item data
        ItemData cropToYield = seedToGrow.cropToYield;

        //Instantiate the harvestable crop
        harvestable = Instantiate(cropToYield.gameModel, transform);

        //Convert Days To Grow into hours
        int hoursToGrow = GameTimeStamp.DaysToHours(seedToGrow.daysToGrow);
        //Convert it to minutes
        maxGrowth = GameTimeStamp.HoursToMinutes(hoursToGrow);


        //Set the growth and health accordingly
        this.growth = growth;
        this.health = health;

        //Check if it is regrowable
        if (seedToGrow.regrowbale)
        {
            //Get the RegrowableHarvestBehaviour from the GameObject
            RegrowableHarvestBehaviour regrowableHarvest = harvestable.GetComponent<RegrowableHarvestBehaviour>();

            //Initialise the harvestable 
            regrowableHarvest.SetParent(this);
        }

        //Set the initial state to Seed
        SwitchState(cropState);

    }


    // the crop will grow if watered
    public void Grow()
    {

        // increse by 1
        growth++;

        if(health < maxHealth)
        {
            health++;
        }

        // grow to be seedling 50% of growth
        if(growth >= maxGrowth / 2 && cropState == CropState.Seed)
        {
            SwitchState(CropState.Seedling);
        }
        // grow from seedling to harvestablw
        if (growth >= maxGrowth && cropState == CropState.Seedling)
        {
            SwitchState(CropState.Harvestable);
        }

        //Inform LandManager on the changes
        LandManager.Instance.OnCropStateChange(landID, cropState, growth, health);
    }

    public void Wither()
    {
        health--;
        if(health <= 0 && cropState != CropState.Seed)
        {
            SwitchState(CropState.wilted);
        }

        //Inform LandManager on the changes
        LandManager.Instance.OnCropStateChange(landID, cropState, growth, health);
    }

    // function to handle the state changes
    void SwitchState(CropState stateToSwitch)
    {

        // reset everty thing to inactive mode
        seed.SetActive(false);
        seedling.SetActive(false);
        harvestable.SetActive(false);
        wilted.SetActive(false);

        switch (stateToSwitch)
        {
            case CropState.Seed:
                // Enable the seed gameobject
                seed.SetActive(true);
                break;
            case CropState.Seedling:
                // Enable the seedling gameobject
                seedling.SetActive(true);
                health = maxHealth;
                break;
            case CropState.Harvestable:
                // Enable the harvestable gameobject
                harvestable.SetActive(true);
                
                // if the seed isn't regrawable
                if(!seedToGrow.regrowbale)
                {
                    // unparnted the crops
                    harvestable.transform.parent = null;
                    RemoveCrop();
                }
                
                break;
            case CropState.wilted:
                // Enable the wilted gameobject
                wilted.SetActive(true);
                break;
        }

        // set the current crop state to the state we're swithcing into
        cropState = stateToSwitch;
    }

    //Destroys and Deregisters the Crop
    public void RemoveCrop()
    {
        LandManager.Instance.DeregisterCrop(landID);
        Destroy(gameObject);
    }

    public void Regrow()
    {
        // Reset the growth
        // get the regrowth time into hours
        int hoursToRegrwo = GameTimeStamp.DaysToHours(seedToGrow.daysToRegrow);
        growth = maxGrowth - GameTimeStamp.HoursToMinutes(hoursToRegrwo);

        SwitchState(CropState.Seedling);
    }

   
}
