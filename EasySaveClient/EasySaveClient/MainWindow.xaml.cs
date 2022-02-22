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


using System.Net;
using System.Net.Sockets;
using System.Threading;


namespace EasySaveClient
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        Socket Socket = null;
        Thread t;

        public MainWindow()
        {
            InitializeComponent();
            this.InformationLabel.Content = "Connecting to server...";
            this.t = new Thread(new ThreadStart(this.Run));
            t.Start();
        }

        public void Notify(string txt)
        {
            this.InformationLabel.Content = txt;

        }

        public void Run()
        {
             this.Socket = this.SeConnecter();
            this.Dispatcher.BeginInvoke(
                System.Windows.Threading.DispatcherPriority.Normal, 
                (ThreadStart)delegate { this.Notify("Connect to " + this.Socket.AddressFamily.ToString()); });
            this.Listen(this.Socket);

        }

        public Socket SeConnecter()
        {
            IPHostEntry ipHostInfo = Dns.GetHostEntry(Dns.GetHostName());
            IPAddress ipAddress = ipHostInfo.AddressList[0];
            IPEndPoint remoteEP = new IPEndPoint(ipAddress, 11000);

            // Create a TCP/IP  socket.  
            Socket sender = new Socket(ipAddress.AddressFamily,
                SocketType.Stream, ProtocolType.Tcp);

            while (true)
            {
                try
                {
                    sender.Connect(remoteEP);
                    break;
                }
                catch (Exception ex)
                {

                }
            }
            
            return sender;
        }

        private void Listen(Socket client)
        {
            byte[] bytes = new Byte[1024];
            while (true)
            {
                try
                {
                    string data = null;
                    int bytesRec = client.Receive(bytes);
                    data += Encoding.UTF8.GetString(bytes, 0, bytesRec);

                        this.Dispatcher.BeginInvoke(
                    System.Windows.Threading.DispatcherPriority.Normal,
                    (ThreadStart)delegate { this.Update(data); });

                }
                catch (Exception ex)
                {

                }
            }

        }

        private void Update(string data)
        {
            

            string name = data.Split(':')[0];
            name = name.Trim();
            string value = data.Split(':')[1];
            value = value.Trim();


            if(name == "value")
            {

            if(Int32.TryParse(value, out int output))
            {
                this.ProgressBar.Value = output;
                this.PercentLabel.Content = output.ToString() + "%";
            }
            }else if (name == "name")
            {
                this.BackupNameLabel.Content = value;
            }
        }

        private static void Deconnecter(Socket socket)
        {
            socket.Close();
        }

        private void PlayButton_Click(object sender, RoutedEventArgs e)
        {
            if(this.Socket != null)
            {
                string msg = "action : play";
                byte[] bmsg = Encoding.UTF8.GetBytes(msg);
                try
                {
                    this.Socket.Send(bmsg);
                }catch (Exception ex)
                {

                }
            }
        }private void PauseButton_Click(object sender, RoutedEventArgs e)
        {
            if(this.Socket != null)
            {
                string msg = "action : pause";
                byte[] bmsg = Encoding.UTF8.GetBytes(msg);
                try
                {
                    this.Socket.Send(bmsg);
                }catch (Exception ex)
                {

                }
            }
        }
        private void StopButton_Click(object sender, RoutedEventArgs e)
        {
            if(this.Socket != null)
            {
                string msg = "action : stop";
                byte[] bmsg = Encoding.UTF8.GetBytes(msg);
                try
                {
                    this.Socket.Send(bmsg);
                this.InformationLabel.Content = msg;
                }catch (Exception ex)
                {

                }
            }
        }
    }
}
