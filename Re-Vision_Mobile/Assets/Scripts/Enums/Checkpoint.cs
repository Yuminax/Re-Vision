using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ReVision.Data;
using ReVision.Enum;
using ReVision.Game;

namespace ReVision.Enum
{
    /// <summary>
    /// The type of checkpoints in game
    /// </summary>
    public enum Checkpoint
    {
        /// <summary>
        /// It's the location of the start of the game
        /// </summary>
        Start,
        /// <summary>
        /// It's a location of a question 
        /// </summary>
        Question,
        /// <summary>
        /// It's a location that mark the end
        /// </summary>
        End,
        /// <summary>
        /// This checkpoint isn't setted
        /// </summary>
        None,
    }
}