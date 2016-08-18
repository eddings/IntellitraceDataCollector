using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Linq;
using System.Diagnostics;

namespace IntelliTraceDataCollector
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        List<LogData> logs = new List<LogData>();
        //DispatcherTimer statusTimer = null;
        CollectionMode collectionMode = CollectionMode.None;
        private string processName = string.Empty;
        private List<int> processIds = new List<int>();
        private string collectionPlanNameAndPath = string.Empty;
        

        public MainWindow()
        {
            InitializeComponent();
            this.lvLogs.ItemsSource = logs;
        }

        private void btnStartCollection_Click(object sender, RoutedEventArgs e)
        {
            if (!Validate())
                return;

            UpdateActionStarted();
            if ( this.collectionMode == CollectionMode.Web )
            {

                BaseDataCollector collector = new IISDataCollector(this.collectionPlanNameAndPath, this.txtOutputLocation.Text.Trim().ToString(), (string)this.cbAppPools.SelectedValue);
                
                Task<ICommandStatus> task = Task<ICommandStatus>.Factory.StartNew(() => collector.StartCollection());
                task.ContinueWith((antecendent) => 
                {
                    UpdateActionCompleted((CommandStatus)task.Result);
                }, TaskScheduler.FromCurrentSynchronizationContext());
            }
            else if ( this.collectionMode == CollectionMode.Service)
            {
                BaseDataCollector collector = new WindowsServiceDataCollector(this.collectionPlanNameAndPath, this.txtOutputLocation.Text.Trim().ToString(), this.cbService.SelectedItem.ToString());

                Task<ICommandStatus> task = Task<ICommandStatus>.Factory.StartNew(() => collector.StartCollection());
                task.ContinueWith((antecendent) =>
                {
                    UpdateActionCompleted((WindowsServiceCollectionCommandStatus)task.Result);
                }, TaskScheduler.FromCurrentSynchronizationContext());
            }
            else
            {
                BaseDataCollector collector = new ProcessDataCollector(this.collectionPlanNameAndPath, this.txtOutputLocation.Text.Trim().ToString(), Path.GetFileName(this.txtExecutablePath.Text.Trim()),  this.txtExecutablePath.Text.Trim(), this.txtCommandLine.Text.Trim());
                Task<ICommandStatus> task = Task<ICommandStatus>.Factory.StartNew(() => collector.StartCollection());
                task.ContinueWith((antecendent) =>
                {
                    UpdateActionCompleted((ProcessCollectionCommandStatus)task.Result);
                }, TaskScheduler.FromCurrentSynchronizationContext());
            }
        }

        private void btnStopCollection_Click(object sender, RoutedEventArgs e)
        {

            //ColorAnimation myColorAnimation = new ColorAnimation();
            //myColorAnimation.From = Colors.White;
            //myColorAnimation.To = Colors.Green;
            
            //myColorAnimation.Duration = new Duration(TimeSpan.FromMilliseconds(1000));
            //myColorAnimation.RepeatBehavior = RepeatBehavior.Forever;

            //Storyboard myStoryboard = new Storyboard();
            //myStoryboard.Children.Add(myColorAnimation);
            //Storyboard.SetTargetName(myColorAnimation, "Button1");
            //Storyboard.SetTargetProperty(myColorAnimation, new PropertyPath("Background.Color"));
            //myStoryboard.Begin(this);
            //myStoryboard.Stop(this);
            

            UpdateActionStarted();
            if (this.collectionMode == CollectionMode.Web)
            {
                BaseDataCollector collector = new IISDataCollector(this.collectionPlanNameAndPath, this.txtOutputLocation.Text.Trim().ToString(), (string)this.cbAppPools.SelectedValue);
                
                Task<ICommandStatus> task = Task<ICommandStatus>.Factory.StartNew(() => collector.StopCollection());
                task.ContinueWith((antecendent) =>
                {
                    UpdateActionCompleted((CommandStatus)task.Result);
                }, TaskScheduler.FromCurrentSynchronizationContext());
            }
            else if (this.collectionMode == CollectionMode.Service)
            {
                BaseDataCollector collector = new WindowsServiceDataCollector(this.collectionPlanNameAndPath, this.txtOutputLocation.Text.Trim().ToString(), this.cbService.SelectedItem.ToString());
                

                Task<ICommandStatus> task = Task<ICommandStatus>.Factory.StartNew(() => collector.StopCollection());
                task.ContinueWith((antecendent) =>
                {
                    UpdateActionCompleted((WindowsServiceCollectionCommandStatus)task.Result);
                }, TaskScheduler.FromCurrentSynchronizationContext());
                
            }
            else
            {
                BaseDataCollector collector = new ProcessDataCollector( this.processIds, this.collectionPlanNameAndPath, this.txtOutputLocation.Text.Trim().ToString() );
                Task<ICommandStatus> task = Task<ICommandStatus>.Factory.StartNew(() => collector.StopCollection());
                task.ContinueWith((antecendent) =>
                {
                    UpdateActionCompleted((ProcessCollectionCommandStatus)task.Result);
                }, TaskScheduler.FromCurrentSynchronizationContext());

            }
            
        }

        private void UpdateActionStarted()
        {
            this.pbCollection.IsIndeterminate = true;
            this.Cursor = Cursors.Wait;
        }

        private void UpdateActionCompleted(ICommandStatus result)
        {
            string commandType = "", message = "";

            this.pbCollection.IsIndeterminate = false;

            switch(result.CommandType)
            {
                case CommandType.IISCollectionStart:
                    commandType = "IIS Collection Start";
                    break;
                case CommandType.IISCollectionStop:
                    commandType = "IIS Collection Stop";
                    break;
                case CommandType.ServiceCollectionStart:
                    commandType = "Service Start";
                    break;
                case CommandType.ServiceCollectionStop:
                    commandType = "Service Stop";
                    break;
                case CommandType.ProcessCollectionStart:
                    commandType = "Process Collection Start";
                    break;
                case CommandType.ProcessCollectionStop:
                    commandType = "Process Collection Stop";
                    break;
            }

            if (result is WindowsServiceCollectionCommandStatus)
            {
                WindowsServiceCollectionCommandStatus wsStatus = (WindowsServiceCollectionCommandStatus)result;
                AddToLog(wsStatus.InfoMessages);
            }
            else if ( result is ProcessCollectionCommandStatus)
            {
                ProcessCollectionCommandStatus pStatus = (ProcessCollectionCommandStatus)result;

                if (result.Success)
                {
                    message = string.Format("Process {0} {1} successfully for Intellitrace Collection.", pStatus.ProcessName, (pStatus.CommandType == CommandType.ProcessCollectionStart ? "started" : "stopped") );
                    this.processIds = pStatus.ProcessIDs;
                    foreach(int id in processIds)
                        AddToLog(string.Format("process id = {0}", id));
                }
                else
                {
                    message = string.Format("Process {0} Failed to {1} for Intellitrace Collection - {2}.", pStatus.ProcessName, (pStatus.CommandType == CommandType.ProcessCollectionStart ? "start" : "stop"), result.ErrorMesage);
                }
                AddToLog(message);
            }
            else if (result is CommandStatus)
            {

                if (result.Success)
                {
                    message = string.Format("{0} Successful.", commandType);
                }
                else
                {
                    message = string.Format("{0} Failed - {1}.", commandType, result.ErrorMesage);
                }
                AddToLog(message);
            }
            this.Cursor = Cursors.Arrow;
        }

        private void RadioButton_Checked(object sender, RoutedEventArgs e)
        {
            var button = sender as RadioButton;
            
            if ( button.Content.ToString().CompareTo("Web") == 0 )
            {
                this.cbService.IsEnabled = false;
                this.cbAppPools.IsEnabled = true;
                this.txtExecutablePath.IsEnabled = false;
                this.btnBrowseExecutablePath.IsEnabled = false;
                this.txtCommandLine.IsEnabled = false;
                collectionMode = CollectionMode.Web;
            }
            else if (button.Content.ToString().CompareTo("Windows Service") == 0)
            {
                this.cbService.IsEnabled = true;
                this.cbAppPools.IsEnabled = false;
                this.txtExecutablePath.IsEnabled = false;
                this.btnBrowseExecutablePath.IsEnabled = false;
                this.txtCommandLine.IsEnabled = false;
                collectionMode = CollectionMode.Service;
            }
            else
            {
                this.cbService.IsEnabled = false;
                this.cbAppPools.IsEnabled = false;
                this.txtExecutablePath.IsEnabled = true;
                this.btnBrowseExecutablePath.IsEnabled = true;
                this.txtCommandLine.IsEnabled = true;
                collectionMode = CollectionMode.Executable;
            }
        }

        private bool Validate()
        {
            if ( collectionMode == CollectionMode.None)
            {
                MessageBox.Show("Select one of the radio button for Collection Target.", "IntelliTrace Collector", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }

            string outputPath = this.txtOutputLocation.Text.Trim();
            if (string.IsNullOrEmpty(outputPath))
            {
                MessageBox.Show("No path defined for Intellitrace output location.", "IntelliTrace Collector", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }

            if (!Directory.Exists(outputPath))
            {
                MessageBoxResult result = MessageBox.Show(string.Format("Output Path {0} does not exists. Do you want to create it?",outputPath), "IntelliTrace Collector", MessageBoxButton.YesNo, MessageBoxImage.Error);
                if (result == MessageBoxResult.No)
                    return false;

                DirectoryInfo info = Directory.CreateDirectory(outputPath);
            }

            if (!Utils.DoesEveryoneHasWritePermissionOnFolder(outputPath))
            {
                Utils.GrantEveryoneWritePermissionToFolder(outputPath);
                AddToLog(string.Format("Everyone granted permissions on {0} folder", outputPath));
            }

            if (collectionMode == CollectionMode.Web)
            {
                if ( this.cbAppPools.SelectedIndex == -1)
                {
                    MessageBox.Show("Select an AppPool name for Intellitrace Data Collection.", "IntelliTrace Collector", MessageBoxButton.OK, MessageBoxImage.Error);
                    return false;
                }
            }
            else if (collectionMode == CollectionMode.Service)
            {
                if ( this.cbService.SelectedIndex == -1)
                {
                    MessageBox.Show("Select a Windows Service for Intellitrace Data Collection.", "IntelliTrace Collector", MessageBoxButton.OK, MessageBoxImage.Error);
                    return false;
                }
                
            }
            //else if (collectionMode == CollectionMode.Executable)
            //{
            //    return true;
            //}
            //return false;

            if (this.cbCollectionPlan.SelectedIndex == -1)
            {
                MessageBox.Show("Select a Collection Plan.", "IntelliTrace Collector", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
            else
                this.collectionPlanNameAndPath = string.Format(@"{0}\{1}\{2}", System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location), Constants.IntelliTraceSoftwareLocation, this.cbCollectionPlan.SelectedValue);

            return true;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            List<string> services = null;
            if (Utils.HasSelectedServicesForCollection())
                services = Utils.GetSelectedServices();
            else
                services = Utils.GetAllServices();
            this.cbService.ItemsSource = services;

            if (Utils.IsIISInstalled())
            {
                List<string> appPools = null;
                if (Utils.HasSelectedAppPoolsForCollection())
                    appPools = Utils.GetSelectedAppPools();
                else
                    appPools = Utils.GetListOfAppPools();

                this.cbAppPools.ItemsSource = appPools;
            }

            string intellitraceCollectorPath = string.Format(@"{0}\{1}", System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location), Constants.IntelliTraceSoftwareLocation);
            List<string> collectionPlanFiles = Directory.GetFiles(intellitraceCollectorPath, "*.xml").Select(x => Path.GetFileName(x)).ToList();
            this.cbCollectionPlan.ItemsSource = collectionPlanFiles;
        }

        private void btnBrowseOutputLocation_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new System.Windows.Forms.FolderBrowserDialog();
            System.Windows.Forms.DialogResult result = dialog.ShowDialog();
            if (result == System.Windows.Forms.DialogResult.OK)
                this.txtOutputLocation.Text = dialog.SelectedPath;
            
        }

        private void btnBrowseExecutablePath_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new System.Windows.Forms.OpenFileDialog();
            dialog.DefaultExt = ".exe";
            dialog.Filter = "Executables (*.exe)|*.exe"; 

            System.Windows.Forms.DialogResult result = dialog.ShowDialog();
            if (result == System.Windows.Forms.DialogResult.OK)
            {
                this.txtExecutablePath.Text = dialog.FileName;
                this.processName = System.IO.Path.GetFileNameWithoutExtension(dialog.FileName);
            }
        }

        private void AddToLog(string message)
        {
            this.logs.Add(new LogData() { EventTime = DateTime.Now, Description = message });
            this.lvLogs.Items.Refresh();
        }

        private void AddToLog(List<string> messages)
        {
            foreach (string message in messages)
                this.logs.Add(new LogData() { EventTime = DateTime.Now, Description = message });
            this.lvLogs.Items.Refresh();
        }

        private void btnEditCollectionPlan_Click(object sender, RoutedEventArgs e)
        {
            if ( this.cbCollectionPlan.SelectedIndex == -1)
            {
                MessageBox.Show("Select a Collection Plan for editing.", "IntelliTrace Collector", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            string intellitraceCollectorPath = string.Format(@"{0}\{1}\{2}", System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location), Constants.IntelliTraceSoftwareLocation, this.cbCollectionPlan.SelectedValue.ToString());
            ProcessStartInfo startInfo = new ProcessStartInfo();
            startInfo.FileName = string.Format(@"{0}\{1}\{2}", System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location), Constants.IntelliTraceConfigLocation, Constants.IntellitraceConfigName);
            startInfo.Arguments = intellitraceCollectorPath;
            Process p = Process.Start(startInfo);
            p.WaitForExit();
        }

        private void cbCollectionPlan_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (this.cbCollectionPlan.SelectedIndex != -1)
                this.btnEditCollectionPlan.IsEnabled = true;
        }
    }
}
