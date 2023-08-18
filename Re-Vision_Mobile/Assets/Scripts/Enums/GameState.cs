using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ReVision.Data;
using ReVision.Enum;
using ReVision.Game;

namespace ReVision.Enum
{
    /// <summary>
    /// The state of the game
    /// </summary>
    public enum GameState
    {
        /// <summary>
        /// Initializing
        /// </summary>
        Load,
        /// <summary>
        /// Movement state
        /// </summary>
        Move,
        /// <summary>
        /// Question state
        /// </summary>
        Question,
        /// <summary>
        /// Just after the question was answered
        /// </summary>
        PostQuestion,
        /// <summary>
        /// The end of the game
        /// </summary>
        End,
    }
}