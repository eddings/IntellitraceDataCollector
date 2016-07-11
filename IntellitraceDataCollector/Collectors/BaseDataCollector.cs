using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace IntelliTraceDataCollector
{
    internal abstract class BaseDataCollector : IIntelliTraceDataCollector
    {
        protected static string IntellitraceCollectorPath;
        protected static string IntellitracePSModuleNameAndPath;
        protected static string IntellitraceSCNameAndPath;

        public BaseDataCollector(string outputPath, string collectionPlanNameWithPath)
        {
            IntellitraceCollectorPath = string.Format(@"{0}\{1}", System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location), Constants.IntelliTraceSoftwareLocation);
            IntellitracePSModuleNameAndPath = string.Format(@"{0}\{1}", IntellitraceCollectorPath, Constants.IntellitracePSModuleName);
            IntellitraceSCNameAndPath = string.Format(@"{0}\{1}", IntellitraceCollectorPath, Constants.IntellitraceSCName);
            this.OutputPath = outputPath;
            this.CollectionPlanNameWithPath = collectionPlanNameWithPath;
        }

        public string OutputPath { get; private set; }
        public string CollectionPlanNameWithPath { get; private set; }

        public abstract ICommandStatus StartCollection();
        public abstract ICommandStatus StopCollection();
        public abstract  void GetCollectionStatus();

        private string calculatedPath = string.Empty;
        public string CalculatedPath
        {
            get
            {
                if ( string.IsNullOrEmpty(calculatedPath))
                {
                    string name;
                    if (this is IISDataCollector)
                        name = ((IISDataCollector)this).AppPoolName;
                    else if (this is WindowsServiceDataCollector)
                        name = ((WindowsServiceDataCollector)this).ServiceName;
                    else
                    {
                        name = ((ProcessDataCollector)this).ProcessName;

                    }
                    DateTime now = DateTime.Now;
                    string folderName = string.Format("{0}_{1}{2}{3}_{4}{5}{6}", name,  now.ToString("yy"), now.ToString("MM"), now.ToString("dd"),  now.ToString("HH"),  now.ToString("mm"), now.ToString("ss"));
                    calculatedPath = string.Format(@"{0}\{1}", this.OutputPath, folderName);
                }

                return calculatedPath;
            }
        }

    }

    

    
}
