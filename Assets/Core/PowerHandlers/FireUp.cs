using UnityEngine;
using System.Collections;

public class FireUp : APowerUp {

    public override void OnPickUp(GameObject powerGo, int ClientGuid)
    {
        this.AssignToSlot(powerGo);
        
    }

    public override void OnUse()
    {
    }

}
