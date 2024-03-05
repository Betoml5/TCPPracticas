using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows;
using TCPWPFCliente.Models.DTOs;

namespace TCPWPFCliente.Services
{
    public class ChatClient
    {
        TcpClient cliente;
        public string equipo = null!;

       public event EventHandler<MensajeDTO>? MensajeRecibido;

        public void Conectar(IPAddress ip)
        {
            try
            {
                IPEndPoint ipe = new IPEndPoint(ip, 9000);
                cliente = new();
                cliente.Connect(ipe);

                var equipo = Dns.GetHostName();
                var msg = new MensajeDTO
                {
                    Fecha = DateTime.Now, Mensaje = "**HELLO",
                    Origen = equipo
                };

                EnviarMensaje(msg);
                RecibirMensaje();
            }
            catch (Exception)
            {

                throw;
            }
        }

        public void Desconectar()
        {
            var msg = new MensajeDTO
            {
                Fecha = DateTime.Now,
                Mensaje = "**BYE",
                Origen = equipo
            };

            EnviarMensaje(msg);
            cliente.Close();
        }

        private void RecibirMensaje()
        {
            new Thread(() =>
            {
                try
                {
                    while (cliente.Connected)
                    {
                        if (cliente.Available > 0)
                        {
                            var ns = cliente.GetStream();
                            byte[] bytes = new byte[cliente.Available];
                            ns.Read(bytes, 0, bytes.Length);
                            var json = Encoding.UTF8.GetString(bytes);
                            var mensaje = JsonSerializer.Deserialize<MensajeDTO>(json);

                            if (mensaje != null)
                            {
                                
                                Application.Current.Dispatcher.Invoke(() =>
                                {
                                    MensajeRecibido?.Invoke(this, mensaje);
                                }); 
                            }
                        }
                    }
                }
                catch (Exception)
                {

                    throw;
                }
              
            })
            {
                IsBackground = true
            }.Start();
        }


        public void EnviarMensaje(MensajeDTO mensaje)
        {
            if (!string.IsNullOrWhiteSpace(mensaje.Mensaje))
            {
                var json = JsonSerializer.Serialize(mensaje);
                byte[] bytes = Encoding.UTF8.GetBytes(json);

                var ns = cliente.GetStream();
                ns.Write(bytes, 0, bytes.Length);
                ns.Flush();
            }
        }
        

    }
}
