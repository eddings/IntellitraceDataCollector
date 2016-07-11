using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
//using System.Windows.Forms;
using Microsoft.Win32; 

namespace MwNetworkOptimizer
{
    /// <summary>
    /// Interaction logic for ucStatusBar.xaml
    /// </summary>
    /// 
    public enum StatusBarMessageType
    {
        Normal,
        Success,
        Error
    }

    public partial class UCStatusBar : UserControl, IStatusBarView
    {
        public UCStatusBar()
        {
            InitializeComponent();
        }

        public void ShowMessage(StatusBarMessageType messageType, string message)
        {
            Reset();
            this.tbMessage.Text = message;
            if ( messageType == StatusBarMessageType.Normal )
                this.tbMessage.Foreground = new SolidColorBrush(Colors.Black);
            if (messageType == StatusBarMessageType.Success)
                this.tbMessage.Foreground = new SolidColorBrush(Colors.Green);
            if (messageType == StatusBarMessageType.Error)
                this.tbMessage.Foreground = new SolidColorBrush(Colors.Red);
        }



        public void Reset()
        {
            this.tbMessage.Text = string.Empty;
            this.progressBar.Visibility = Visibility.Hidden;
            this.tbProgress.Visibility = Visibility.Hidden;
        }

        public void ShowProgressBar(double value, double maximum)
        {
            if (this.progressBar.Visibility != System.Windows.Visibility.Visible)
                this.progressBar.Visibility = System.Windows.Visibility.Visible;

            if (this.progressBar.Maximum != maximum)
                this.progressBar.Maximum = maximum;

            this.progressBar.Value = value;
            if (this.tbProgress.Visibility != System.Windows.Visibility.Visible)
                this.tbProgress.Visibility = System.Windows.Visibility.Visible;
            this.tbProgress.Text = string.Format("({0} of {1})", value, maximum);
        }

        public void ShowCancellationNotice(string message)
        {
            Reset();
            ShowMessage(StatusBarMessageType.Normal, message);
            this.progressBar.Visibility = System.Windows.Visibility.Visible;
            this.progressBar.IsIndeterminate = true;

        }
    }

    public interface IStatusBarView
    {
        void Reset();
        void ShowMessage(StatusBarMessageType messageType, string message);
        void ShowProgressBar(double value, double maximum);
        void ShowCancellationNotice(string message);
    }

}
