using UnityEngine;
using System.Collections;

public abstract class APowerUp {

    //Generic Method called on initialization
    public void Init(GameObject powerGO){
        
    }

    //Remove item from scene and put it back to pool
    public void Delete(GameObject powerGO){

    }

    public void AssignToSlot(GameObject powerGO)
    {

    }

    public abstract void OnPickUp(GameObject powerGO, int ClientGuid);

    public abstract void OnUse();


}
