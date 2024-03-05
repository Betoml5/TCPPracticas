// See https://aka.ms/new-console-template for more information


using System.Net;
using System.Net.Sockets;
using System.Text;
//iniciamos un servidor TCP
TcpListener server = new(IPAddress.Any, 8000);
server.Start();
while (true)
{
    TcpClient client = server.AcceptTcpClient();
    Console.WriteLine("Cliente aceptado" + client.Client.RemoteEndPoint?.ToString() + " en el puerto: " +  "Nose");
    Thread HiloAtendedor = new Thread(() =>
    {
        AtenderCliente(client);
    });

    HiloAtendedor.IsBackground = true;
    HiloAtendedor.Start();
}

void AtenderCliente(TcpClient client)
{


    while (true)
    {
        if (client.Available > 0)
        {
            var ns = client.GetStream();
            byte[] buffer = new byte[client.Available];
            ns.Read(buffer, 0, buffer.Length);

            Console.WriteLine("Ip: " + client.Client.RemoteEndPoint?.ToString() + ": " + Encoding.UTF8.GetString(buffer));

        }
    }
}

