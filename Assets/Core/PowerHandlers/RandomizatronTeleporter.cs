using UnityEngine;
using System.Collections;

public class RandomizatronTeleporter : APowerUp {

    public override void OnPickUp(GameObject powerGo, int clientGuid)
    {
        this.AssignToSlot(powerGo);
        base.OnPickUp(powerGo, clientGuid);
    }

    public override void OnUse(GameObject powerGo, int clientGuid)
    {
    }

}
