using GalaSoft.MvvmLight.Command;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Threading;
using TCPWPF.Models;
using TCPWPF.Services;

namespace TCPWPF.ViewModels
{
    internal class ChatViewModel : INotifyPropertyChanged
    {

        public ChatServer Server { get; set; } = new();
        public ObservableCollection<string> Usuarios { get; set; } = new();
        public ObservableCollection<MensajeDTO> Mensajes  { get; set; } = new();
        public string IP { get; set; } = "0.0.0.0";

        public ICommand IniciarCommand { get; set; }
        public ICommand DetenerCommand { get; set; }


        public ChatViewModel()
        {
            var direcciones = Dns.GetHostAddresses(Dns.GetHostName());
            if (direcciones != null)
            {
                IP = string.Join(",", direcciones
                    .Where(x => x.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
                    .Select(x => x.ToString()));

            }

            IniciarCommand = new RelayCommand(Iniciar);
            DetenerCommand = new RelayCommand(Detener);

            Server.MensajeRecibido += Server_MensajeRecibido;
        }

        private void Server_MensajeRecibido(object? sender, MensajeDTO e)
        {

                if (e.Mensaje == "**HELLO")
                {
                    e.Mensaje = $"{e.Origen} se ha conectado";
                    Usuarios.Add(e.Origen);
                }
                else if (e.Mensaje == "**BYE")
                {
                    e.Mensaje = $"{e.Origen} se ha desconectado";
                    Usuarios.Remove(e.Origen);
                }

                Mensajes.Add(e);
         
        }


        private void Detener()
        {
            Mensajes.Clear();
            Usuarios.Clear();
            Server.Detener();

          
        }


        private void Iniciar()
        {
            Server.Iniciar();
        }


        public event PropertyChangedEventHandler? PropertyChanged;
    }
}
