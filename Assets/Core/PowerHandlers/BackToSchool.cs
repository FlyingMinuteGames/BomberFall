using UnityEngine;
using System.Collections;

public class BackToSchool : APowerUp {

    public override void OnPickUp(GameObject powerGo, int clientGuid)
    {
        Debug.Log("In pick up of BackToschool");
        Packet p = PacketBuilder.BuildBindOffensiveItem(clientGuid, Config.PowerType.BACK_TO_SCHOOL);
        GameMgr.Instance.s.SendPacketTo(GameMgr.Instance.s.GetTcpClient(clientGuid), p);
        base.OnPickUp(powerGo, clientGuid);
    }

    public override void OnUse(GameObject powerGo, int clientGuid)
    {
    }

}
