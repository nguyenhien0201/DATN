using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;
using System.Net;
using System.Threading;

namespace MQTT
{
    class AsyncEvent : Dictionary<Action<IAsyncResult>, ManualResetEvent>
    {
        public AsyncEvent()
        {
        }
        new public ManualResetEvent this[Action<IAsyncResult> index]
        {
            get
            {
                ManualResetEvent e;
                if (TryGetValue(index, out e) == false)
                {
                    base.Add(index, e = new ManualResetEvent(false));
                }
                return e;
            }
        }
        public AsyncCallback Get(Action<IAsyncResult> index)
        {
            return new AsyncCallback(index);
        }
    }

    public class Client
    {
        #region SERVER
        Socket _socket;
        string _host { get; set; }
        public string Host => _host;
        int _port { get; set; } = 1883;
        public int Port => _port;

        IPEndPoint _remoteEP;
        bool _socket_connected;
        public bool Connected => _socket_connected;
        #endregion

        #region CLIENT
        string _id;
        //public string ID => _id;
        string _userName;
        // public string UserName => _userName;
        string _password;
        //public string Password => _password;

        private AsyncEvent _async = new AsyncEvent();
        #endregion

        #region CONNECTION
        protected virtual void RaiseConnectionChanged()
        {
            if (_socket_connected != _socket.Connected)
            {
                _socket_connected = _socket.Connected;
                ConnectionChanged?.Invoke(this);
            }
        }
        private void on_connected(IAsyncResult ar)
        {
            try
            {
                _socket.EndConnect(ar);
                _mqtt_connect();

                RaiseConnectionChanged();
            }
            catch (Exception e)
            {
            }
        }
        private void on_disconnect(IAsyncResult ar)
        {
            _socket.EndDisconnect(ar);
            RaiseConnectionChanged();
        }
        private void _mqtt_connect()
        {
            Send(Packet.Connect(_id, _userName, _password, 60));
            create_threading();
        }
        private void _mqtt_disconnect()
        {
            var p = Packet.Disconnect()[0];
            _socket.Send(p, p.Length, 0);
        }
        /// <summary>
        /// connect the socket
        /// </summary>
        public bool AutoReconnect { get; set; }
        public void Connect(string username, string password)
        {
            _userName = username;
            _password = password;
            Connect();
        }
        public void Connect()
        {
            IPAddress ip = null;
            try
            {
                ip = IPAddress.Parse(_host);
            }
            catch
            {
                ip = Dns
                    .GetHostEntry(_host == null ? Dns.GetHostName() : _host)
                    .AddressList[0];
            }

            _remoteEP = new IPEndPoint(ip, _port);
            Reconnect();
        }
        public void Reconnect()
        {
            if (_socket_connected) { return; }

            _busy = true;
            _socket = new Socket(_remoteEP.AddressFamily,
                SocketType.Stream, ProtocolType.Tcp);

            _socket.BeginConnect(_remoteEP,
                _async.Get(on_connected), this);

            int count = 5;
            var ts = new ThreadStart(() =>
            {

                while (_socket.Connected == false && count-- > 0)
                {
                    //Console.Write('.');
                    Thread.Sleep(200);
                }
            });
            new Thread(ts).Start();
        }
        public void Disconnect()
        {
            if (_socket_connected)
            {
                _busy = true;
                _mqtt_disconnect();
                _socket.BeginDisconnect(true, new AsyncCallback(on_disconnect), null);
            }
        }
        #endregion

        #region EVENTS
        public event Action<Client> ConnectionChanged;
        public event Action<string, byte[]> DataReceived;
        public event Action<string, string> TextReceived;

        protected virtual void OnResponseReceived(byte code, int length, byte[] buffer)
        {
            if (code == 0x30)
            {
                int len = (buffer[0] << 8) | buffer[1];
                string topic = Encoding.UTF8.GetString(buffer, 2, len);

                int i = len + 2;
                byte[] data = new byte[buffer.Length - i];
                for (int k = 0; k < data.Length; k++)
                {
                    data[k] = buffer[i++];
                }
                DataReceived?.Invoke(topic, buffer);

                if (TextReceived != null)
                {
                    TextReceived.Invoke(topic, Encoding.UTF8.GetString(data));
                }
            }
        }
        #endregion

        #region RECEIVE
        Thread _listerning;
        bool _busy;
        byte[] one_byte = new byte[1];
        byte read_byte()
        {
            _socket.Receive(one_byte, 1, 0);
            return one_byte[0];
        }

        void create_threading()
        {
            if ((_listerning != null && _listerning.IsAlive)) return;

            var ts = new ThreadStart(() =>
            {
                while (true)
                {
                    if (_socket.Connected != _socket_connected)
                    {
                        RaiseConnectionChanged();
                    }
                    if (_socket.Connected == false)
                    {
                        if (!AutoReconnect)
                        {
                            return;
                        }

                        Thread.Sleep(1000);

                        Reconnect();
                        continue;
                    }

                    if (_busy) continue; // Đang gửi lên server

                    try
                    {
                        byte code = read_byte();
                        if (code != 0)
                        {
                            var len = new RemainingLength();
                            while (len.Read(read_byte())) ;

                            int size = len.GetValue();
                            byte[] remainingData = new byte[size];
                            for (int i = 0; i < size; i++)
                            {
                                remainingData[i] = read_byte();
                            }

                            OnResponseReceived(code, size, remainingData);
                        }
                    }
                    catch
                    {
                    }
                }
            });
            _listerning = new Thread(ts);
            _listerning.Start();
        }
        public void Subscribe(string topic, byte qos)
        {
            Send(Packet.Subscribe(topic, qos));
        }
        public void Subscribe(string topic)
        {
            Subscribe(topic, 0);
        }
        #endregion

        #region SEND
        private void _debug(Packet packet, bool endl)
        {
            StringBuilder builder = new StringBuilder();
            foreach (byte[] e in packet)
            {
                foreach (var b in e)
                {
                    builder.AppendFormat("{0:X2} ", b);
                }
            }

            if (endl) builder.Append('\n');
        }
        private void Send(Packet packet)
        {
            //if (!_socket_connected) Reconnect(); //
            var ts = new ThreadStart(() =>
            {
                _busy = true;
                _socket.Send(packet.ToBytes()); //some errors occurs in here
                _busy = false;
            });
            new Thread(ts).Start();
        }
        #endregion

        #region CONSTRUCTORS
        public Client(string id, string host, int port)
        {
            _id = id;
            _host = host;
            _port = port;
        }
        public Client(string id, string host, int port, string username, string password)
            : this(id, host, port)
        {
            _userName = username;
            _password = password;
        }
        public Client(string host, int port)
            : this(Guid.NewGuid().ToString(), host, port)
        {
        }
        public Client(string host) : this(host, 1883)
        {
        }
        #endregion

        #region PUBLISH
        public void Publish(string topic, byte[] message, byte qos, bool retain)
        {
            //if (!Connected) { Reconnect(); }
            if (Connected)
            {
                Send(Packet.Publish(topic, message, qos, retain));
            }
        }
        public void Publish(string topic, string message, byte qos, bool retain)
        {
            if (Connected)
            {
                Publish(topic, Encoding.UTF8.GetBytes(message), qos, retain);
            }

        }
        public void Publish(string topic, byte[] message, byte qos)
        {
            Publish(topic, message, qos, false);
        }
        public void Publish(string topic, string message, byte qos)
        {
            Publish(topic, message, qos, false);
        }
        public void Publish(string topic, byte[] message)
        {
            Publish(topic, message, 0, false);
        }
        public void Publish(string topic, string message)
        {
            Publish(topic, message, 0, false);
        }
        #endregion
    }
}
