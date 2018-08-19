using SpatialSlur.SlurCore;
using SpatialSlur.SlurField;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Anthozoa
{
    public class RD2DSS
    {
        //world holds no geometry. pure relative location + chemical ratio
        //private CellGrid2d grid;
        //private CellGrid2d nextGrid;

        private GridField2d<Vec2d> grid;
        private GridField2d<Vec2d> nextGrid;

        //diffusion parameters
        private double dA = 1f;
        private double dB = 0.3f;
        private double feed = 0.055f;
        private double kill = 0.062f;

        double deltaTime = 1f;

        public GridField2d<Vec2d> Grid { get => grid; }
        public double DA { get => dA; set => dA = value; }
        public double DB { get => dB; set => dB = value; }
        public double Feed { get => feed; set => feed = value; }
        public double Kill { get => kill; set => kill = value; }


        internal RD2DSS(int xRes_, int yRes_, double dA_, double dB_, double feed_, double kill_)
        {
            const string errorMessage = "The resolution of the grid must be at least 1 in each dimension.";

            dA = dA_;
            dB = dB_;
            feed = feed_;
            kill = kill_;


            if (xRes_ < 1 || yRes_ < 1)
            {
                throw new ArgumentException(errorMessage);
            }
            grid = GridField2d.Vec2d.Create(xRes_, yRes_);
            //grid = new CellGrid2d(xRes_, yRes_);
            PopulateGrid();
            nextGrid = grid;
        }


        private void PopulateGrid()
        {
            for (int i = 0; i < grid.CountX; i++)
            {
                for (int j = 0; j < grid.CountY; j++)
                {
                    double ca = 1d;
                    double cb = 0d;
                    Vec2d cell = new Vec2d(ca, cb);
                    grid[i, j] = cell;
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
            GridField2d<Vec2d> tempGrid = GridField2d.Vec2d.CreateCopy(grid);
            grid = GridField2d.Vec2d.CreateCopy(nextGrid);
            nextGrid = GridField2d.Vec2d.CreateCopy(tempGrid);
        }

        //private void ReactionDiffuse()
        //{

        //    for (int x = 0; x < nextGrid.CountX; x++)
        //    {
        //        for (int y = 0; y < nextGrid.CountY; y++)
        //        {
        //            double a = grid[x, y].X;
        //            double b = grid[x, y].Y;

        //            double rA = a + dA * LaPlaceA(x, y) - a * b * b + feed * (1 - a) * deltaTime;
        //            double rB = b + dB * LaPlaceB(x, y) + a * b * b - (kill + feed) * b * deltaTime;

        //            rA = Utility.Constrain(rA, 0, 1d);
        //            rB = Utility.Constrain(rB, 0, 1d);

        //            Vec2d newVal = new Vec2d(rA, rB);
        //            nextGrid[x, y] = newVal;
        //        }
        //    }
        //}
        private void ReactionDiffuse()
        {

            Parallel.For(0, nextGrid.CountX, x =>
            {
                Parallel.For(0, nextGrid.CountY, y =>
                {
                    double a = grid[x, y].X;
                    double b = grid[x, y].Y;

                    double rA = a + dA * LaPlaceA(x, y) - a * b * b + feed * (1 - a) * deltaTime;
                    double rB = b + dB * LaPlaceB(x, y) + a * b * b - (kill + feed) * b * deltaTime;

                    rA = Utility.Constrain(rA, 0, 1d);
                    rB = Utility.Constrain(rB, 0, 1d);

                    Vec2d newVal = new Vec2d(rA, rB);
                    nextGrid[x, y] = newVal;
                });
            });
        }

        private double LaPlaceA(int x, int y)
        {
            double sumA = 0;
            sumA += grid[x, y].X * -1;
            if (x > 0) sumA += grid[x - 1, y].X * 0.2;
            if (x < grid.CountX-1) sumA += grid[x + 1, y].X * 0.2;
            if (y < grid.CountY-1) sumA += grid[x, y + 1].X * 0.2;
            if (y > 0) sumA += grid[x, y - 1].X * 0.2;
            if (x > 0 && y > 0) sumA += grid[x - 1, y - 1].X * 0.05;
            if (x < grid.CountX-1 && y > 0) sumA += grid[x + 1, y - 1].X * 0.05;
            if (x < grid.CountX-1 && y < grid.CountY-1)
                sumA += grid[x + 1, y + 1].X * 0.05;
            if (x > 0 && y < grid.CountY-1) sumA += grid[x - 1, y + 1].X * 0.05;
            return sumA;
        }


        private double LaPlaceB(int x, int y)
        {
            double sumB = 0;
            sumB += grid[x, y].Y * -1;
            if (x > 0) sumB += grid[x - 1, y].Y * 0.2;
            if (x < grid.CountX-1) sumB += grid[x + 1, y].Y * 0.2;
            if (y < grid.CountY-1) sumB += grid[x, y + 1].Y * 0.2;
            if (y > 0) sumB += grid[x, y - 1].Y * 0.2;
            if (x > 0 && y > 0) sumB += grid[x - 1, y - 1].Y * 0.05;
            if (x < grid.CountX-1 && y > 0) sumB += grid[x + 1, y - 1].Y * 0.05;
            if (x < grid.CountX-1 && y < grid.CountY-1)
                sumB += grid[x + 1, y + 1].Y * 0.05;
            if (x > 0 && y < grid.CountY-1) sumB += grid[x - 1, y + 1].Y * 0.05;
            return sumB;
        }

        public void Seed(int xMin, int xMax, int yMin, int yMax)
        {
            for (int i = xMin; i < xMax; i++)
            {
                for (int j = yMin; j < yMax; j++)
                {
                    Vec2d val = new Vec2d(0, 1);
                    grid[i, j] = val;
                }
            }
        }
    }
}
