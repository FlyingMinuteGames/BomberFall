using UnityEngine;
using System.Collections;

public abstract class APowerUp {

    //Generic Method called on initialization
    public void Init(GameObject powerGO){
        
    }

    //Remove item from scene and put it back to pool
    public void Delete(GameObject powerGO){
        Debug.Log("Trying to call despawn on powerUp(" + powerGO.name + ")");
        GameMgr.Instance.Despawn(GOType.GO_PWRUP, powerGO.GetComponent<Guid>().GetGUID()); 
    }

    public void AssignToSlot(GameObject powerGO)
    {

    }

    public virtual void OnPickUp(GameObject powerGO, int clientGuid)
    {
        Delete(powerGO); 
    }

    public abstract void OnUse(GameObject powerGO, int clientGuid);


}
