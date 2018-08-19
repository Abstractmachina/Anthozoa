using Rhino.Geometry;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Anthozoa
{


    public class RD2D
    {
        //world holds no geometry. pure relative location + chemical ratio
        private CellGrid2d grid;
        private CellGrid2d nextGrid;

        //diffusion parameters
        private double dA = 1f;
        private double dB = 0.3f;
        private double feed = 0.055f;
        private double kill = 0.062f;

        public CellGrid2d Grid { get => grid; }
        public double DA { get => dA; set => dA = value; }
        public double DB { get => dB; set => dB = value; }
        public double Feed { get => feed; set => feed = value; }
        public double Kill { get => kill; set => kill = value; }


        internal RD2D(int xRes_, int yRes_, double dA_, double dB_, double feed_, double kill_)
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
            grid = new CellGrid2d(xRes_, yRes_);
            PopulateGrid();
            nextGrid = grid;
        }


        private void PopulateGrid()
        {
            for (int i = 0; i < grid.XRes; i++)
            {
                for (int j = 0; j < grid.YRes; j++)
                {
                    double ca = 1d;
                    double cb = 0d;
                    Cell c = new Cell(ca, cb);
                    grid.Cells[i, j] = c;
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
            CellGrid2d tempGrid = grid;
            grid = nextGrid;
            nextGrid = tempGrid;
        }

        private void ReactionDiffuse()
        {
            double deltaTime = 1f;
            for (int x = 1; x < nextGrid.Cells.GetLength(0) - 1; x++)
            {
                for (int y = 1; y < nextGrid.Cells.GetLength(1) - 1; y++)
                {
                    double a = grid.Cells[x, y].A;
                    double b = grid.Cells[x, y].B;

                    double rA = a + dA * LaPlaceA(x, y) - a * b * b + feed * (1 - a) * deltaTime;
                    double rB = b + dB * LaPlaceB(x, y) + a * b * b - (kill + feed) * b * deltaTime;

                    rA = Utility.Constrain(rA, 0, 1d);
                    rB = Utility.Constrain(rB, 0, 1d);

                    nextGrid.Cells[x, y].A = rA;
                    nextGrid.Cells[x, y].B = rB;
                }
            }
        }

        private double LaPlaceA(int x, int y)
        {
            double sumA = 0;
            sumA += grid.Cells[x, y].A * -1;
            sumA += grid.Cells[x - 1, y].A * 0.2;
            sumA += grid.Cells[x + 1, y].A * 0.2;
            sumA += grid.Cells[x, y + 1].A * 0.2;
            sumA += grid.Cells[x, y - 1].A * 0.2;
            sumA += grid.Cells[x - 1, y - 1].A * 0.05;
            sumA += grid.Cells[x + 1, y - 1].A * 0.05;
            sumA += grid.Cells[x + 1, y + 1].A * 0.05;
            sumA += grid.Cells[x - 1, y + 1].A * 0.05;
            return sumA;
        }
        private double LaPlaceB(int x, int y)
        {
            double sumB = 0;
            sumB += grid.Cells[x, y].B * -1;
            sumB += grid.Cells[x - 1, y].B * 0.2;
            sumB += grid.Cells[x + 1, y].B * 0.2;
            sumB += grid.Cells[x, y + 1].B * 0.2;
            sumB += grid.Cells[x, y - 1].B * 0.2;
            sumB += grid.Cells[x - 1, y - 1].B * 0.05;
            sumB += grid.Cells[x + 1, y - 1].B * 0.05;
            sumB += grid.Cells[x + 1, y + 1].B * 0.05;
            sumB += grid.Cells[x - 1, y + 1].B * 0.05;
            return sumB;
        }

        public void Seed(int xMin, int xMax, int yMin, int yMax)
        {
            for (int i = xMin; i < xMax; i++)
            {
                for (int j = yMin; j < yMax; j++)
                {
                    grid.Cells[i, j].A = 0;
                    grid.Cells[i, j].B = 1;
                }
            }
        }
    }


    public static class Utility
    {
        public static double Constrain(double val, double min, double max)
        {
            if (val > max) val = max;
            else if (val < min) val = min;
            return val;
        }
    }

    public struct CellGrid2d
    {
        private int xRes, yRes;
        private Cell[,] cells;

        public int YRes { get => yRes;  }
        public int XRes { get => xRes;  }
        public Cell[,] Cells { get => cells; set => cells = value; }

        public CellGrid2d(int xRes_, int yRes_)
        {
            xRes = xRes_;
            yRes = yRes_;
            cells = new Cell[xRes,yRes];
        }
    }

    //Cell holding chemical ratio
    public struct Cell
    {
        private double a, b;

        public double A {   get => a;set => a = value; }
        public double B { get => b; set => b = value; }

        public Cell(double a_, double b_)
        {
            a = a_;
            b = b_;
        }
    }
}
