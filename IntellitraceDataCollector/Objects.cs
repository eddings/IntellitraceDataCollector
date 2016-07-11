using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace IntelliTraceDataCollector
{
    public enum CollectionMode 
    { 
        None,
        Web,
        Service,
        Executable
    }

    public enum CommandType
    {
        IISCollectionStart,
        IISCollectionStop,
        ServiceCollectionStart,
        ServiceCollectionStop,
        ProcessCollectionStart,
        ProcessCollectionStop
    }

    public interface ICommandStatus
    {
        bool Success { get; set; }
        CommandType CommandType { get; set; }
        string ErrorMesage { get; set; }
    }

    public class CommandStatus : ICommandStatus
    {
        public bool Success { get; set; }
        public CommandType CommandType { get; set; }
        public string ErrorMesage { get; set; }

        public CommandStatus() { }
    }

    public class IISCollectionCommandStatus : CommandStatus
    {
        public string ApplicationPool { get; set; }
        public int ProcessID { get; set; }
        public DateTime ProcessStartTime { get; set; }
        public string CollectionPlanPath { get; set; }
        public string OutputPath { get; set; }

        public IISCollectionCommandStatus() { }

    }

    public class WindowsServiceCollectionCommandStatus : CommandStatus
    {
        public string ServiceName { get; set; }
        public List<string> InfoMessages { get; set; }

        public WindowsServiceCollectionCommandStatus() 
        {
            this.InfoMessages = new List<string>();
        }

    }

    public class ProcessCollectionCommandStatus : CommandStatus
    {
        public string ProcessName { get; set; }
        public List<int> ProcessIDs { get; set;}

        public ProcessCollectionCommandStatus() 
        {
            this.ProcessIDs = new List<int>();
        }

    }
}
