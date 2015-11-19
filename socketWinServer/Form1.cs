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
using System.IO;

namespace socketWinServer
{
    public partial class Form1 : Form
    {
        public static string data = null;
        string destFolder = @"C:\ReceivedFiles";
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (puerto.Text != "")
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
                this.SetText("Conectándose a: " + localEndPoint.ToString());
                textBox1.Refresh();

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
                    while (true)
                    {
                        this.SetText("Waiting for a connection...");
                        textBox1.Refresh();
                        // Program is suspended while waiting for an incoming connection.
                        Socket handler = server.Accept();
                        data = null;
                        this.SetText("\nClient Connected!!\n==================\n CLient IP\n" + handler.RemoteEndPoint + "\n");
                        textBox1.Refresh();

                        /* CON trheads*/
                        //var thread = new Thread(() => ClientSession(handler))
                        //{
                        //    IsBackground = true
                        //};
                        //thread.Start();





                        /*El que sirve mejor*/

                        //bytes = new byte[1024];
                        //int bytesRec = handler.Receive(bytes);
                        //data += Encoding.ASCII.GetString(bytes, 0, bytesRec);

                        //// Show the data on the console.
                        //textBox3.Text += "Text received :" + data + "\n==================\n";
                        //textBox3.Refresh();

                        //// Echo the data back to the client.
                        //byte[] msg = Encoding.ASCII.GetBytes("Msg recibido por el servidor");

                        //handler.Send(msg);
                        //handler.Shutdown(SocketShutdown.Both);
                        //handler.Close();



                        /*Con archivos*/

                        byte[] clientData = new byte[1024 * 5000];
                       // string receivedPath = "e:/";

                        int receivedBytesLen = handler.Receive(clientData);
                        int fileNameLen = BitConverter.ToInt32(clientData, 0);
                        string fileName = "\\Recibido";
                        Console.WriteLine("Client:{0} connected & File {1} started received.", handler.RemoteEndPoint, fileName);
                        BinaryWriter bWrite = new BinaryWriter(File.Open(destFolder + fileName, FileMode.Append)); ;
                        bWrite.Write(clientData);
                        SetText("File: {0} received & saved at path: {1} " +fileName +" - "+ destFolder);
                        textBox1.Refresh();


                        /*Cierre de conexión*/
                        // Echo the data back to the client.
                        byte[] msg = Encoding.ASCII.GetBytes("Msg recibido por el servidor");

                        handler.Send(msg);
                        handler.Shutdown(SocketShutdown.Both);
                        handler.Close();

                    }

                }
                catch (Exception ev)
                {
                    textBox1.Text = ev.ToString();
                    textBox1.Refresh();
                }
            }
            else textBox1.Text = "Por favor digite primero un número de puerto";
           
        }

        private void ClientSession(Socket clientSocket)
        {
            //this.SetText("\nClient Connected!!\n==================\n CLient IP\n"+ clientSocket.RemoteEndPoint+"\n");
            Byte[] bReceive = new Byte[1024 * 5000];
            int i = clientSocket.Receive(bReceive);
            //this.SetText( "Recieved...");
            string retorno = "";
            for (int k = 0; k < i; k++)
               retorno += Convert.ToChar(bReceive[k]);
            textBox3.Text += "Text received :" + retorno + "\n==================\n";
            textBox3.Refresh();

            // Echo the data back to the client.
            byte[] msg = Encoding.ASCII.GetBytes("Msg recibido por el servidor");

            clientSocket.Send(msg);
            clientSocket.Shutdown(SocketShutdown.Both);
            clientSocket.Close();
        }

        delegate void SetTextCallback(string text);
        private void SetText(string text)
        {
            // InvokeRequired required compares the thread ID of the
            // calling thread to the thread ID of the creating thread.
            // If these threads are different, it returns true.
            if (this.textBox1.InvokeRequired)
            {
                SetTextCallback d = new SetTextCallback(SetText);
                this.Invoke(d, new object[] { text });
            }
            else
            {
                this.textBox1.Text += text+"\n";
            }
        }


        private void Form1_Load(object sender, EventArgs e)
        {
            IPHostEntry ipHostInfo = Dns.Resolve(Dns.GetHostName());
            IPAddress ipAddress = ipHostInfo.AddressList[0];
            textBox2.Text = ipAddress.ToString();
        }
     
    }
}
