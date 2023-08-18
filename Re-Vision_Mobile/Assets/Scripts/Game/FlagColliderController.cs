using ReVision.Enum;
using ReVision.Game;
using ReVision.MyDebug;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

/// <summary>
/// Script called when an object collid a flag (checkpoint)
/// </summary>
public class FlagColliderController : MonoBehaviour
{

    /// <summary>
    /// Reference on the level component
    /// </summary>
    /// <remarks>This reference is getting through the componenent of the player <see cref="GameObject.Find(_scripts)"/></remarks>
    protected Game game;

    protected void Awake()
    {
        game = GameObject.Find("_scripts").GetComponent<Game>();
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        // If the player is colliding with the flag
        var player = collision.gameObject.GetComponent<PlayerController>();
        if (player != null)
        {
            DebugMode.Log("Player hit flag");
            DebugMode.Log(tag);
            Checkpoint type = Checkpoint.None;

            // Detect the checkpoint type
            if (tag == "StartFlag")
                type = Checkpoint.Start;
            else if (tag == "FlagQuestion")
                type = Checkpoint.Question;
            else if (tag == "FlagFinish")
                type = Checkpoint.End;

            // Retransmit the information to the the game manager
            game.OnHitFlag(type);
        }
    }
}
