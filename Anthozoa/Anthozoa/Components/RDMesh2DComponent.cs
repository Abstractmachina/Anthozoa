using System;
using System.Collections.Generic;
using System.Drawing;
using System.Threading.Tasks;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Types;
using Rhino.Geometry;
using SpatialSlur.SlurRhino;

namespace Anthozoa
{
    public class RDMesh2DComponent : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the RDMesh2DComponent class.
        /// </summary>
        public RDMesh2DComponent()
          : base("Mesh Visualizer", "MVis",
              "Returns a mesh representation of cells",
              "Anthozoa", "Display")
        {
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddMeshParameter("Base Mesh", "M", "Base mesh to color", GH_ParamAccess.item);
            pManager.AddGenericParameter("CellGrid2d", "G", "Anthozoa CellGrid", GH_ParamAccess.item);
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddTextParameter("debug", "D", ".", GH_ParamAccess.list);
            pManager.AddMeshParameter("Mesh Output", "M", "Mesh Visualization", GH_ParamAccess.item);
        }

        List<string> debugLog = new List<string>();
        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {

            debugLog.Clear();
            CellGrid2d grid = new CellGrid2d();
            Mesh m = null;

            if (!DA.GetData(0, ref m)) return;
            if (!DA.GetData(1, ref grid)) return;


            if (m.Faces.Count != grid.Cells.Length) AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "Mesh must have same amount of faces as cellgrid.");

            debugLog.Add(grid.XRes.ToString());
            debugLog.Add(grid.YRes.ToString());
            debugLog.Add(grid.Cells.Length.ToString());

            foreach(Point3d v in m.Vertices) m.VertexColors.Add(0, 0, 0);

            double[] AFlat = new double[grid.Cells.Length];
            int count = 0;
            for (int i = 0; i < grid.XRes; i++)
            {
                for (int j=0; j < grid.YRes; j++)
                {
                    AFlat[count] = grid.Cells[i, j].A;
                    count++;
                }
            }

            for (int i = 0; i < m.Faces.Count; i++)
            {

                int[] indices = m.Faces.GetTopologicalVertices(i);
                int val = (int) SpatialSlur.SlurCore.SlurMath.Remap(AFlat[i], 0, 1, 0, 255);

                foreach (int ind in indices) m.VertexColors[ind] = Color.FromArgb(val,val,val);
            }


            DA.SetDataList("debug", debugLog);
            DA.SetData("Mesh Output", m);
        }

        /// <summary>
        /// Provides an Icon for the component.
        /// </summary>
        protected override System.Drawing.Bitmap Icon
        {
            get
            {
                //You can add image files to your project resources and access them like this:
                // return Resources.IconForThisComponent;
                return null;
            }
        }

        /// <summary>
        /// Gets the unique ID for this component. Do not change this ID after release.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("991574e7-4539-495a-b283-ece148cc0bd3"); }
        }
    }
}