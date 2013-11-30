using UnityEngine;
using System.Collections;

public class BringASwordToAGF : APowerUp {

    public override void OnPickUp(GameObject powerGo, int ClientGuid)
    {
        this.AssignToSlot(powerGo);
        
    }

    public override void OnUse()
    {
    }

}
