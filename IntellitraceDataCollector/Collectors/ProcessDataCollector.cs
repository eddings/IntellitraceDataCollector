using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Management;
using System.Text;
using System.Threading;

namespace IntelliTraceDataCollector
{
    internal class ProcessDataCollector : BaseDataCollector
    {
        public string ProcessName { get; private set; }
        public string ProcessNameWithPath { get; private set; }
        public string CommandLineArguments { get; private set; }
        public List<int> ProcessIDs { get; private set; }

        public ProcessDataCollector(string collectionPlanNameAndPath, string output, string processName, string processNamePath, string arguments) 
            : base(output, collectionPlanNameAndPath)
        {
            //this.CollectionPlanNameWithPath = collectionPlanNameAndPath;
            //this.OutputPath = output;
            this.ProcessName = processName;
            this.ProcessNameWithPath = processNamePath;
            this.CommandLineArguments = arguments;
        }

        public ProcessDataCollector(List<int> procIds, string collectionPlanNameAndPath, string output)
            : base(output, collectionPlanNameAndPath)
        {
            this.ProcessIDs = procIds;
        }

        public override ICommandStatus StartCollection()
        {
            ProcessCollectionCommandStatus status = new ProcessCollectionCommandStatus();
            status.CommandType = CommandType.ProcessCollectionStart;
            status.ProcessName = this.ProcessName;

            string outputFileNameWithPath = string.Format(@"{0}\{1}", this.OutputPath, GetFileName());
            string arguments = string.Format(" launch /cp:\"{0}\"  /f:\"{1}\" \"{2}\" {3}", this.CollectionPlanNameWithPath, outputFileNameWithPath, this.ProcessNameWithPath, this.CommandLineArguments);
            ProcessStartInfo startInfo = new ProcessStartInfo();
            startInfo.CreateNoWindow = false;
            startInfo.UseShellExecute = false;
            startInfo.FileName = IntellitraceSCNameAndPath;
            startInfo.WindowStyle = ProcessWindowStyle.Hidden;
            startInfo.Arguments = arguments;
            try
            {
                Process proc = Process.Start(startInfo);
                status.Success = true;
                status.ProcessIDs.Add(proc.Id);
                Thread.Sleep(5000);
                status.ProcessIDs.AddRange(ListOfProcessesLaunched(proc.Id));
            }
            catch(Exception ex)
            {
                status.Success = false;
                status.ErrorMesage = ex.Message;
            }
            return status;
            
        }

        private string GetFileName()
        {
            DateTime now = DateTime.Now;
            string fileName = string.Format("{0}_{1}{2}{3}_{4}{5}{6}.itrace", this.ProcessName,  now.ToString("yy"), now.ToString("MM"), now.ToString("dd"),  now.ToString("HH"),  now.ToString("mm"), now.ToString("ss"));
            return fileName;
        }

        public override ICommandStatus StopCollection()
        {
            ProcessCollectionCommandStatus status = new ProcessCollectionCommandStatus();
            status.CommandType = CommandType.ProcessCollectionStop;
            status.ProcessName = this.ProcessName;

            foreach (int procId in this.ProcessIDs)
            {
                Process[] currentProcesses = Process.GetProcesses();
                Process proc = currentProcesses.FirstOrDefault(pr => pr.Id == procId);
                if (proc == null)
                    continue;
                
                try
                {
                    //KillProcessAndChildren(this.ProcessID);
                    proc.Kill();
                    Thread.Sleep(1000);
                    status.Success = true;
                }
                catch (Exception ex)
                {
                    status.Success = false;
                    status.ErrorMesage = ex.Message;
                }
            }
            status.Success = true;
            return status;
        }

        //private void KillProcessAndChildren(int pid)
        //{
        //    ManagementObjectSearcher searcher = new ManagementObjectSearcher("Select * From Win32_Process Where ParentProcessID=" + pid);
        //    ManagementObjectCollection moc = searcher.Get();
        //    foreach (ManagementObject mo in moc)
        //    {
        //        KillProcessAndChildren(Convert.ToInt32(mo["ProcessID"]));
        //    }
        //    try
        //    {
        //        Process proc = Process.GetProcessById(pid);
        //        proc.Kill();
        //    }
        //    catch (ArgumentException ex) 
        //    {
        //        // Process already exited.
        //    }
        //}

        private List<int> ListOfProcessesLaunched(int pid)
        {
            List<int> processList = new List<int>();
            ManagementObjectSearcher searcher = new ManagementObjectSearcher("Select * From Win32_Process Where ParentProcessID=" + pid);
            ManagementObjectCollection moc = searcher.Get();
            foreach (ManagementObject mo in moc)
            {
                processList.Add(Convert.ToInt32(mo["ProcessID"]));
            }
            return processList;
        }

        public override void GetCollectionStatus()
        {
            throw new NotImplementedException();
        }
    }
}
