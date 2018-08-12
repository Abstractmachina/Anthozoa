using System;
using System.Collections.Generic;

using Grasshopper.Kernel;
using Grasshopper.Kernel.Types;
using Rhino.Geometry;

// In order to load the result of this wizard, you will also need to
// add the output bin/ folder of this project to the list of loaded
// folder in Grasshopper.
// You can use the _GrasshopperDeveloperSettings Rhino command for that.

namespace Anthozoa
{
    public class RDSolverComponent : GH_Component
    {


        /// <summary>
        /// Each implementation of GH_Component must provide a public 
        /// constructor without any arguments.
        /// Category represents the Tab in which the component will appear, 
        /// Subcategory the panel. If you use non-existing tab or panel names, 
        /// new tabs/panels will automatically be created.
        /// </summary>
        public RDSolverComponent()
          : base("ReactionDiffusion 2D", "RD2D",
              "Reaction-diffusion algorithm using the Gray-Scott Model",
              "Anthozoa", "Main")
        {
        }

        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddBooleanParameter("Reset Simulation", "RES", "Clear and Reload all values.", GH_ParamAccess.item, true);
            pManager.AddBooleanParameter("Run Simulation", "RUN", "Run Simulation", GH_ParamAccess.item, false);
            pManager.AddNumberParameter("Simulation Speed", "SPD", "Frame rate in ms.", GH_ParamAccess.item, 1000d);
            pManager.AddNumberParameter("Diffusion Rate A", "dA", "Determines how well chemical A diffuses.", GH_ParamAccess.item, 1d);
            pManager.AddNumberParameter("Diffusion Rate B", "dB", "Determines how well chemical B diffuses.", GH_ParamAccess.item, 0.3d);
            pManager.AddNumberParameter("Feed Rate", "F", "Rate of how fast B is fed in.", GH_ParamAccess.item, 0.055d);
            pManager.AddNumberParameter("Kill Rate", "K", "Rate of how fast A is killed.", GH_ParamAccess.item, 0.062d);
            pManager.AddNumberParameter("Grid Width", "X", "Resolution in X.", GH_ParamAccess.item, 100d);
            pManager.AddNumberParameter("Grid Depth", "Y", "Resolution in Y.", GH_ParamAccess.item, 100d);

        }

        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddTextParameter("Debug Log", "OUT", "Message log for debugging.", GH_ParamAccess.list);
            pManager.AddGenericParameter("RD Grid", "G", "RD Values as cellgrid.", GH_ParamAccess.item);
        }

        protected override void SolveInstance(IGH_DataAccess DA)
        {
            if (!DA.GetData(0, ref reset)) return;
            if (!DA.GetData(1, ref run)) return;
            if (!DA.GetData("Simulation Speed", ref speed)) return;

            if (reset)
            {
                double tempx = 0;
                double tempy = 0;
                double dA = 0;
                double dB = 0;
                double kill = 0;
                double feed = 0;

                frameCount = 0;


                if (!DA.GetData("Diffusion Rate A", ref dA)) return;
                if (!DA.GetData("Diffusion Rate B", ref dB)) return;
                if (!DA.GetData("Feed Rate", ref feed)) return;
                if (!DA.GetData("Kill Rate", ref kill)) return;
                if (!DA.GetData("Grid Width", ref tempx)) return;
                if (!DA.GetData("Grid Depth", ref tempy)) return;

                int xRes = (int)tempx; //convert double into int
                int yRes = (int)tempy;

                Setup(xRes, yRes, dA, dB, feed, kill);
            }

            if (run)
            {

                Update();
            }

            DA.SetDataList(0, debugLog);
            DA.SetData(1, rd.Grid);
        }

        bool reset, run;
        double speed;

        RDSolver2d rd = null;
        public static List<string> debugLog = new List<string>();
        public static int frameCount;


        public void Setup(int xRes_, int yRes_, double dA_, double dB_, double feed_, double kill_)
        {
            rd = new RDSolver2d(xRes_, yRes_, dA_, dB_, feed_, kill_);
            rd.Seed(50, 60, 50, 60);
            //Seed(20, 30, 60, 70);
            frameCount = 0;
        }

        public void Update()
        {
            debugLog.Clear();

            rd.Update();
            frameCount++;
            debugLog.Add(frameCount.ToString());
            foreach (Cell c in rd.Grid.Cells)
            {
                string s = String.Format("A: {0}; B: {1}", c.A, c.B);
                debugLog.Add(s);
            }
        }


        //Schedule a new solution after a specified time interval
        private void ScheduleCallBack(GH_Document doc) { this.ExpireSolution(false); }

        protected override void AfterSolveInstance()
        {
            if (!this.run) return;
            GH_Document ghDocument = OnPingDocument();
            ghDocument.ScheduleSolution((int)speed, new GH_Document.GH_ScheduleDelegate(this.ScheduleCallBack));
        }



        protected override System.Drawing.Bitmap Icon
        {
            get
            {
                // You can add image files to your project resources and access them like this:
                //return Resources.IconForThisComponent;
                return null;
            }
        }
        public override Guid ComponentGuid
        {
            get { return new Guid("080f7e50-9214-44c2-be63-e5ef8750bdcf"); }
        }
    }
}
