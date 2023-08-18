using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ReVision.Data;
using ReVision.Enum;
using ReVision.Game;

namespace ReVision.Data
{
    /// <summary>
    /// Player data class.
    /// Used to configure the ingame skins.
    /// </summary>
    public class Player
    {
        /// <summary>
        /// The relative path of the head (like a hat) skin file.
        /// </summary>
        /// <example>/Skin/Head/Head_Skin.png</example>
        public string HeadPath { get; set; }
        /// <summary>
        /// The relative path of the body skin file.
        /// </summary>
        /// <example>/Skin/Body/Body_Skin.png</example>
        public string BodyPath { get; set; }
        /// <summary>
        /// The relative path of the flag (or another item, like a sword) skin file.
        /// </summary>
        /// <example>/Skin/Flag/Flag_Skin.png</example>
        public string FlagPath { get; set; }
        /// <summary>
        /// The title (a floating text above) of the player
        /// </summary>        
        public string Title { get; set; }

    }
}