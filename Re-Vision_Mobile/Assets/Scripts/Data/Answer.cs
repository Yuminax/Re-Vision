using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using ReVision.Data;
using ReVision.Enum;
using ReVision.Game;

namespace ReVision.Data
{
    /// <summary>
    /// Answer of a question
    /// </summary>
    public class Answer
    {
        /// <summary>
        /// The sentence of the answer
        /// </summary>
        public string Sentence { get; set; }
        /// <summary>
        /// Is this answer correct ?
        /// </summary>
        public bool IsCorrect { get; set; }

    }
}