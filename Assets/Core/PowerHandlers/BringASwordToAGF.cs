using UnityEngine;
using System.Collections;

public class BringASwordToAGF : APowerUp {

    public override void OnPickUp(GameObject powerGo, int clientGuid)
    {
        Packet p = PacketBuilder.BuildBindOffensiveItem(clientGuid, Config.PowerType.BRING_A_SW_TO_A_GF);
        GameMgr.Instance.s.SendPacketTo(GameMgr.Instance.s.GetTcpClient(clientGuid), p);
        GameObject go = ObjectMgr.Instance.Get(clientGuid);
        BomberController bc = go.GetComponent<BomberController>();
        bc.hasOffensiveItem = (int)Config.PowerType.BRING_A_SW_TO_A_GF;

        base.OnPickUp(powerGo, clientGuid);
    }

    public override void OnUse(GameObject powerGo, int clientGuid)
    {
    }

}
