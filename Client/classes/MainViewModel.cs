using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace Client.classes
{
    public class MainViewModel : ViewModelBase
    {
        public MainViewModel() {
            Name = "";
            Message = "";
            StartBtnEnable = true;
            StopBtnEnable = false;
            SendBtnEnable = false;
        }

        public string Name
        {
            get => _name;
            set => SetProperty(ref _name, value);
        }
        public string Message
        {
            get => _message;
            set => SetProperty(ref _message, value);
        }
        public bool SendBtnEnable
        {
            get => _sendBtnEnable;
            set => SetProperty(ref _sendBtnEnable, value);
        }
        public bool StartBtnEnable
        {
            get => _startBtnEnable;
            set => SetProperty(ref _startBtnEnable, value);
        }
        public bool StopBtnEnable
        {
            get => _stopBtnEnable;
            set => SetProperty(ref _stopBtnEnable, value);
        }
        public int LocalPort
        {
            get => _localPort;
            set => SetProperty(ref _localPort, value);
        }
        public int RemotePort
        {
            get => _remotePort;
            set => SetProperty(ref _remotePort, value);
        }

        public static BindingList<Message> Messages { get; private set; } = new BindingList<Message>();

        public RelayCommand SendMessageCommand {
            get => new RelayCommand(() =>
            {
                NetHandler.Instance.Send(Message);
                Message = "";
            });
        }
        public RelayCommand StopCommand { 
            get => new RelayCommand(() =>
            {
                NetHandler.Instance.Stop();

                StartBtnEnable = true;
                SendBtnEnable = false;
                StopBtnEnable = false;
            }); 
        }
        public RelayCommand StartCommand
        {
            get => new RelayCommand(() =>
            {  
                NetHandler.Instance.LocalPort = LocalPort;
                NetHandler.Instance.RemotePort = RemotePort;

                NetHandler.Instance.Start(
                    Name,
                    (Message value) => AddMessage(value)
                );

                StartBtnEnable = false;
                SendBtnEnable = true;
                StopBtnEnable = true;
            });
        }

        private void AddMessage(Message message)
        {
            Messages.Add(message);
        }

        private string _name;
        private string _message;

        private bool _startBtnEnable;
        private bool _stopBtnEnable;
        private bool _sendBtnEnable;

        private int _localPort = 7777;
        private int _remotePort = 7777;
    }
}
