using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct TempSceneRefs
{
    public CameraManager camera_manager
    {
        get
        {
            if (camera_manager_ == null)
                camera_manager_ = GameObject.FindObjectOfType<CameraManager>();

            return camera_manager_;
        }
    }

    public RespawnManager respawn_manager
    {
        get
        {
            if (respawn_manager_ == null)
                respawn_manager_ = GameObject.FindObjectOfType<RespawnManager>();

            return respawn_manager_;
        }
    }


    private CameraManager camera_manager_;
    private RespawnManager respawn_manager_;

}
