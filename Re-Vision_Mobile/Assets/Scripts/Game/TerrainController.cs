using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D;

namespace ReVision.Game
{
    /// <summary>
    /// Class used to control the terrain structure
    /// </summary>
    public class TerrainController : MonoBehaviour
    {
        /// <summary>
        /// reference on the sprite shape used to generate the terrain
        /// </summary>
        private SpriteShapeController sc;

        private void Awake()
        {
            sc = GetComponent<SpriteShapeController>();
        }

        // Start is called before the first frame update
        void Start()
        {

        }

        /// <summary>
        /// Generate a terrain with a given number of point
        /// </summary>
        /// <param name="points">the points of the shape</param>
        /// <param name="depth">The initial and final y coordinate of the shape</param>
        /// <param name="startX">The start X point</param>
        public void InitLine(List<Vector2> points, int depth, int startX)
        {
            sc.spline.Clear();
            // Start below the terrain
            sc.spline.InsertPointAt(0, new Vector2(startX, -depth));
            // outside the left part of the camera
            sc.spline.InsertPointAt(1, new Vector2(startX, 0));

            foreach (Vector2 point in points)
            {
                sc.spline.InsertPointAt(sc.spline.GetPointCount(), point);
                sc.spline.SetTangentMode(sc.spline.GetPointCount() - 1, ShapeTangentMode.Continuous);
            }

            // End below the terrain
            sc.spline.InsertPointAt(sc.spline.GetPointCount(), new Vector2(points[^1].x, -depth));
        }

        /// <summary>
        /// Improve the terrain length by adding a new point at the end
        /// </summary>
        /// <param name="point">Coordinate of the new point</param>
        /// <remarks>When a new point is add, the  last "depth" point (y coordinate) is also updated</remarks>
        public void AddPoint(Vector2 point)
        {
            //Add a point before the last one (which is used for the depth)
            sc.spline.InsertPointAt(sc.spline.GetPointCount() - 1, point);
            sc.spline.SetTangentMode(sc.spline.GetPointCount() - 2, ShapeTangentMode.Continuous);

            //Update the point x location for the depth
            int lastPointIndex = sc.spline.GetPointCount() - 1;
            sc.spline.SetPosition(lastPointIndex,
                new Vector2(point.x, sc.spline.GetPosition(lastPointIndex).y)
                );
        }

        // Update is called once per frame
        void Update()
        {

        }
    }
}
