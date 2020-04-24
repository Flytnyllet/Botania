using UnityEngine;

public interface IInteractable //No it's not a typo
{
    //Gör så att saker kan plockas upp från Interactor.cs
    //Behöver en collider för att bli tillgänglig
    bool Interact();
}

//Used for pickup that needs saving 
public abstract class InteractableSaving : MonoBehaviour
{
    protected virtual void PickUp()
    {
        PrefabSaveData data = GetComponentInChildren<PrefabSaveData>();

        if (data != null)
            data.StoreInPrefabSpawnerSaveData();
        else
            Debug.LogError(gameObject.name + " does not have a PrefabSaveData script on it which it must have since it is a pickup! Result of this the pickup just performed will NOT be saved!");
    }

    public virtual void PickedUpAlready()
    {
        Debug.LogError(this.name + " is trying to partially spawn without having overriden the function PickedUpAlready() from InteractableSaving");
    }
}
