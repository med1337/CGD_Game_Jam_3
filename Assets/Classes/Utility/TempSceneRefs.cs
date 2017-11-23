using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct TempSceneRefs
{
    public RespawnManager respawn_manager
    {
        get
        {
            if (respawn_manager_ == null)
                respawn_manager_ = GameObject.FindObjectOfType<RespawnManager>();

            return respawn_manager_;
        }
    }


    private RespawnManager respawn_manager_;


}
