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
using System.Windows.Threading;

namespace Socket_4I
{
    /// <summary>
    /// Logica di interazione per MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        Socket socket = null;
        DispatcherTimer dTimer = null;
        
        //Il codice nel main permette l'inizializzazione del socket
        public MainWindow()
        {
            InitializeComponent();
             //inizializzo la socket
            socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);

            //inizializzo un indirizzo ip locale qualunque associato alla porta 11000
            IPEndPoint local_endpoint = new IPEndPoint(IPAddress.Any, 10000);

            //associo la socket all'endpoint locale inizializzato sopra
            socket.Bind(local_endpoint);

            //indico che la socket NON è in modalità di blocco
            socket.Blocking = false;
            //indico che la socket può inviare o ricevere pacchetti broadcast
            socket.EnableBroadcast = true;

            //inizializzo il DispatcherTimer
            dTimer = new DispatcherTimer();
            //quando trascorre l'intervallo del timer, eseguo il metodo aggiornamento_dTimer
            dTimer.Tick += new EventHandler(aggiornamento_dTimer);
            //inizializzo l'intervallo del timer a 250 millisecondi
            dTimer.Interval = new TimeSpan(0, 0, 0, 0, 250);
            //avvio il dTimer
            dTimer.Start();

        }

        //il codice sottostante invece gestisce la ricezione dei messaggi
        private void aggiornamento_dTimer(object sender, EventArgs e)
        {
            int nBytes = 0;

            if ((nBytes = socket.Available) > 0)
            {
                //ricezione dei caratteri in attesa
                byte[] buffer = new byte[nBytes];

                //identifico l'indirizzo di rete (casuale e utilizza la porta 0)
                EndPoint remoteEndPoint = new IPEndPoint(IPAddress.Any, 0);

                //individua il numero di byte che occupa il messaggio
                nBytes = socket.ReceiveFrom(buffer, ref remoteEndPoint);

                //recupera il mittente del messaggio
                string from = ((IPEndPoint)remoteEndPoint).Address.ToString();

                //recupera il messaggio ricevuto, traducendo il buffer
                string messaggio = Encoding.UTF8.GetString(buffer, 0, nBytes);

                //aggiorna la lista di messaggio con il mittente e il messaggio
                lstMessaggi.Items.Add(from+": "+messaggio);

            }
        }

        //il codice sottostante gestisce invece l'invio dei messaggi
        private void btnInvia_Click(object sender, RoutedEventArgs e)
        {

            //all'indirizzo ip associo il numero di porta da utilizzare
            IPEndPoint remote_endpoint = new IPEndPoint(IPAddress.Broadcast, 11000);

            //trasformo il messaggio di txtMessaggio.Text in un array di Byte
            byte[] messaggio = Encoding.UTF8.GetBytes(txtMessaggio.Text);

            //invio il mio array di byte (messaggio) all'indirizzo ip con la rispettiva porta da utilizzare (remote_endpoint)
            socket.SendTo(messaggio, remote_endpoint);

            //pulisco la textbox del messaggio per scriverne uno nuovo
            txtMessaggio.Text = "";
        }
    }
}
