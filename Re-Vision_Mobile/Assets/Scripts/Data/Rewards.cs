using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ReVision.Data;
using ReVision.Enum;
using ReVision.Game;

namespace ReVision.Data
{
    /// <summary>
    /// The rewards data
    /// </summary>
    public class Rewards
    {
        /// <summary>
        /// The ID of the rewards, to identify it in the DB
        /// </summary>
        public string ID { get; set; }
        /// <summary>
        /// The relative path of the skin file.
        /// </summary>
        /// <example>/Skin/Head/Head_Skin.png</example>
        public string ImagePath { get; set; }
        /// <summary>
        /// The type of the reward (which skin part is)
        /// </summary>
        public SkinType Type { get; set; }
        /// <summary>
        /// The critera to unlock this reward
        /// </summary>
        public Criteria Criteria { get; set; }
        /// <summary>
        /// The number of times or quantity the <see cref="Criteria"/> must be reached
        /// </summary>
        public int CriteriaQuantity { get; set; }
    }
}
