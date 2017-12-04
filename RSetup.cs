using System;
using System.IO;
using System.Diagnostics;
using RDotNet;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RApplication
{
    class RSetup
    {
        //Declare the REngine Object
        REngine engine;

        //Set the R Environment
        public RSetup()
        {
            setPath();
        }

        //Shut the REngine
        public void exit()
        {
            engine.Close();
        }

        //Set the R Environment Path
        public static void setPath()
        {
            var oldpath = System.Environment.GetEnvironmentVariable("PATH");
            string rPath32 = @"C:\Program Files\R\R-3.3.2\bin\i386";
            string rPath64 = @"C:\Program Files\R\R-3.3.2\bin\x64";
            var rPath = System.Environment.Is64BitProcess ? rPath64 : rPath32;

            if (Directory.Exists(rPath) == false)
                throw new DirectoryNotFoundException(string.Format("Could not find the specified path to the directory containing R.dll: {0}", rPath));

            var newPath = string.Format("{0}{1}{2}", rPath, System.IO.Path.PathSeparator, oldpath);

            System.Environment.SetEnvironmentVariable("PATH", newPath);
                        
        }

        //Set REngine
        public REngine setEngine()
        {
            engine = REngine.CreateInstance("RDotNet");
            engine.Initialize();
            return engine;
        }

        //Load Forecast Library
        public void loadForecastLib()
        {
            Console.WriteLine("Shall I load the forecast library Y/N ?");
            string r = Console.ReadLine();
            if(r.Equals("Y"))
                engine.Evaluate("library(\"forecast\")");
                Console.WriteLine("Forecast Library loaded...");
        }

        //Seasonal Plotting on the PI data
        public void plotSeason()
        {
            Console.WriteLine("Shall I plot season, Y/N ?");
            string r = Console.ReadLine();
            if (r.Equals("Y"))
                engine.Evaluate("monthplot(piData$BA.TEMP.1)");
                Console.WriteLine("Seasonal plot generated...");
        }

        //Simple plot the PI Data
        public void plotPairs()
        {
            Console.WriteLine("Shall I plot pairs, Y/N ?");
            string r = Console.ReadLine();
            if (r.Equals("Y"))
                engine.Evaluate("pairs(piData[,c(3,4,6)], pch=19)");
                Console.WriteLine("Pair plot generated...");
        }

        //Print Summary
        public void listSummary(String val)
        {
            Console.WriteLine("Shall I list summary, Y/N ?");
            string r = Console.ReadLine();
            if (r.Equals("Y"))
                engine.Evaluate("Summary("+val+")");
                Console.WriteLine("");
                Console.WriteLine("Summary generated...");
        }

        // Evaluate the correlation between pairs
        public void calcCorrelation(String val1, String val2)
        {
            Console.WriteLine("Do you want to calculate correlation, Y/N ?");
            string r = Console.ReadLine();
            if (r.Equals("Y"))
                engine.Evaluate("cor("+val1+","+val2+")");
        }

        //ACF Plotting
        public void calcACF(String val)
        {
            engine.Evaluate("ACF(" + val + ")");
        }

        //Lag plotting
        public void plotLag(String val)
        {
            engine.Evaluate("Summary(" + val + ","+" lags = 9"+")");
        }

        // SImple Forecasting using ARIMA Modelling
        public void demoARIMA()
        {
            engine.Evaluate("library(\"forecast\")");
            engine.Evaluate("piData <- read.csv(\"C:/Users/rkalaiyarasan/Desktop/Data_Analysis/data.csv\", header = TRUE, sep = \",\")");
            engine.Evaluate("piData$Timestamp <- strptime(piData$Timestamp, format = \"%m/%d/%Y %H:%M:%S\")");
            engine.Evaluate("piData$time <- as.Date(piData$Timestamp, \"%m/%d/%Y %H:%M:%S\")");
            engine.Evaluate("View(piData)");
            
            
            engine.Evaluate("source(\"C:/Users/rkalaiyarasan/Desktop/Data_Analysis/forecastModified.R\")");
            engine.Evaluate("result.arima <- forecastArima(piData, n.ahead = 90)");
            engine.Evaluate("source(\"C:/Users/rkalaiyarasan/Desktop/Data_Analysis/plotForecastResult.R\")");
            engine.Evaluate("print(plotForecastResult(result.arima, title = \"Forecasting with ARIMA\"))");
        }

        //Method to create DataFrame
        public void createDF()
        {
            PISetup ps = new PISetup();

            Dictionary<DateTime, Object> dict = ps.getArchiveValues();

            Double[] db = new double[dict.Count];
            String[] dt = new String[dict.Count];

            int i = 0;
            foreach(object ob in dict.Values)
            {
                db[i] = Convert.ToDouble(ob);
                i += 1;
            }

            int j = 0;
            foreach (DateTime da in dict.Keys)
            {
                dt[j] = Convert.ToString(da);
                j += 1;
            }

            NumericVector myNum = engine.CreateNumericVector(db);
            CharacterVector myCha = engine.CreateCharacterVector(dt);

            engine.SetSymbol("Time", myCha);
            engine.SetSymbol("BA.TEMP.1", myNum);
            engine.Evaluate("df <- data.frame(Timestamp=c(Time), BA.TEMP.1=c(BA.TEMP.1))");
            //engine.Evaluate("write.csv(df, file = \"C:/File_1.csv\")");
            //engine.Evaluate("View(df)");
        }

        // Method to write the data frame to csv file
        public void write_to_csv(String fileName)
        {
            engine.Evaluate("write.csv(df, file = \"C:/"+fileName+")");
        }

        //Method to create DataFrame
        public void createDF_new()
        {
            PISetup ps = new PISetup();

            Object[,] obj = ps.getArchiveValues_new();

            String[] date = new String[obj.GetLength(1)];
            String[] phase = new String[obj.GetLength(1)];
            String[] status = new String[obj.GetLength(1)];
            Double[] temp = new double[obj.GetLength(1)];
            Double[] conc = new double[obj.GetLength(1)];
            Double[] level = new double[obj.GetLength(1)];

            for (int i = 0; i < obj.GetLength(1); i++)
            {
                date[i] = Convert.ToString(obj[0, i]);
            }

            for (int i = 0; i < obj.GetLength(1); i++)
            {
                phase[i] = Convert.ToString(obj[4, i]);
            }

            for (int i = 0; i < obj.GetLength(1); i++)
            {
                status[i] = Convert.ToString(obj[5, i]);
            }


            for (int i=0; i<obj.GetLength(1);i++)
            {
                temp[i] = Convert.ToDouble(obj[1,i]);
            }

            for (int i = 0; i < obj.GetLength(1); i++)
            {
                conc[i] = Convert.ToDouble(obj[2, i]);
            }

            for (int i = 0; i < obj.GetLength(1); i++)
            {
                level[i] = Convert.ToDouble(obj[3, i]);
            }

            NumericVector temp_vec = engine.CreateNumericVector(temp);
            NumericVector conc_vec = engine.CreateNumericVector(conc);
            NumericVector level_vec = engine.CreateNumericVector(level);
            CharacterVector date_vec = engine.CreateCharacterVector(date);
            CharacterVector phase_vec = engine.CreateCharacterVector(phase);
            CharacterVector status_vec = engine.CreateCharacterVector(status);

            engine.SetSymbol("Time", date_vec);
            engine.SetSymbol("BA.TEMP.1", temp_vec);
            engine.SetSymbol("BA.CONC.1", conc_vec);
            engine.SetSymbol("BA.LEVEL.1", level_vec);
            engine.SetSymbol("BA.PHASE.1", phase_vec);
            engine.SetSymbol("BA.ACTIVE.1", status_vec);
            write_to_csv("name.csv");
            engine.Evaluate("df <- data.frame(Timestamp=c(Time), BA.TEMP.1=c(BA.TEMP.1), BA.CONC.1=c(BA.CONC.1), BA.LEVEL.1=c(BA.LEVEL.1), BA.PHASE.1=c(BA.PHASE.1), BA.ACTIVE.1=c(BA.ACTIVE.1))");
            
            //engine.Evaluate("View(df)");
        }

        /*
        public void demoARIMA_1()
        {
            engine.Evaluate("library(\"forecast\")");
            //engine.Evaluate("piData <- read.csv(\"C:/Users/rkalaiyarasan/Desktop/Data_Analysis/data.csv\", header = TRUE, sep = \",\")");
            engine.Evaluate("df$Timestamp <- strptime(df$Timestamp, format = \"%m/%d/%Y %H:%M:%S\")");
            engine.Evaluate("df$time <- as.Date(df$Timestamp, \"%m/%d/%Y %H:%M:%S\")");
            //engine.Evaluate("View(piData)");


            engine.Evaluate("source(\"C:/Users/rkalaiyarasan/Desktop/Data_Analysis/forecastModified.R\")");
            engine.Evaluate("result.arima <- forecastArima(df, n.ahead = 90)");
            engine.Evaluate("source(\"C:/Users/rkalaiyarasan/Desktop/Data_Analysis/plotForecastResult.R\")");
            engine.Evaluate("print(plotForecastResult(result.arima, title = \"Forecasting with ARIMA\"))");
        }
        */

        //Scatter Plot
        public void scatterPlot()
        {
            engine.Evaluate("library(\"ggvis\")");
            engine.Evaluate("print(View(df))");
            engine.Evaluate("print(df %>% ggvis(~BA.CONC.1,~BA.LEVEL.1) %>% layer_points())");
        }

        //Simple Plotting
        public void simplePlot()
        {
            engine.Evaluate("print(plot(df$Timestamp,df$BA.LEVEL.1, type = \"l\"))");
            Console.WriteLine("Generated simple plot");
            
        }
    }
}
