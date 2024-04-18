using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Items/Seed")]
public class SeedData : ItemData
{
    //Time it takes before the seed matures into a crop
    public int daysToGrow;

    //The crop the seed will yield
    public ItemData cropToYield;

    // The seedling GameObject
    public GameObject seedling;

    [Header("Regrowbale")]
    // is the planet able to regrow the crop after being harvested
    public bool regrowbale;
    // time take before the plant yields another crop
    public int daysToRegrow;
}
