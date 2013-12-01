﻿using UnityEngine;
using System.Collections;

public class BringASwordToAGF : APowerUp {

    public override void OnPickUp(GameObject powerGo, int clientGuid)
    {
        this.AssignToSlot(powerGo);
        base.OnPickUp(powerGo, clientGuid);
    }

    public override void OnUse()
    {
    }

}