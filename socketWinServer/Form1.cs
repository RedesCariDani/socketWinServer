using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Net.Sockets;
using System.Net;
using System.Threading;

namespace socketWinServer
{
    public partial class Form1 : Form
    {
        public static string data = null;
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            byte[] bytes = new Byte[1024];

#pragma warning disable CS0618 // Type or member is obsolete
                              // Establish the local endpoint for the socket.
                              // Dns.GetHostName returns the name of the 
                              // host running the application.
            IPHostEntry ipHostInfo = Dns.Resolve(Dns.GetHostName());           
            IPAddress ipAddress = ipHostInfo.AddressList[0];
            IPEndPoint localEndPoint = new IPEndPoint(ipAddress, int.Parse(puerto.Text));
            //puerto.Text = "Conectándose a: " + localEndPoint.ToString();

            // Create a TCP/IP socket.
            Socket server = new Socket(AddressFamily.InterNetwork,
                SocketType.Stream, ProtocolType.Tcp);

            // Bind the socket to the local endpoint and 
            // listen for incoming connections.
            try
            {
                server.Bind(localEndPoint);
                server.Listen(10);

                // Start listening for connections.
                //while (true)
                //{
                    //textBox1.Text ="Waiting for a connection...";
                    // Program is suspended while waiting for an incoming connection.
                    Socket handler = server.Accept();
                    data = null;
                Console.WriteLine("Client has been accepted!");
                var thread = new Thread(() => ClientSession(handler))
                {
                    IsBackground = true
                };
                thread.Start();

                // An incoming connection needs to be processed.
                //while (true)
                //{
                //    bytes = new byte[1024];
                //    int bytesRec = handler.Receive(bytes);
                //    data += Encoding.ASCII.GetString(bytes, 0, bytesRec);
                //    if (data.IndexOf("<EOF>") > -1)
                //    {
                //        break;
                //    }
                //}

                //// Show the data on the console.
                //

                //// Echo the data back to the client.
                //byte[] msg = Encoding.ASCII.GetBytes(data);

                //handler.Send(msg);
                //handler.Shutdown(SocketShutdown.Both);
                //handler.Close();
                //}

            }
            catch (Exception ev)
            {
                textBox1.Text = ev.ToString();
            }

            Console.WriteLine("\nPress ENTER to continue...");
            Console.Read();
        }

        private void ClientSession(Socket clientSocket)
        {
            Console.WriteLine("\nClient Connected!!\n==================\n CLient IP {0}\n", clientSocket.RemoteEndPoint);
            Byte[] bReceive = new Byte[1024 * 5000];
            int i = clientSocket.Receive(bReceive);
            textBox1.Text = "Recieved...";
            string retorno = "";
            for (int k = 0; k < i; k++)
               retorno += Convert.ToChar(bReceive[k]);
            textBox1.Text = "Text received : {0}" + retorno; ;
            //throw new NotImplementedException();
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }
    }
}
