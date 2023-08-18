using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ReVision.MyDebug
{
    /// <summary>
    /// Class used to debug the game
    /// It extends the Debug class, + give some additional debug methods
    /// </summary>
    public class DebugMode : Debug
    {
        /// <summary>
        /// Container of the UI debug elements
        /// </summary>
        public GameObject UI_Holder { get; set; } = null;

        private bool enabled = true;
        /// <summary>
        /// Is the DebugMode enable ?
        /// !! Disable this will also disable all Log of normal <see cref="Debug"/>, like <see cref="Debug.Log(object)"/> !!
        /// </summary>
        public bool Enabled
        {
            get { return enabled; }
            set
            {
                enabled = value;
                Debug.unityLogger.logEnabled = value;
            }
        }

        /// <summary>
        /// Link the UI element which contains all debugs elements
        /// </summary>
        /// <param name="uiHolder">Container of the UI debug elements (set to <see cref="UI_Holder"/></param>
        public DebugMode(GameObject uiHolder)
        {
            UI_Holder = uiHolder;
        }

        /// <summary>
        /// Add UI debug points at the specified coordinate
        /// </summary>
        /// <param name="coord">Coordinate to add the debug point</param>
        /// <remarks>Criteria : <see cref="Enabled"/> <see langword="true"/> and <see cref="UI_Holder"/> not <see langword="null"/></remarks>
        public void UI_TerrainPoint(Vector2 coord)
        {
            if (!Enabled)
                return;

            if (UI_Holder == null)
                return;

            GameObject terrainPoint_UIdebug = MonoBehaviour.Instantiate(Resources.Load("Prefabs/Point", typeof(GameObject))) as GameObject;
            terrainPoint_UIdebug.transform.position = coord;
            terrainPoint_UIdebug.transform.parent = UI_Holder.transform;
        }
    }
}