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
using TCPWPFCliente.Models.DTOs;
using TCPWPFCliente.Services;

namespace TCPWPFCliente.ViewModels
{
    public class ChatViewModel : INotifyPropertyChanged
    {

        public ObservableCollection<MensajeDTO> Mensajes { get; set; } = new();
        public string Mensaje { get; set; } = string.Empty;
        public ICommand EnviarCommand { get; set; }
        public string IP { get; set; } = "";
        public ICommand ConnectarCommand { get; set; }
        public bool Conectado { get; set; }
        public int NumeroMensaje { get; set; }

        ChatClient chatClient = new();

        public ChatViewModel()
        {
            chatClient.MensajeRecibido += ChatClient_MensajeRecibido; 
            EnviarCommand = new RelayCommand(EnviarMensaje);
            ConnectarCommand = new RelayCommand(Conectar);
        }

        private void Conectar()
        {
            IPAddress.TryParse(IP, out var ip);
            if (ip != null)
            {
                chatClient.Conectar(ip);
                Conectado = true;
                PropertyChanged?.Invoke(this, new(nameof(Conectado)));
            }
        }

        private void EnviarMensaje()
        {
            if (string.IsNullOrWhiteSpace(Mensaje))
            {
                chatClient.EnviarMensaje(new MensajeDTO
                {
                    Fecha = DateTime.Now,
                    Mensaje = Mensaje,
                    Origen = chatClient.equipo
                });
            }
        }

        private void ChatClient_MensajeRecibido(object? sender, MensajeDTO e)
        {

            Dispatcher.CurrentDispatcher.Invoke(() =>
            {
                if (e.Mensaje == "**HELLO")
                {
                    e.Mensaje = $"{e.Origen} se ha conectado";
                }

                if (e.Mensaje == "**BYE")
                {
                    e.Mensaje = $"{e.Origen} se ha desconectado";
                }

                Mensajes.Add(e);
                NumeroMensaje = Mensajes.Count - 1;
                PropertyChanged?.Invoke(this, new(nameof(NumeroMensaje)));
            });
        }

        public event PropertyChangedEventHandler? PropertyChanged;
    }
}
