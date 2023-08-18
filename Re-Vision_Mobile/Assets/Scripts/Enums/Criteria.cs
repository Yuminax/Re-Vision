using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ReVision.Data;
using ReVision.Enum;
using ReVision.Game;

namespace ReVision.Enum
{
    /// <summary>
    /// The criterias, used for rewards
    /// </summary>
    public enum Criteria
    {
        /// <summary>
        /// Need to perform a "no fail" for the whole quizz
        /// </summary>
        ZeroFail,
        /// <summary>
        /// The quizz need to be played a certain number of time
        /// </summary>
        QuizzPlayed,
        /// <summary>
        /// The sum of all <see cref="Question.Score"/> of a <see cref="Quizz"/> must be reached at the specified value
        /// </summary>
        QuizzMastery,
        /// <summary>
        /// The player need to win the multi-player race
        /// </summary>
        MultiFirst,
    }
}