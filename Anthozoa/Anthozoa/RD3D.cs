using Rhino.Geometry;
using SpatialSlur.SlurCore;
using SpatialSlur.SlurField;
using SpatialSlur.SlurMesh;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Anthozoa
{
    class RD3D
    {
        //world holds no geometry. pure relative location + chemical ratio

        private GridField3d<Vec3d> grid;
        private GridField3d<Vec3d> nextGrid;

        //diffusion parameters
        private double dA = 1f;
        private double dB = 0.3f;
        private double feed = 0.055f;
        private double kill = 0.062f;

        double deltaTime = 1f;

        public GridField3d<Vec3d> Grid { get => grid; }
        public double DA { get => dA; set => dA = value; }
        public double DB { get => dB; set => dB = value; }
        public double Feed { get => feed; set => feed = value; }
        public double Kill { get => kill; set => kill = value; }


        internal RD3D(int xRes_, int yRes_, int zRes_, double dA_, double dB_, double feed_, double kill_)
        {
            const string errorMessage = "The resolution of the grid must be at least 1 in each dimension.";

            dA = dA_;
            dB = dB_;
            feed = feed_;
            kill = kill_;


            if (xRes_ < 1 || yRes_ < 1 || zRes_ < 1)
            {
                throw new ArgumentException(errorMessage);
            }
            grid = GridField3d.Vec3d.Create(xRes_, yRes_, zRes_);
            PopulateGrid();
            nextGrid = grid;
        }


        private void PopulateGrid()
        {
            for (int i = 0; i < grid.CountX; i++)
            {
                for (int j = 0; j < grid.CountY; j++)
                {
                    for (int k = 0; k < grid.CountZ; k++)
                    {
                        double ca = 1d;
                        double cb = 0d;
                        Vec3d cell = new Vec3d(ca, cb, 0);
                        grid[i, j, k] = cell;
                    }
                }
            }
        }

        public void Update()
        {
            ReactionDiffuse();
            SwapGrid();
        }

        private void SwapGrid()
        {
            GridField3d<Vec3d> tempGrid = GridField3d.Vec3d.CreateCopy(grid);
            grid = GridField3d.Vec3d.CreateCopy(nextGrid);
            nextGrid = GridField3d.Vec3d.CreateCopy(tempGrid);
        }

        /// <summary>
        /// 
        /// </summary>
        private void ReactionDiffuse()
        {

            Parallel.For(0, nextGrid.CountX, x =>
            {
                Parallel.For(0, nextGrid.CountY, y =>
                {
                    Parallel.For(0, nextGrid.CountZ, z =>
                    {
                        double a = grid[x, y, z].X;
                        double b = grid[x, y, z].Y;

                        double rA = a + dA * LaPlaceA(x, y, z) - a * b * b + feed * (1 - a) * deltaTime;
                        double rB = b + dB * LaPlaceB(x, y, z) + a * b * b - (kill + feed) * b * deltaTime;

                        rA = Utility.Constrain(rA, 0, 1d);
                        rB = Utility.Constrain(rB, 0, 1d);

                        Vec2d newVal = new Vec2d(rA, rB);
                        nextGrid[x, y, z] = newVal;
                    });

                });
            });
        }

        //private void ReactionDiffuse()
        //{

        //    for (int x = 0; x < nextGrid.CountX; x++)
        //    {
        //        for (int y = 0; y < nextGrid.CountY; y++)
        //        {
        //            for (int z = 0; z < nextGrid.CountZ; z++)
        //            {
        //                double a = grid[x, y, z].X;
        //                double b = grid[x, y, z].Y;

        //                double rA = a + dA * LaPlaceA(x, y, z) - a * b * b + feed * (1 - a) * deltaTime;
        //                double rB = b + dB * LaPlaceB(x, y, z) + a * b * b - (kill + feed) * b * deltaTime;

        //                rA = Utility.Constrain(rA, 0, 1d);
        //                rB = Utility.Constrain(rB, 0, 1d);

        //                Vec2d newVal = new Vec2d(rA, rB);
        //                nextGrid[x, y, z] = newVal;
        //            }

        //        }
        //    }
        //}
        double nb1 = 0.2d;
        double nb2 = 0.02d;

        private double LaPlaceA(int x, int y, int z)
        {
            double sumA = 0;
            //this cell
            sumA += grid[x, y, z].X * -1;
            //immediate neighbors 
            if (x > 0)                  sumA += grid[x - 1, y, z].X * nb1;
            if (x < grid.CountX - 1)    sumA += grid[x + 1, y, z].X * nb1;
            if (y < grid.CountY - 1)    sumA += grid[x, y + 1, z].X * nb1;
            if (y > 0)                  sumA += grid[x, y - 1, z].X * nb1;
            if (z> 0)                   sumA += grid[x, y, z - 1].X * nb1;
            if (z < grid.CountZ-1)      sumA += grid[x, y, z + 1].X * nb1;
            //secondary neighbors
            if (x > 0 && y > 0)                             sumA += grid[x - 1, y - 1, z].X * nb2;
            if (x < grid.CountX - 1 && y > 0)               sumA += grid[x + 1, y - 1, z].X * nb2;
            if (x < grid.CountX - 1 && y < grid.CountY - 1) sumA += grid[x + 1, y + 1, z].X * nb2;
            if (x > 0 && y < grid.CountY - 1)               sumA += grid[x - 1, y + 1, z].X * nb2;
            //secondary neighbors below
            if (z > 0)
            {
                if (x > 0 && y > 0)                             sumA += grid[x - 1, y - 1, z - 1].X * nb2;
                if (x < grid.CountX - 1 && y > 0)               sumA += grid[x + 1, y - 1, z - 1].X * nb2;
                if (x < grid.CountX - 1 && y < grid.CountY - 1) sumA += grid[x + 1, y + 1, z - 1].X * nb2;
                if (x > 0 && y < grid.CountY - 1)               sumA += grid[x - 1, y + 1, z - 1].X * nb2;
            }
            //secondary neighbors above
            if (z < grid.CountZ-1)
            {
                if (x > 0 && y > 0)                             sumA += grid[x - 1, y - 1, z + 1].X * nb2;
                if (x < grid.CountX - 1 && y > 0)               sumA += grid[x + 1, y - 1, z + 1].X * nb2;
                if (x < grid.CountX - 1 && y < grid.CountY - 1) sumA += grid[x + 1, y + 1, z + 1].X * nb2;
                if (x > 0 && y < grid.CountY - 1)               sumA += grid[x - 1, y + 1, z + 1].X * nb2;
            }
            return sumA;
        }


        private double LaPlaceB(int x, int y, int z)
        {
            double sumB = 0;
            //this cell
            sumB += grid[x, y, z].Y * -1;
            //immediate neighbors 
            if (x > 0) sumB += grid[x - 1, y, z].Y * nb1;
            if (x < grid.CountX - 1) sumB += grid[x + 1, y, z].Y * nb1;
            if (y < grid.CountY - 1) sumB += grid[x, y + 1, z].Y * nb1;
            if (y > 0) sumB += grid[x, y - 1, z].Y * nb1;
            if (z > 0) sumB += grid[x, y, z - 1].Y * nb1;
            if (z < grid.CountZ - 1) sumB += grid[x, y, z + 1].Y * nb1;
            //secondary neighbors
            if (x > 0 && y > 0) sumB += grid[x - 1, y - 1, z].Y * nb2;
            if (x < grid.CountX - 1 && y > 0) sumB += grid[x + 1, y - 1, z].Y * nb2;
            if (x < grid.CountX - 1 && y < grid.CountY - 1) sumB += grid[x + 1, y + 1, z].Y * nb2;
            if (x > 0 && y < grid.CountY - 1) sumB += grid[x - 1, y + 1, z].Y * nb2;
            //secondary neighbors below
            if (z > 0)
            {
                if (x > 0 && y > 0) sumB += grid[x - 1, y - 1, z - 1].Y * nb2;
                if (x < grid.CountX - 1 && y > 0) sumB += grid[x + 1, y - 1, z - 1].Y * nb2;
                if (x < grid.CountX - 1 && y < grid.CountY - 1) sumB += grid[x + 1, y + 1, z - 1].Y * nb2;
                if (x > 0 && y < grid.CountY - 1) sumB += grid[x - 1, y + 1, z - 1].Y * nb2;
            }
            //secondary neighbors above
            if (z < grid.CountZ - 1)
            {
                if (x > 0 && y > 0) sumB += grid[x - 1, y - 1, z + 1].Y * nb2;
                if (x < grid.CountX - 1 && y > 0) sumB += grid[x + 1, y - 1, z + 1].Y * nb2;
                if (x < grid.CountX - 1 && y < grid.CountY - 1) sumB += grid[x + 1, y + 1, z + 1].Y * nb2;
                if (x > 0 && y < grid.CountY - 1) sumB += grid[x - 1, y + 1, z + 1].Y * nb2;
            }
            return sumB;
        }

        public void Seed(int xMin, int xMax, int yMin, int yMax, int zMin, int zMax)
        {
            for (int i = xMin; i < xMax; i++)
            {
                for (int j = yMin; j < yMax; j++)
                {
                    for (int k = zMin; k < zMax; k++)
                    {
                        Vec3d val = new Vec3d(0, 1, 0);
                        grid[i, j, k] = val;
                    }
                }
            }
        }


        public void test(GridField3d<Vec3d> f)
        {

            GridField3d<double> field = GridField3d.Double.Create(f.CountX, f.CountY, f.CountZ);

            IEnumerable<double> vals = f.Values.Select(i => {return f.ValueAt(i).X;});
            field.Set(vals);

            //GridField3d.Double.Create(f);

            Mesh m = new Mesh();

            m = Isosurface.MarchingCubes.Evaluate(field, 0.2d);

        }

    }
}
