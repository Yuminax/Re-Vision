using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ReVision.Data;
using ReVision.Enum;
using ReVision.Game;

namespace ReVision.Data
{
    /// <summary>
    /// The main class for the quizz
    /// </summary>
    public class Quizz
    {
        /// <summary>
        /// All questions of the quizz
        /// </summary>
        public List<Question> Questions { get; set; }
        /// <summary>
        /// The theme of the quizz, like "Math"; "French"; ...
        /// </summary>
        public string Theme { get; set; }
        /// <summary>
        /// The name of the quizz.
        /// Used to allow the user to identify the quiz
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// The Direct download URL to the zip file, which contains all images
        /// </summary>
        public string URL { get; set; }
        /// <summary>
        /// An ID for the quizz, to identify it in the DB
        /// </summary>
        public string ID { get; set; }
        /// <summary>
        /// The possible rewards linked to this quizz
        /// </summary>
        public List<Rewards> Rewards { get; set; }
    }
}