using System;
using System.Collections.Generic;
using System.Linq;
using System.Management.Automation;
using System.Management.Automation.Runspaces;
using System.Text;

namespace IntelliTraceDataCollector
{
    internal class IISDataCollector : BaseDataCollector
    {
        public string AppPoolName { get; private set; }
        
        public IISDataCollector(string collectionPlanNameAndPath, string output, string appPoolName) 
            : base(output, collectionPlanNameAndPath)
        {
            //this.CollectionPlanNameWithPath = collectionPlanNameAndPath;
            //this.OutputPath = output;
            this.AppPoolName = appPoolName;
        }

        public override ICommandStatus StartCollection()
        {
            CommandStatus status = new CommandStatus();
            status.CommandType = CommandType.IISCollectionStart;
            try
            {
                using (PowerShell PowerShellInstance = PowerShell.Create())
                {
                    InitialSessionState session = InitialSessionState.CreateDefault();
                    session.ImportPSModule(new string[] { IntellitracePSModuleNameAndPath });
                    Runspace runspace = RunspaceFactory.CreateRunspace(session);
                    runspace.Open();

                    PowerShellInstance.Runspace = runspace;
                    PowerShellInstance.Commands.AddCommand(@"Start-IntelliTraceCollection");
                    PowerShellInstance.Commands.AddParameter("Confirm", false);
                    PowerShellInstance.Commands.AddParameter("ApplicationPool", this.AppPoolName);
                    PowerShellInstance.Commands.AddParameter("CollectionPlan", this.CollectionPlanNameWithPath);
                    PowerShellInstance.Commands.AddParameter("OutputPath", this.OutputPath);

                    var results = PowerShellInstance.Invoke();
                }
                status.Success = true;

            }
            catch (Exception ex)
            {
                status.Success = false;
                status.ErrorMesage = ex.Message;
            }
            return status;
        }

        public override ICommandStatus StopCollection()
        {
            CommandStatus status = new CommandStatus();
            status.CommandType = CommandType.IISCollectionStop;
            try
            {
                using (PowerShell PowerShellInstance = PowerShell.Create())
                {
                    InitialSessionState session = InitialSessionState.CreateDefault();
                    session.ImportPSModule(new string[] { IntellitracePSModuleNameAndPath });
                    Runspace runspace = RunspaceFactory.CreateRunspace(session);
                    runspace.Open();

                    PowerShellInstance.Runspace = runspace;
                    PowerShellInstance.Commands.AddCommand(@"Stop-IntelliTraceCollection");
                    PowerShellInstance.Commands.AddParameter("Confirm", false);
                    PowerShellInstance.Commands.AddParameter("ApplicationPool", this.AppPoolName);

                    var results = PowerShellInstance.Invoke();
                }
                status.Success = true;
            }
            catch (Exception ex)
            {
                status.Success = false;
                status.ErrorMesage = ex.Message;
            }
            return status;
        }

        public override void GetCollectionStatus()
        {
            return;
        }
    }
}
