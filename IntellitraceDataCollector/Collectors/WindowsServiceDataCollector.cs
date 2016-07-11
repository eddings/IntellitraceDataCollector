using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.ServiceProcess;
using System.Xml;

namespace IntelliTraceDataCollector
{
    internal class WindowsServiceDataCollector : BaseDataCollector
    {
        public string ServiceName { get; private set; }
        private const string serviceKeyRegistryPath = @"SYSTEM\CurrentControlSet\services\";

        public WindowsServiceDataCollector(string collectionPlanNameAndPath, string output, string serviceName)
            : base(output, collectionPlanNameAndPath)
        {
            //this.CollectionPlanNameWithPath = collectionPlanNameAndPath;
            //this.OutputPath = output;
            this.ServiceName = serviceName;
        }

        public override ICommandStatus StartCollection()
        {
            WindowsServiceCollectionCommandStatus status = new WindowsServiceCollectionCommandStatus();
            status.ServiceName = this.ServiceName;
            status.CommandType = CommandType.ServiceCollectionStart;

            //if (!Utils.DoesEveryoneHasWritePermissionOnFolder(this.CalculatedPath))
            //{
            //    Utils.GrantEveryoneWritePermissionToFolder(this.CalculatedPath);
                
            //}

            CommandStatus updatePlanStatus = (CommandStatus)UpdatePlanForOutputFolderPath();
            if ( updatePlanStatus.Success)
            {
                status.InfoMessages.Add("Collection Plan updated successfully for Output path.");
            }
            else
            {
                status.InfoMessages.Add(updatePlanStatus.ErrorMesage);
                status.Success = false;
                return status;
            }

            CommandStatus createRegistryStatus = (CommandStatus)CreateRegistryEntryForIntellitrace();
            if (createRegistryStatus.Success)
            {
                status.InfoMessages.Add(string.Format("Registry Key IntelliTrace data collection successfully updated for service {0}.", this.ServiceName));
            }
            else
            {
                status.InfoMessages.Add(createRegistryStatus.ErrorMesage);
                status.Success = false;
                return status;
            }


            CommandStatus resetServiceStatus;

            ServiceController service = new ServiceController(this.ServiceName);
            if (service.Status == ServiceControllerStatus.Running)
            {
                resetServiceStatus= (CommandStatus)ResetService(false);
                if (resetServiceStatus.Success)
                {
                    status.InfoMessages.Add(string.Format("Service {0} stopped collection successfully.", this.ServiceName));
                }
                else
                {
                    status.InfoMessages.Add(resetServiceStatus.ErrorMesage);
                    status.Success = false;
                    return status;
                }
            }
            else
            {
                status.InfoMessages.Add(string.Format("Service {0} state = {1}.", this.ServiceName, service.Status.ToString()));
            }

            resetServiceStatus = (CommandStatus)ResetService(true);
            if (resetServiceStatus.Success)
            {
                status.InfoMessages.Add(string.Format("Service {0} started collection successfully.", this.ServiceName));
            }
            else
            {
                status.InfoMessages.Add(resetServiceStatus.ErrorMesage);
                status.Success = false;
                return status;
            }

            status.Success = true;
            return status;
        }

        public override ICommandStatus StopCollection()
        {
            WindowsServiceCollectionCommandStatus status = new WindowsServiceCollectionCommandStatus();
            status.ServiceName = this.ServiceName;
            status.CommandType = CommandType.ServiceCollectionStart;
            List<string> info = new List<string>();

            CommandStatus removeRegistryStatus = (CommandStatus)RemoveRegistryEntryForIntellitrace();
            if (removeRegistryStatus.Success)
            {
                info.Add(string.Format("Registry Key IntelliTrace data collection removed for service {0}.", this.ServiceName));
            }
            else
            {
                info.Add(removeRegistryStatus.ErrorMesage);
                status.Success = false;
                status.InfoMessages = info;
                return status;
            }


            CommandStatus resetServiceStatus;
            ServiceController service = new ServiceController(this.ServiceName);
            if (service.Status == ServiceControllerStatus.Running)
            {
                resetServiceStatus = (CommandStatus)ResetService(false);
                if (resetServiceStatus.Success)
                {
                    info.Add(string.Format("Service {0} stopped collection successfully.", this.ServiceName));
                }
                else
                {
                    info.Add(resetServiceStatus.ErrorMesage);
                    status.Success = false;
                    status.InfoMessages = info;
                    return status;
                }
            }
            else
            {
                info.Add(string.Format("Service {0} state = {1}.", this.ServiceName, service.Status.ToString()));
            }

            resetServiceStatus = (CommandStatus)ResetService(true);
            if (resetServiceStatus.Success)
            {
                info.Add(string.Format("Service {0} started collection successfully.", this.ServiceName));
            }
            else
            {
                info.Add(resetServiceStatus.ErrorMesage);
                status.Success = false;
                status.InfoMessages = info;
                return status;
            }

            status.Success = true;
            status.InfoMessages = info;
            return status;
        }

        public override void GetCollectionStatus()
        {
            return;
        }

        private ICommandStatus CreateRegistryEntryForIntellitrace()
        {
            CommandStatus status = new CommandStatus();
            string enableProfiling = "COR_ENABLE_PROFILING=1";
            string profilerGUID = "COR_PROFILER={AAAAAA70-DFED-4CB4-A1D6-920F51E9674A}";
            string profilerPath = string.Format("COR_PROFILER_PATH={0}", Path.GetFullPath(string.Format(@"{0}\{1}", IntellitraceCollectorPath, Constants.IntellitraceProfilerPathDllx64)));
            string collectionPlan = string.Format("VSLOGGER_CPLAN={0}", Path.GetFullPath(this.CollectionPlanNameWithPath));

            try
            {
                using (RegistryKey Key = Registry.LocalMachine.OpenSubKey(string.Format("{0}{1}", serviceKeyRegistryPath, this.ServiceName), true))
                {
                    if (Key != null)
                    {
                        Key.SetValue("Environment", new string[] { enableProfiling, profilerGUID, profilerPath, collectionPlan });
                        status.Success = true;
                    }
                    else
                    {
                        status.Success = false;
                        status.ErrorMesage = string.Format("CreateRegistryEntryForIntellitrace: Registry Key {0}{1} could not be located.", serviceKeyRegistryPath, this.ServiceName);
                    }
                }
            }
            catch (Exception ex)
            {
                status.Success = false;
                status.ErrorMesage = "CreateRegistryEntryForIntellitrace: " + ex.Message;
            }

            return status;
        }

        private ICommandStatus RemoveRegistryEntryForIntellitrace()
        {
            CommandStatus status = new CommandStatus();
            try
            {
                using (RegistryKey Key = Registry.LocalMachine.OpenSubKey(string.Format("{0}{1}", serviceKeyRegistryPath, this.ServiceName), true))
                {
                    if (Key != null)
                    {
                        Key.DeleteValue("Environment");
                        status.Success = true;
                    }
                    else
                    {
                        status.Success = false;
                        status.ErrorMesage = string.Format("RemoveRegistryEntryForIntellitrace: Registry Key {0}{1} could not be located.", serviceKeyRegistryPath, this.ServiceName);
                    }
                }
            }
            catch (Exception ex)
            {
                status.Success = false;
                status.ErrorMesage = "RemoveRegistryEntryForIntellitrace: " + ex.Message;
            }

            return status;
        }

        

        private ICommandStatus UpdatePlanForOutputFolderPath()
        {
            CommandStatus status = new CommandStatus();
            //string collectionPlanNamePath = string.Format(@"{0}\{1}", IntellitraceCollectorPath, CollectionPlanFileName);

            try
            {
                XmlDocument doc = new XmlDocument();
                XmlNamespaceManager manager = new XmlNamespaceManager(doc.NameTable);
                manager.AddNamespace("tlog", "urn:schemas-microsoft-com:visualstudio:tracelog");
                doc.Load(this.CollectionPlanNameWithPath);

                var node = doc.SelectSingleNode("//tlog:LogFileDirectory", manager);
                node.InnerText = this.OutputPath;
                doc.Save(this.CollectionPlanNameWithPath);
                status.Success = true;
            }
            catch (Exception ex)
            {
                status.Success = false;
                status.ErrorMesage = "UpdatePlanForOutputFolderPath: " + ex.Message;
            }
            return status;

        }

        private ICommandStatus ResetService(bool start)
        {
            CommandStatus status = new CommandStatus();
            string header =  start == true ? "Start Service " : "Stop Service ";

            int timeoutInMillisecond = 60000;   // 1  minute
            try
            {
                ServiceController service = new ServiceController(this.ServiceName);
                if (start)
                {
                    service.Start();
                    service.WaitForStatus(ServiceControllerStatus.Running, TimeSpan.FromMilliseconds(timeoutInMillisecond));
                }
                else
                {
                    service.Stop();
                    service.WaitForStatus(ServiceControllerStatus.Stopped, TimeSpan.FromMilliseconds(timeoutInMillisecond));
                }
                
            }
            catch(System.ServiceProcess.TimeoutException tex)
            {
                status.Success = false;
                status.ErrorMesage = "ResetService: " + header + "timed out.";
            }
            catch(Exception ex)
            {
                status.Success = false;
                status.ErrorMesage = "ResetService: " + header + ex.Message;
            }
            status.Success = true;
            return status;
        }

        

    }
}
