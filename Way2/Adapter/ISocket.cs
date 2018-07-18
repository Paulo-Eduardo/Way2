using System.Net.Sockets;

namespace Way2.Adapter
{
    public interface ISocket
    {
        void Connect(string host, int port);
        void Close();
        byte[] Receive(byte[] buffer);
        int Send(byte[] buffer);
    }
}
