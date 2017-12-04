using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OSIsoft.AF;
using OSIsoft.AF.PI;
using OSIsoft.AF.Time;
using OSIsoft.AF.Data;

namespace RApplication
{
    class PISetup
    {
        PIServer piServer;

        public PISetup()
        {
            piServer = PIServers.GetPIServers().DefaultPIServer;
        }


        /// <summary>
        /// Retrieves the PI Point history data from the data archive for the specified time range
        /// </summary>
        /// <returns> the frame containts the history data generated for the points as matrix</returns>
        public Object[,] getArchiveValues_new()
        {


            List<PIPoint> ptList = new List<PIPoint>();
            DateTime starttime = AFTime.Parse("t");
            DateTime endtime = AFTime.Parse("t-2d");

            AFTimeRange timeRange = AFTimeRange.Parse(starttime.ToString(), endtime.ToString());
            var boundaryType = AFBoundaryType.Inside;

            List<OSIsoft.AF.Asset.AFValues> recordList = new List<OSIsoft.AF.Asset.AFValues>();



            ptList.Add(PIPoint.FindPIPoint(piServer, "BA:TEMP.1"));
            ptList.Add(PIPoint.FindPIPoint(piServer, "BA:CONC.1"));
            ptList.Add(PIPoint.FindPIPoint(piServer, "BA:LEVEL.1"));
            ptList.Add(PIPoint.FindPIPoint(piServer, "BA:PHASE.1"));
            ptList.Add(PIPoint.FindPIPoint(piServer, "BA:ACTIVE.1"));

            foreach (PIPoint pt in ptList)
            {
                recordList.Add(pt.RecordedValues(timeRange, boundaryType, "", false));
            }

            //multi array with mixed objects

            List<Int32> mxl = new List<Int32>();
            mxl.Add(recordList[0].Count);
            mxl.Add(recordList[1].Count);
            mxl.Add(recordList[2].Count);
            mxl.Add(recordList[3].Count);
            mxl.Add(recordList[4].Count);
            int mxl_max = mxl.Max();

            Object[,] frame = new Object[ptList.Count+1,mxl_max];
            int i = 1;
            
            foreach (OSIsoft.AF.Asset.AFValues vals in recordList)
            {
                int j = 0;
                foreach (OSIsoft.AF.Asset.AFValue afval in vals)
                {
                   if (i < ptList.Count+1 && j < mxl_max)
                    {
                        if(afval.Timestamp != null)
                            frame[0, j] = DateTime.Parse(afval.Timestamp.LocalTime.ToString());
                        if (afval.Value != null)
                            frame[i, j] = afval.Value;
                        j += 1;
                    }
                        

                }
                i += 1;
            }
       return frame;
        }
    }
}
