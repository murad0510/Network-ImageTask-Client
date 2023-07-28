using Microsoft.Win32;
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
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Network_ImageTask_Client.ViewModels
{
    public class MainViewModel : BaseViewModel
    {
        public RelayCommand SendButtonCommand { get; set; }
        public RelayCommand AddImageButtonCommand { get; set; }

        private BitmapImage image;

        public BitmapImage Image
        {
            get { return image; }
            set { image = value; OnPropertyChanged(); }
        }

        public MainViewModel()
        {

            AddImageButtonCommand = new RelayCommand((_) =>
            {
                File_send(Image);
            });

            SendButtonCommand = new RelayCommand((_) =>
            {
                var ipAdress = IPAddress.Parse("10.2.27.3");
                var port = 27034;

                Task.Run(() =>
                {
                    var ep = new IPEndPoint(ipAdress, port);

                    try
                    {
                        var socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                        socket.Connect(ep);


                        if (socket.Connected)
                        {
                            var sendImage = Image;
                            var bytes = GetJPGFromImageControl(Image);
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

        public void File_send(object parametr)
        {
            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();
            dlg.FileName = "Image";
            dlg.DefaultExt = ".png";

            if (dlg.ShowDialog() == true)
            {
                Image = new BitmapImage(new Uri(dlg.FileName));
                ImageBrush brush = new ImageBrush(Image);
                //IsOkay = true;
            }
        }

        public byte[] GetJPGFromImageControl(BitmapImage imageC)
        {
            MemoryStream memStream = new MemoryStream();
            JpegBitmapEncoder encoder = new JpegBitmapEncoder();
            encoder.Frames.Add(BitmapFrame.Create(imageC));
            encoder.Save(memStream);
            return memStream.ToArray();
        }
    }

}
