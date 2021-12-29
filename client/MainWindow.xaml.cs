using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.IO;
using System.Net;
using Newtonsoft.Json;

namespace QuickLink
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private readonly string UploadUrl = "https://quicklink.vvzero.xyz/upload";

        public struct SuccessResponse
        {
            public string message;
            public string url;
        }

        public void UploadFile()
        {
            string[] args = Environment.GetCommandLineArgs();
            string filePath;
            try
            {
                filePath = args[1];
            }
            catch
            {
                ShowErrorBox("Don't open this directly.");
                return;
            }

            WebClient fileUploder = new WebClient();
            string downloadUrl = "";

            try
            {
                byte[] responseArray = fileUploder.UploadFile(UploadUrl, filePath);
                SuccessResponse response = JsonConvert.DeserializeObject<SuccessResponse>(System.Text.Encoding.ASCII.GetString(responseArray));
                downloadUrl = response.url;
            }
            catch (WebException e)
            {
                if (e.Status == WebExceptionStatus.ProtocolError)
                {
                    if (((HttpWebResponse)e.Response).StatusCode == HttpStatusCode.Forbidden)
                    {
                        ShowErrorBox("File type not supported.");
                    }
                    else
                    {
                        ShowErrorBox("Wrong operation.");
                    }
                }
                else
                {
                    ShowErrorBox("Network error: " + e.Message);
                }
                return;
            }
            catch (Exception e)
            {
                ShowErrorBox("Unknown error: " + e.Message);
                return;
            }

            try
            {
                Clipboard.SetDataObject(downloadUrl);
            }
            catch (Exception e)
            {
                ShowErrorBox(e.Message);
            }
        }

        private void ShowErrorBox(string msg)
        {
            MessageBox.Show(
                Application.Current.MainWindow,
                msg,
                "Error",
                MessageBoxButton.OK,
                MessageBoxImage.Error);
        }
    }
}
