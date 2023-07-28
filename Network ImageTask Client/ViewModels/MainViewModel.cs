using Network_ImageTask_Client.Commands;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Imaging;

namespace Network_ImageTask_Client.ViewModels
{
    public class MainViewModel : BaseViewModel
    {
        public RelayCommand SendButtonCommand { get; set; }
        public RelayCommand AddImageButtonCommand { get; set; }

        private string image;

        public string Image
        {
            get { return image; }
            set { image = value; OnPropertyChanged(); }
        }

        public MainViewModel()
        {
            AddImageButtonCommand = new RelayCommand((_) =>
            {
                Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();
                dlg.FileName = "Image"; // Default file name
                dlg.DefaultExt = ".png"; // Default file extension
                dlg.Filter = "Image Files (.jpg)|*.jpg;*.jpeg;*.png;*.gif;*.tif;"; // Filter files by extension

                // Show open file dialog box
                Nullable<bool> result = dlg.ShowDialog();

                // Process open file dialog box results
                if (result == true)
                {
                    // Open document
                    string filename = dlg.FileName;
                    //var name = filename.Split('\\');
                    //for (int i = 0; i < a.Length; i++)
                    //{

                    //}
                    //BitmapImage filefoto = new BitmapImage();
                    //filefoto.UriSource = new Uri(filename);

                    //var buffer = new byte[34000];
                    Image = filename;
                }
            });

            SendButtonCommand = new RelayCommand((_) =>
            {
                Task.Run(() =>
                {
                    var socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

                    var ipAdress = IPAddress.Parse("10.2.29.13");
                    var port = 27014;

                    var ep = new IPEndPoint(ipAdress, port);

                    try
                    {
                        socket.Connect(ep);

                        if (socket.Connected)
                        {
                            var sendImage = Image;
                            var bytes = Encoding.ASCII.GetBytes(sendImage);
                            socket.Send(bytes);

                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"{ex.Message}");
                    }
                });
            });
        }
        public string GetImagePath(byte[] buffer)
        {
            ImageConverter ic = new ImageConverter();
            var data = ic.ConvertFrom(buffer);

            Image img = data as Image;
            if (img != null)
            {
                Bitmap bitmap1 = new Bitmap(img);

                var path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "Images2");
                Directory.CreateDirectory(path);
                var strGuid = Guid.NewGuid().ToString();
                bitmap1.Save($@"{path}\image{strGuid}.png");
                var imagepath = $@"{path}\image{strGuid}.png";
                return imagepath;
            }
            else
            {
                return String.Empty;
            }

        }
    }

}
