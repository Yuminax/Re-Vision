using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ReVision.Data;
using ReVision.Enum;
using ReVision.Game;

namespace ReVision.Data
{
    /// <summary>
    /// Statistics of a question for a specific run
    /// </summary>
    public class QuestionStats
    {
        /// <summary>
        /// The minimal time ellapsed to answer the question
        /// </summary>
        public float MinTime { get; set; } = float.MaxValue;
        /// <summary>
        /// Number of wrong attempt to answer the question
        /// </summary>
        public int NbWrongAnswer { get; set; } = 0;
        /// <summary>
        /// Number of time this question was shown to the player
        /// </summary>
        public int NbAttempt { get; set; } = 0;
        /// <summary>
        /// The score earned for a question, during the run
        /// </summary>
        public int EarnedScore { get; set; } = 0;
    }
}