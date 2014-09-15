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
using System.Threading;
using System.Net;
using System.Net.Sockets;
using System.ComponentModel;

namespace AuctionHouseTCSF
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private const int PORT = 5000;
        NetworkStream networkStream;
        StreamReader streamReader;
        StreamWriter streamWriter;
        Socket socket;
        IPEndPoint serverAddress;
        BidClientThreadMethod myBidClientThreadMethod;
        Thread receiveBidThread;
        List<Bidder> myBidder;

        private delegate void bidReceivedDelegate(long bid);
        public MainWindow()
        {
            InitializeComponent();
            serverAddress = new IPEndPoint(IPAddress.Parse("127.0.0.5"), PORT);
            myBidClientThreadMethod = new BidClientThreadMethod();
            myBidClientThreadMethod.ReceivedBidEvent += new BidReceived(BidReceivedHandler);
            myBidder = new List<Bidder>();
            txtBidMaking.IsEnabled = false;
            btnLogIn.IsEnabled = true;
            btnLogOut.IsEnabled = false;
            btnSubmitBid.IsEnabled = false;
            btnLogIn.Focus();
        }

        public void BidReceivedHandler(long bid)
        {

            lblBids.Dispatcher.Invoke(new bidReceivedDelegate(BidReceivedInvoke), bid);
        }
        public void BidReceivedInvoke(long bid)
        {
            lblBids.Content += bid + "\r\n";

        }

        private void btnSubmitBid_Click(object sender, RoutedEventArgs e)
        {
            streamWriter.WriteLine(txtBidMaking.Text);
            streamWriter.Flush();
            txtBidMaking.Text = "";
        }

        private void btnLogIn_Click(object sender, RoutedEventArgs e)
        {
            socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            try
            {
                socket.Connect(serverAddress);
            }
            catch (SocketException ex)
            {
                Console.WriteLine("Can't connect to server");
                Console.WriteLine(ex.ToString());
                return;
            }
            networkStream = new NetworkStream(socket);
            streamReader = new StreamReader(networkStream);
            streamWriter = new StreamWriter(networkStream);

            receiveBidThread = new Thread(new ParameterizedThreadStart(myBidClientThreadMethod.ReceivedBidInfo));
            receiveBidThread.Start(streamReader);

            txtBidMaking.IsEnabled = true;
            btnLogIn.IsEnabled = false;
            btnLogOut.IsEnabled = true;
            btnSubmitBid.IsEnabled = true;
        }

        private void btnLogOut_Click(object sender, RoutedEventArgs e)
        {
            streamReader.Close();
            streamWriter.Close();
            networkStream.Close();
            socket.Shutdown(SocketShutdown.Both);
            socket.Close();

            receiveBidThread.Join();
            lblBids.Content += "\r\nI am leaving the auction";

            txtBidMaking.IsEnabled = false;
            txtBidMaking.Text = "";
            btnLogIn.IsEnabled = true;
            btnLogOut.IsEnabled = false;
            btnSubmitBid.IsEnabled = false;
            btnLogIn.Focus();
        }
    }
}
