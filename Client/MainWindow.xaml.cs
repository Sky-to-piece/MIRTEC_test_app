using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
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

namespace Client
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window{
        const string ip = "127.0.0.1";
        const int port = 8080;
        public MainWindow(){
            InitializeComponent();
        }

        void RBAll_Checked(object sender, RoutedEventArgs e){
            TBRecordNum.IsEnabled = false;
        }

        void RBOne_Checked(object sender, RoutedEventArgs e){
            TBRecordNum.IsEnabled = true;
        }

        void BTRequest_Click(object sender, RoutedEventArgs e){

            if ((bool)(RBAll.IsChecked == true)) {
                ServerConnection();
                MessageBox.Show("Показаны все результаты", null, MessageBoxButton.OK, MessageBoxImage.Error);
            } else if ((bool)(RBOne.IsChecked == true && TBRecordNum.Text.Length > 0)) {
                ServerConnection();
                MessageBox.Show($"Показаны результаты по номеру записи {TBRecordNum.Text}", null, MessageBoxButton.OK, MessageBoxImage.Error);
            } else {
                MessageBox.Show("вы ничего не выбрали", null, MessageBoxButton.OK, MessageBoxImage.Error);
            }
            
        }

        void BTRequest_KeyDown(object sender, KeyEventArgs e){
            if (e.Key == Key.Enter){
                BTRequest_Click(BTRequest, null);
            }
        }

        void TBRecordNum_PreviewTextInput(object sender, TextCompositionEventArgs e){
            if (!Char.IsDigit(e.Text, 0)){
                e.Handled = true;
            }
        }

        void ServerConnection(){
            try{
                IPEndPoint tcpEndPoint = new IPEndPoint(IPAddress.Parse(ip), port);

                Socket tcpSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

                string message = TBRecordNum.Text;

                byte[] data = Encoding.UTF8.GetBytes(message);

                tcpSocket.Connect(tcpEndPoint);
                tcpSocket.Send(data);

                byte[] buffer = new byte[256];
                int size = 0;
                StringBuilder answer = new StringBuilder();

                do{
                    size = tcpSocket.Receive(buffer);
                    answer.Append(Encoding.UTF8.GetString(buffer, 0, size));
                }
                while (tcpSocket.Available > 0);

                TBLConclusion.Text = (answer.ToString());

                tcpSocket.Shutdown(SocketShutdown.Both);
                tcpSocket.Close();
            }
            catch (Exception Exceptions){
                MessageBox.Show($"{Exceptions.ToString()}", null, MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

    }
}
