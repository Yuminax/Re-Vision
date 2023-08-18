using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ReVision.Data;
using ReVision.Enum;
using ReVision.Game;

namespace ReVision.Enum
{
    /// <summary>
    /// The type of the skin part
    /// </summary>
    public enum SkinType
    {
        /// <summary>
        /// It's a skin for the <see cref="Player.FlagPath"/>
        /// </summary>
        Flag,
        /// <summary>
        /// It's a skin for the <see cref="Player.BodyPath"/>
        Body,
        /// <summary>
        /// It's a skin for the <see cref="Player.HeadPath"/>
        /// </summary>
        Head,
        /// <summary>
        /// It's a title for the <see cref="Player.Title"/>
        /// </summary>
        Title,
    }

}