using HueApi.Entertainment.Models;
using Org.BouncyCastle.Crypto.Tls;
using System.Net.Sockets;
using System.Text;

namespace HueApi.Entertainment.Connection
{
  /// <summary>
  /// Based on https://github.com/jinghongbo/Ssl.Net/tree/master/src/Ssl.Net/Ssl.Net
  /// </summary>
  internal class UdpTransport : DatagramTransport
  {
    private Socket _socket;

    public UdpTransport(Socket socket)
    {
      _socket = socket;
    }

    public void Close()
    {

    }

    public int GetReceiveLimit()
    {
      return 1024 * 4;
    }

    public int GetSendLimit()
    {
      return 1024 * 4;
    }

    public int Receive(byte[] buf, int off, int len, int waitMillis)
    {
      string converted = Encoding.UTF8.GetString(buf, 0, buf.Length);

      if (_socket.Connected)
      {
        if (waitMillis == 0 && _socket.Available == 0)
        {
          return -1;
        }

        if (SpinWait.SpinUntil(() => _socket.Available > 0, waitMillis))
        {
          return _socket.Receive(buf, off, len, SocketFlags.None);
        }
        else
        {
          if (waitMillis == 60000) // 1 min
          {
            throw new TimeoutException();
          }

          return -1;
        }

      }

      throw new HueEntertainmentException("Receiving data but socket not connected");
    }

    public void Send(byte[] buf, int off, int len)
    {
      string converted = Encoding.UTF8.GetString(buf, 0, buf.Length);

      if (_socket.Connected)
      {
        _socket.Send(buf, off, len, SocketFlags.None);
      }
      else
      {
        throw new HueEntertainmentException("Sending data but socket is not connected");
      }
    }
  }
}
