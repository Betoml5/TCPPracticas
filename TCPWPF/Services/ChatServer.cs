using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows;
using TCPWPF.Models;

namespace TCPWPF.Services
{
    public class ChatServer
    {
        TcpListener server = null!;

        List<TcpClient> clients = new();


        public void Iniciar()
        {
            server = new (new IPEndPoint(IPAddress.Any, 9000));
            server.Start();

            new Thread(Escuchar)
            {
                IsBackground = true
            }.Start();
        }

        public void Detener()
        {
            if (server != null)
            {
                server.Stop();
                foreach (var c in clients)
                {
                    c.Close();
                }
                clients.Clear();
            }
        }

        void Escuchar()
        {
            while (server.Server.IsBound)
            {
                var tcpClient = server.AcceptTcpClient();
                clients.Add(tcpClient);

                Thread t = new Thread(() =>
                {
                      RecibirMensaje(tcpClient);
                });
                t.IsBackground = true;
                t.Start();
            }
        }

        public event EventHandler<MensajeDTO>? MensajeRecibido;
        void RecibirMensaje(TcpClient client)
        {
            while (client.Connected)
            {
                var ns = client.GetStream();
                while (client.Available == 0)
                {
                    Thread.Sleep(500);
                }

                byte[] buffer = new byte[client.Available];
                ns.Read(buffer, 0, buffer.Length);

                var json = Encoding.UTF8.GetString(buffer);
                var mensaje = JsonSerializer.Deserialize<MensajeDTO>(json);
                    RelayMessage(client, buffer);

                if (mensaje != null)
                {
                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        MensajeRecibido?.Invoke(this, mensaje);
                    });
                }
            }

            clients.Remove(client);
        }   

        void RelayMessage(TcpClient cliente, byte[] mensaje)
        {

            foreach (var client in clients)
            {
                if (client != cliente && client.Connected)
                {
                    var ns = client.GetStream();
                    ns.Write(mensaje, 0, mensaje.Length);
                    ns.Flush();
                }

            }
        }

    }
}
