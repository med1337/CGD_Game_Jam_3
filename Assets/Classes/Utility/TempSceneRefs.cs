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

    public PlayerScore player_score
    {
        get
        {
            if (player_score_ == null)
                player_score_ = GameObject.FindObjectOfType<PlayerScore>();

            return player_score_;
        }
    }


    private CameraManager camera_manager_;
    private RespawnManager respawn_manager_;
    private PlayerScore player_score_;

}
