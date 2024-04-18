using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteactableObject : MonoBehaviour
{
    public ItemData item;

    public virtual void Pickup()
    {
        InventoryManager.Instance.equippedItem = item;
        InventoryManager.Instance.RenderHand();
        Destroy(gameObject);
    }
}
