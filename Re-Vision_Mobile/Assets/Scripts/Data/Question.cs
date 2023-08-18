using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ReVision.Data;
using ReVision.Enum;
using ReVision.Game;

namespace ReVision.Data
{
    /// <summary>
    /// A question of the quizz
    /// </summary>
    public class Question
    {
        /// <summary>
        /// The sentence of the question
        /// </summary>
        public string Sentence { get; set; }
        /// <summary>
        /// All possible answers
        /// </summary>
        public List<Answer> Answers { get; set; }
        /// <summary>
        /// The relative path of the image associated to the question
        /// </summary>
        /// <example>/Question/QuestionX.png</example>
        public string ImagePath { get; set; }
        /// <summary>
        /// The score mastery of the question.
        /// Higer score is, better the player is good with this question.
        /// </summary>
        public int Score { get; set; }

    }
}