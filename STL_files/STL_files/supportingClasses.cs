using System;
using System.Data;
using System.Drawing;
using System.Collections.Generic;

using IxMilia.Stl;

namespace STL_files {

    class Polygon {
        //============================== variables ===================================================

        /// <summary>
        /// The height of the floor, a value of NaN (not a number) will mean the floor is removed.
        /// </summary>
        public float FloorZ { get; private set; }
        
        /// <summary>
        /// The height of the roof, a value of NaN (not a number) will mean the roof is removed.
        /// </summary>
        public float RoofZ { get; private set;}
        
        public List<PointF> vertices;
        //============================== /variables ==================================================

        //============================== constructors ================================================
        /// <param name="floorZ">The height of the floor, a value of NaN (not a number) will mean the floor is removed.</param>
        /// <param name="roofZ">The height of the roof, a value of NaN (not a number) will mean the roof is removed.</param>
        public Polygon(float floorZ, float roofZ, List<PointF> verts) {
            FloorZ = floorZ;
            RoofZ = roofZ;
            vertices = verts;
        }

        /// <param name="floorZ">The height of the floor, a value of NaN (not a number) will mean the floor is removed.</param>
        /// <param name="roofZ">The height of the roof, a value of NaN (not a number) will mean the roof is removed.</param>
        public Polygon(float floorZ, float roofZ) {
            FloorZ = floorZ;
            RoofZ = roofZ;
            vertices = new List<PointF>();
        }
        //============================== /constructors ===============================================

        //============================== functions ===================================================
        /// <param name="floorZ">The height of the floor, a value of NaN (not a number) will mean the floor is removed.</param>
        public void setFloor(float floorz) {
            FloorZ = floorz;
        }

        /// <param name="roofZ">The height of the roof, a value of NaN (not a number) will mean the roof is removed.</param>
        public void setRoof() {
            
        }
        //============================== /functions ==================================================
    }


    static class Convert {
        public static StlFile GetFile(List<Polygon> polygons) {


            StlFile file = new StlFile();

            return file;
        }

        public static List<StlTriangle> Triangulate(this Polygon polygon) {
            List<StlTriangle> tris = new List<StlTriangle>();

            return tris;
        }
    }
}