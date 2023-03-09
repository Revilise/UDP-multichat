using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading;
using System.Windows;

namespace Client.classes
{
    public class NetHandler
    {
        public static NetHandler Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new NetHandler();
                }

                return _instance;
            }
        }
        public int LocalPort { get; set; } // local port for getting message
        public int RemotePort { get; set; } // remote port for sending message

        private NetHandler()
        {
            try
            {
                NameValueCollection config = ConfigurationSettings.AppSettings;
                groupAddress = IPAddress.Parse(config["GroupAddress"]);
                //LocalPort = int.Parse(config["LocalPort"]);
                //RemotePort = int.Parse(config["RemotePort"]);
                ttl = int.Parse(config["TTL"]);
            }
            catch { MessageBox.Show("Error of config file"); }

            _syncContext = SynchronizationContext.Current;
        }

        private bool done = true; // flag of stopping current Thread
        private UdpClient client; // client`s Socket
        private IPAddress groupAddress;

        private int ttl;

        private static IPEndPoint remoteEP;
        private static UnicodeEncoding encoding = new UnicodeEncoding();
        private BinaryFormatter formatter = new BinaryFormatter();
        public string ClientName { get; set; }

        private void Listener(Action<Message> MessageCallback)
        {
            done = false;

            try
            {
                while (!done)
                {
                    IPEndPoint ep = null;
                    byte[] buf = client.Receive(ref ep);

                    Message message;
                    using (MemoryStream ms = new MemoryStream(buf))
                    {
                        message = (Message)formatter.Deserialize(ms);
                    }

                    _syncContext.Post(obj => MessageCallback(message), null);
                }
            } catch (Exception ex)
            {
                if (done) return;
                else
                    MessageBox.Show(ex.Message);
            }
        }

        public void Stop()
        {

            Message temp = new Message
            {
                Name = "System",
                Content = ClientName + " has left from the chat"
            };

            byte[] data;

            using (MemoryStream ms = new MemoryStream())
            {
                formatter.Serialize(ms, temp);
                data = ms.ToArray();
            }

            client.Send(data, data.Length, remoteEP);

            client.DropMulticastGroup(groupAddress);
            client.Close();

            done = true;
        }

        public void Start(string name, Action<Message> MessageCallback)
        {
            ClientName = name;

            client = new UdpClient(LocalPort);
            client.JoinMulticastGroup(groupAddress, ttl);

            remoteEP = new IPEndPoint(groupAddress, RemotePort);

            Thread reciever = new Thread(
                new ThreadStart(() => Listener(MessageCallback))
            );

            reciever.IsBackground = true;
            reciever.Start();

            Message temp = new Message
            {
                Name = "System",
                Content = ClientName + " has joined the chat"
            };

            byte[] data;
            using (MemoryStream ms = new MemoryStream())
            {
                formatter.Serialize(ms, temp);
                data = ms.ToArray();
            }

            client.Send(data, data.Length, remoteEP);
        }

        public void Send(string message)
        {
            try
            {
                byte[] data;

                Message temp = new Message
                {
                    Name = ClientName,
                    Content = message
                };

                using (MemoryStream ms = new MemoryStream())
                {
                    formatter.Serialize(ms, temp);
                    data = ms.ToArray();
                }

                client.Send(data, data.Length, remoteEP);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private readonly SynchronizationContext _syncContext;

        private static NetHandler _instance;
    }
}
