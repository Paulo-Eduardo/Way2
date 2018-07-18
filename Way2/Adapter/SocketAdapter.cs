using System.Net.Sockets;

namespace Way2.Adapter
{
    class SocketAdapter : ISocket
    {
        private Socket _socket;

        public SocketAdapter()
        {
            _socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        }

        public void Connect(string host, int port)
        {
            _socket.Connect(host, port);
        }

        public void Close()
        {
            _socket.Close();
        }

        public byte[] Receive(byte[] buffer)
        {
            _socket.Receive(buffer);
            return buffer;
        }

        public int Send(byte[] buffer)
        {
            return _socket.Send(buffer);
        }

    }
}