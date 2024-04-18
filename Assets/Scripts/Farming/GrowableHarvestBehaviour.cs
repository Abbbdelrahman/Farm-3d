using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RegrowableHarvestBehaviour : InteactableObject
{
    CropBehaviour parentCrop;

    // set the parent crop
    public void SetParent(CropBehaviour parentCrop)
    {
        this.parentCrop = parentCrop;
    }
    public override void Pickup()
    {
        InventoryManager.Instance.equippedItem = item;
        InventoryManager.Instance.RenderHand();

        // set the parent crop back to seedling to regrow
        parentCrop.Regrow();
    }

}
