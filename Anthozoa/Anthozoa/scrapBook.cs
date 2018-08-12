using Rhino.Geometry;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Anthozoa
{
    /*
    class ScrapBook
    {
        private void InitGeometry()
        {
            for (int z = 0; z < zRes; z++)
            {
                for (int y = 0; y < yRes; y++)
                {
                    for (int x = 0; x < xRes; x++)
                    {
                        Mesh m = new Mesh();
                        Point3d[] pts = new Point3d[8];
                        pts[0] = new Point3d(x, y, z);
                        pts[1] = new Point3d(x + 1, y, z);
                        pts[2] = new Point3d(x + 1, y + 1, z);
                        pts[3] = new Point3d(x, y + 1, z);
                        pts[4] = new Point3d(x, y, z + 1);
                        pts[5] = new Point3d(x + 1, y, z + 1);
                        pts[6] = new Point3d(x + 1, y + 1, z + 1);
                        pts[7] = new Point3d(x, y + 1, z + 1);

                        m.Vertices.AddVertices(pts);

                        m.Faces.AddFace(0, 1, 2, 3);
                        m.Faces.AddFace(0, 1, 4, 5);
                        m.Faces.AddFace(1, 2, 5, 6);
                        m.Faces.AddFace(2, 3, 6, 7);
                        m.Faces.AddFace(3, 0, 7, 4);
                        m.Faces.AddFace(4, 5, 6, 7);

                        geom[x, y, z] = m;
                    }
                }
            }
        }



        //create a mesh-representation in 3d space for each pixel
        public List<Mesh> MeshRender()
        {

            List<Mesh> recs = new List<Mesh>();

            for (int i = 0; i < grid.GetLength(0); i++)
            {
                for (int j = 0; j < grid.GetLength(1); j++)
                {
                    double a = grid[i, j].A;
                    double b = grid[i, j].B;
                    int balance = (int)Utility.Constrain((a - b) * 255, 0d, 255d);
                    Color c = Color.FromArgb(255, balance, balance, balance);

                    Point3d p1 = new Point3d(i, j, 0);
                    Point3d p2 = new Point3d(p1.X + 1, p1.Y, 0);
                    Point3d p3 = new Point3d(p1.X + 1, p1.Y + 1, 0);
                    Point3d p4 = new Point3d(p1.X, p1.Y + 1, 0);

                    Mesh tempMesh = new Mesh();
                    tempMesh.Vertices.Add(p1);
                    tempMesh.VertexColors.Add(c);
                    tempMesh.Vertices.Add(p2);
                    tempMesh.VertexColors.Add(c);
                    tempMesh.Vertices.Add(p3);
                    tempMesh.VertexColors.Add(c);
                    tempMesh.Vertices.Add(p4);
                    tempMesh.VertexColors.Add(c);
                    tempMesh.Faces.AddFace(0, 1, 2, 3);
                    recs.Add(tempMesh);
                }
            }
            return recs;
        }

        public void Seed(int xMin, int xMax, int yMin, int yMax)
        {
            for (int i = xMin; i < xMax; i++)
            {
                for (int j = yMin; j < yMax; j++)
                {
                    grid[i, j].A = 0;
                    grid[i, j].B = 1;
                }
            }
        }
    }
    */
}
