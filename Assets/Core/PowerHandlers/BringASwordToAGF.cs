using UnityEngine;
using System.Collections;

public class BringASwordToAGF : APowerUp {

    public override void OnPickUp(GameObject powerGo, int clientGuid)
    {
        Packet p = PacketBuilder.BuildBindOffensiveItem(clientGuid, Config.PowerType.BRING_A_SW_TO_A_GF);
        GameMgr.Instance.s.SendPacketTo(GameMgr.Instance.s.GetTcpClient(clientGuid), p);
        base.OnPickUp(powerGo, clientGuid);
    }

    public override void OnUse()
    {
    }

}
