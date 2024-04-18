using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CropBehaviour : MonoBehaviour
{
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
    public void Plant(SeedData seedToGrow)
    {
        // save the seed information
        this.seedToGrow = seedToGrow;

        // set the seedling and harvestable gameobject
        seedling = Instantiate(seedToGrow.seedling, transform);

        // access the crop item data
        ItemData cropToYield = seedToGrow.cropToYield;

        // Instantiate the harvestable crop
        harvestable = Instantiate(cropToYield.gameModel, transform);

        // convert the days growth into hours
        int hoursToGrow = GameTimeStamp.DaysToHours(seedToGrow.daysToGrow);
        //convert it to minutes
        maxGrowth = GameTimeStamp.HoursToMinutes(hoursToGrow);

        // check if is regrowable
        if(seedToGrow.regrowbale)
        {
            RegrowableHarvestBehaviour regrowableHarvest = harvestable.GetComponent<RegrowableHarvestBehaviour>();
            
            regrowableHarvest.SetParent(this);
        }

// set the Intial state of seed
SwitchState(CropState.Seed);
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
    }

    public void Wither()
    {
        health--;
        if(health <= 0 && cropState != CropState.Seed)
        {
            SwitchState(CropState.wilted);
        }
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
                    Destroy(gameObject);
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

    public void Regrow()
    {
        // Reset the growth
        // get the regrowth time into hours
        int hoursToRegrwo = GameTimeStamp.DaysToHours(seedToGrow.daysToRegrow);
        growth = maxGrowth - GameTimeStamp.HoursToMinutes(hoursToRegrwo);

        SwitchState(CropState.Seedling);
    }
}
