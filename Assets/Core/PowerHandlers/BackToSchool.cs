using UnityEngine;
using System.Collections;

public class BackToSchool : APowerUp {

    public override void OnPickUp(GameObject powerGo, int clientGuid)
    {
        Debug.Log("In pick up of BackToschool");
        Packet p = PacketBuilder.BuildBindOffensiveItem(clientGuid, Config.PowerType.BACK_TO_SCHOOL);
    }

    public override void OnUse()
    {
    }

}
