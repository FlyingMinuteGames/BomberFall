using UnityEngine;
using System.Collections;

public abstract class APowerUp {

    //Generic Method called on initialization
    public void Init(GameObject powerGO){
        
    }

    //Destructor
    public void Delete(GameObject powerGO){

    }

    public abstract void OnPickUp(GameObject powerGO, int ClientGuid);
    public abstract void OnUse();
    public abstract void OnTimedOut();


}
