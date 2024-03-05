

using System.Net;
using System.Net.Sockets;
using System.Text;

TcpClient client = new();

Console.WriteLine("Escribe la IP del servidor");
var ip  = Console.ReadLine() ?? "127.0.0.1";
var ipe = new IPEndPoint(IPAddress.Parse(ip), 8000);
client.Connect(ipe);

string cadena;

while ( (cadena = Console.ReadLine()) != null)
{
    byte[] buffer = Encoding.UTF8.GetBytes(cadena);
    var ns = client.GetStream();
    ns.Write(buffer, 0, buffer.Length);
    ns.Close();
}
