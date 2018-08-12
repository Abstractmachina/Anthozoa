using System;
using System.Collections.Generic;

using Grasshopper.Kernel;
using Rhino.Geometry;

using SpatialSlur.SlurField;
using SpatialSlur.SlurCore;


namespace Anthozoa.Components
{
    public class RDFieldComponent : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the RDFieldComponent class.
        /// </summary>
        public RDFieldComponent()
          : base("RD Field", "RDField",
              "Converts cell grid to gridfield",
              "Anthozoa", "Subcategory")
        {
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddGenericParameter("Cell Grid", "G", "Anthozoa cell grid", GH_ParamAccess.item);
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddGenericParameter("GridField", "F", "Slur Gridfield2d", GH_ParamAccess.item);
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            CellGrid2d g = new CellGrid2d();
            if (!DA.GetData(0, ref g)) return;

            


            Vec2d[] vals = new Vec2d[g.XRes * g.YRes];
            int count = 0;
            for (int i = 0; i < g.XRes; i++)
            {
                for (int j = 0; j < g.YRes; j++)
                {
                    vals[count] = new Vec2d(g.Cells[i, j].A, g.Cells[i, j].B);
                    count++;
                }
            }
            //field.Set(vals);
            //DA.SetData(0, field);



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
            get { return new Guid("21564674-6989-4c3c-9862-bb16ba582523"); }
        }
    }
}