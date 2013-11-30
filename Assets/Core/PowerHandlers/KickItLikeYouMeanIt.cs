using UnityEngine;
using System.Collections;

public class KickItLikeYouMeanIt : APowerUp {

    public override void OnPickUp(GameObject powerGo, int ClientGuid)
    {
        this.AssignToSlot(powerGo);
        
    }

    public override void OnUse()
    {
    }

}
