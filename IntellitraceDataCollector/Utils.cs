using Microsoft.Web.Administration;
using System.Collections.Generic;
using System.Linq;
using System.ServiceProcess;
using System.IO;
using System.Security.AccessControl;
using System.Security.Principal;
using System;
using System.Management;
using System.Diagnostics;
using System.ComponentModel;
using System.Configuration;
using Microsoft.Win32;

namespace IntelliTraceDataCollector
{
    internal class Utils
    {
        private static bool ValidateServiceName(string serviceName)
        {
            ServiceController ctl = ServiceController.GetServices().FirstOrDefault(s => s.ServiceName.ToLower() == serviceName.ToLower());
            if (ctl == null)
                return false;
            else
                return true;
        }

        private static bool ValidateAppPoolName(string appPoolName)
        {
            ServerManager manager = new ServerManager();
            foreach (var appPool in manager.ApplicationPools)
            {
                if (appPool.Name.ToLower() == appPoolName.ToLower() )
                    return true;
            }
            return false;
        }

        internal static bool GrantEveryoneWritePermissionToFolder(string folderPath)
        {
            DirectorySecurity sec = Directory.GetAccessControl(folderPath);
            SecurityIdentifier everyone = new SecurityIdentifier(WellKnownSidType.WorldSid, null);
            sec.AddAccessRule(new FileSystemAccessRule(everyone, FileSystemRights.FullControl | FileSystemRights.Synchronize, InheritanceFlags.ContainerInherit | InheritanceFlags.ObjectInherit, PropagationFlags.None, AccessControlType.Allow));
            Directory.SetAccessControl(folderPath, sec);
            return true;
        }

        internal static bool DoesEveryoneHasWritePermissionOnFolder(string path)
        {
            SecurityIdentifier everyone = new SecurityIdentifier(WellKnownSidType.WorldSid, null);

            DirectoryInfo di = new DirectoryInfo(path);
            DirectorySecurity acl = di.GetAccessControl(AccessControlSections.All);
            AuthorizationRuleCollection rules = acl.GetAccessRules(true, true, typeof(NTAccount));

            foreach (AuthorizationRule rule in rules)
            {
                if (rule.IdentityReference.Translate(typeof(SecurityIdentifier)) == everyone.Translate(typeof(SecurityIdentifier)))
                {
                    //Cast to a FileSystemAccessRule to check for access rights
                    if ((((FileSystemAccessRule)rule).FileSystemRights & FileSystemRights.WriteData) > 0)
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
            }
            return false;
        }

        internal static List<string> GetListOfAppPools()
        {
            List<string> appPools = new List<string>();
            ServerManager manager = new ServerManager();
            foreach (var appPool in manager.ApplicationPools)
            {
                appPools.Add(appPool.Name);
            }
            return appPools;
        }

        internal static List<string> GetAllServices()
        {
            List<string> services = new List<string>();
            ManagementObjectSearcher searcher = new ManagementObjectSearcher("SELECT * FROM Win32_Service");

            foreach (ManagementObject result in searcher.Get())
            {
                string name = (string)result["Name"];
                services.Add(name);
            }
            return services;
        }

        internal bool IsThisADotNetProcess(int procId)
        {
            using (Process proc = Process.GetProcessById(procId))
            {
                var modules = proc.Modules.Cast<ProcessModule>().Where(
                                m => m.ModuleName.CompareTo("clr.dll") == 0 || m.ModuleName.CompareTo("mscorwks.dll") == 0);

                return modules.Any();
            }
        }

        internal int GetProcessIdForService(string serviceName)
        {
            ManagementObjectSearcher searcher = new ManagementObjectSearcher(string.Format("SELECT * FROM Win32_Service WHERE Name = '{0}'", serviceName));
            foreach (ManagementObject result in searcher.Get())
            {

                bool started = Convert.ToBoolean(result["Started"]);
                if (started)
                {
                    int pid = Convert.ToInt32(result["ProcessId"]);
                    return pid;
                }
            }
            return -1;
        }

        internal static bool HasSelectedServicesForCollection()
        {
            ServiceConfigurationSection serviceConfigSection = ConfigurationManager.GetSection("ServicesSection") as ServiceConfigurationSection;
            if (serviceConfigSection == null)
                return false;

            return serviceConfigSection.Services.Count > 0;
        }

        internal static List<string> GetSelectedServices()
        {
            List<string> services = new List<string>();
            ServiceConfigurationSection serviceConfigSection = ConfigurationManager.GetSection("ServicesSection") as ServiceConfigurationSection;

            for (int i = 0; i < serviceConfigSection.Services.Count; i++)
            {
                if ( ValidateServiceName(serviceConfigSection.Services[i].Name))
                    services.Add(serviceConfigSection.Services[i].Name);
            }
            return services;
        }

        internal static bool HasSelectedAppPoolsForCollection()
        {
            AppPoolConfigurationSection appPoolConfigSection = ConfigurationManager.GetSection("AppPoolsSection") as AppPoolConfigurationSection;
            if (appPoolConfigSection == null)
                return false;

            return appPoolConfigSection.AppPools.Count > 0;
        }

        internal static List<string> GetSelectedAppPools()
        {
            List<string> appPools = new List<string>();
            AppPoolConfigurationSection appPoolConfigSection = ConfigurationManager.GetSection("AppPoolsSection") as AppPoolConfigurationSection;

            for (int i = 0; i < appPoolConfigSection.AppPools.Count; i++)
            {
                if (ValidateAppPoolName(appPoolConfigSection.AppPools[i].Name))
                appPools.Add(appPoolConfigSection.AppPools[i].Name);
            }

            return appPools;
        }

        internal static bool IsIISInstalled()
        {
            return (Registry.LocalMachine.OpenSubKey(@"Software\Microsoft\InetStp") != null);
        }
    }
}


