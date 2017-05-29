using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;
using System.Net;
using System.IO;
using System.Threading;

namespace ConsoleSocket_Test
{
    class Program
    {
        public static string localIP = "192.168.243.1";
        public static int localPort = 86;

        static Socket server = null;
        static Socket clientSocket = null;

        static List<Socket> clientList = new List<Socket>();

        static byte[] SendBytes = new byte[256];
        static byte[] RecBytes = new byte[256];
        static byte[] TempBytes = new byte[256];
        static string ClientMSG = null;

        const string fileErrMSG = "Can not read file data now.";
        const string OKMSG = "Server is OK";
        static bool flag = false;
        static int clientCount = 0;

        private static void ListenClientConnect()
        {
            
            while (true)
            {
                lock (clientList)
                {
                    
                    clientSocket = server.Accept();
                    clientSocket.Send(Encoding.ASCII.GetBytes(OKMSG));
                    clientList.Add(clientSocket);
                    
                    if (clientSocket.Poll(-1, SelectMode.SelectRead))
                    {
                        if (!clientSocket.Connected)

                            clientList.RemoveAt(clientList.Count - 1);
                    }


                }
            }
        }

        
        private static void ResponseClient()
        {
            while(true)
            {
                if (clientCount != clientList.Count)
                {
                    flag = true;
                    clientCount = clientList.Count;
                }
                else
                {
                    flag = false;
                }

                if (flag)
                {
                    if (clientList.Any())
                    {

                        try
                        {

                            foreach (Socket item in clientList)
                            {

                                if (item.Poll(10, SelectMode.SelectRead | SelectMode.SelectWrite))
                                {
                                    if (item.Available > 0)
                                    {
                                        item.Receive(RecBytes, RecBytes.Length, SocketFlags.None);
                                        ClientMSG = Encoding.ASCII.GetString(RecBytes);

                                        Console.WriteLine("Message from client:");
                                        Console.WriteLine(ClientMSG);
                                    }
                                    else if (!item.Connected)
                                    {
                                        
                                        continue;
                                    }
                                    else
                                    {
                                        Console.WriteLine("The client from " + item.RemoteEndPoint + "connected this server");

                                        string fileData = File.ReadAllText(@"C:\Users\lz_home\Desktop\Test.txt", Encoding.ASCII);
                                        //int data = Convert.ToInt32(fileData);

                                        if (fileData == null)
                                        {
                                            Console.WriteLine("No Data in file now...");
                                            SendBytes = Encoding.ASCII.GetBytes(fileErrMSG);
                                            item.Send(SendBytes, SendBytes.Length, SocketFlags.None);

                                        }
                                        else
                                        {
                                            Console.WriteLine("The data in file is:");
                                            Console.WriteLine(fileData);

                                            SendBytes = Encoding.ASCII.GetBytes(fileData);

                                            if (item.Connected)
                                                item.Send(SendBytes, SendBytes.Length, SocketFlags.None);

                                        }
                                    }
                                }

                            }
                        }
                        catch (InvalidOperationException InOE)
                        {
                            //Console.WriteLine(InOE.Message);
                            continue;
                        }
                        catch(SocketException SocketE)
                        {
                            Console.WriteLine(SocketE.Message);
                            continue;
                        }

                    }
                    else
                    {
                        Console.WriteLine("No client connecting now...");
                    }
                }
            }
        }

        static void Main(string[] args)
        {
            
            try
            {
                
                IPAddress localAddr = IPAddress.Parse(localIP);
                IPEndPoint localEndPoint = new IPEndPoint(localAddr, localPort);

                server = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

                server.Bind(localEndPoint);
                server.Listen(10);

                Console.WriteLine("Start listening...");

                Thread ListenThread = new Thread(ListenClientConnect);
                Thread ResponseThread = new Thread(ResponseClient);
                ListenThread.Start();
                ResponseThread.Start();
                
            }
            catch (IOException e)
            {
                Console.WriteLine("Please ensure the file existing...");

                Console.WriteLine("Source : " + e.Source);
                Console.WriteLine("Message : " + e.Message);
            }
            catch (SocketException e)
            {
                Console.WriteLine("SocketException caught!!!");
                Console.WriteLine("Source : " + e.Source);
                Console.WriteLine("Message : " + e.Message);
            }
            catch (ArgumentNullException e)
            {
                Console.WriteLine("ArgumentNullException caught!!!");
                Console.WriteLine("Source : " + e.Source);
                Console.WriteLine("Message : " + e.Message);
            }
            catch (NullReferenceException e)
            {
                Console.WriteLine("NullReferenceException caught!!!");
                Console.WriteLine("Source : " + e.Source);
                Console.WriteLine("Message : " + e.Message);
            }
            catch (Exception e)
            {
                Console.WriteLine("Exception caught!!!");
                Console.WriteLine("Source : " + e.Source);
                Console.WriteLine("Message : " + e.Message);
            }


        }

      
    }
}
