using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ReVision.Data;
using ReVision.Enum;
using ReVision.Game;
using TMPro;
using System;

namespace ReVision.Game
{
    /// <summary>
    /// Script used to control the player inside the scene "Level"
    /// </summary>
    public class PlayerController : MonoBehaviour
    {
        /// <summary>
        /// Reference of the player's rigidbody.
        /// Initialized in the "Awake" method.
        /// </summary>
        private Rigidbody2D player;
        
        /// <summary>
        /// Reference of the players sprite used for the body
        /// </summary>
        [field: SerializeField]
        private SpriteRenderer bodySkin;

        /// <summary>
        /// Reference of the players sprite used for the head
        /// </summary>
        [field: SerializeField]
        private SpriteRenderer headSkin;
        
        /// <summary>
        /// Reference of the player sprite used for the flag
        /// </summary>
        [field: SerializeField]
        private SpriteRenderer flagSkin;

        /// <summary>
        /// Reference of the player title text used for his title
        /// </summary>
        [field: SerializeField]
        private TextMeshProUGUI title;

        /// <summary>
        /// The current position of the character
        /// </summary>
        public Vector3 Position
        {
            get { return player.position; }
            set { player.position = value; }
        }

        /// <summary>
        /// The vector that represents the velocity of the different axis
        /// </summary>
        public Vector2 Velocity
        {
            get { return player.velocity; }
            set { player.velocity = value; }
        }

        /// <summary>
        /// Is the player is moving
        /// </summary>
        public bool IsMoving { get; set; } = true;

        /// <summary>
        /// The horizontal speed in tile per second
        /// </summary>
        [field: SerializeField]
        public float SpeedX { get; set; } = 2.5f;

        private void Awake()
        {
            player = GetComponent<Rigidbody2D>();
        }

        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }

        private void FixedUpdate()
        {
            // At each frames, perform the move of the player
            Move();
        }

        /// <summary>
        /// Calculate the velocity of the player to determine his deplacement.
        /// If <see cref="IsMoving"/> is false, the velocity will be (0,0)
        /// </summary>
        private void Move()
        {
            var xVelocity = IsMoving ? SpeedX : 0.0f;
            var yVelocity = IsMoving ? Velocity.y : 0.0f;
            Velocity = new Vector2(xVelocity, yVelocity);
        }

        /// <summary>
        /// Apply the skins of the player according the specified data. If the skin can't be found, use default skin stored in resources
        /// </summary>
        /// <param name="data">Data to use</param>
        public void SetSkin(Player data)
        {
            bodySkin.sprite = Utils.LoadFromPeristantOrResource(data?.BodyPath, "Sprites/Body_Cosm1");
            headSkin.sprite = Utils.LoadFromPeristantOrResource(data?.HeadPath, "Sprites/Head_Cosm1");
            flagSkin.sprite = Utils.LoadFromPeristantOrResource(data?.FlagPath, "Sprites/Flag_Cosm1");

            title.text = data?.Title ?? "";
        }
    }
}