using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using System.Web;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace XToys_Integration
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        static HttpClient client = new HttpClient();
        string webhookId = "";
        string activeWindow = "";
        public MainWindow()
        {
            InitializeComponent();
            textBox.Text = Settings1.Default.webhookId;
        }

        private void button_Click(object sender, RoutedEventArgs e)
        {
            timer1_Tick(null, null);
            InitTimer();
            startButton.IsEnabled = false;
            stopButton.IsEnabled = true;
        }

        private Timer timer1;
        public void InitTimer()
        {
            timer1 = new Timer();
            timer1.Elapsed += new ElapsedEventHandler(timer1_Tick);
            timer1.Interval = 1000; 
            timer1.Start();
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            string text = GetActiveWindowTitle();
            if(text != activeWindow)
            {
                activeWindow = text;
                sendActiveWindowUpdateAsync(activeWindow);

                this.Dispatcher.Invoke(() =>
                {
                    windowValue.Content = activeWindow;
                });
            }

        }


        [DllImport("user32.dll")]
        static extern IntPtr GetForegroundWindow();

        [DllImport("user32.dll")]
        static extern int GetWindowText(IntPtr hWnd, StringBuilder text, int count);

        private string GetActiveWindowTitle()
        {
            const int nChars = 256;
            StringBuilder Buff = new StringBuilder(nChars);
            IntPtr handle = GetForegroundWindow();

            if (GetWindowText(handle, Buff, nChars) > 0)
            {
                return Buff.ToString();
            }
            return null;
        }

        private void button1_Click(object sender, RoutedEventArgs e)
        {
            timer1.Stop();
            startButton.IsEnabled = true;
            stopButton.IsEnabled = false;
            activeWindow = "";
            windowValue.Content = activeWindow;

        }

        private async Task sendActiveWindowUpdateAsync(string activeWindow)
        {
            string url = "https://xtoys.app/webhook?id=" + HttpUtility.UrlEncode(webhookId) + "&action=activewindow&activewindow=" + HttpUtility.UrlEncode(activeWindow);
            Console.WriteLine(url);
            HttpResponseMessage response = await client.GetAsync(url);        
        }

        private void textBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            webhookId = textBox.Text;
            Settings1.Default.webhookId = webhookId;
            Settings1.Default.Save();
        }
    }
}
