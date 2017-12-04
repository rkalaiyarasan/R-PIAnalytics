using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RDotNet;

namespace RApplication
{
    class Program
    {
        
        static void Main(string[] args)
        {
            REngine engine;
            
            RSetup rs = new RSetup();
            engine = rs.setEngine();

            rs.createDF_new();
            Console.ReadKey();
            rs.demoARIMA();
            rs.simplePlot();
            Console.ReadKey();
            rs.scatterPlot();
            Console.ReadKey();
            rs.exit();
            Console.ReadKey();

        }
    }
}