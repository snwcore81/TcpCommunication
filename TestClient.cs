﻿using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Threading;
using TcpCommunication.Classes.Services;
using TcpCommunication.Classes;
using TcpCommunication.Interfaces;

using static TcpCommunication.Interfaces.INetworkAction;
using System.Threading.Tasks;
using TcpCommunication.Classes.Messages;
using System.IO;

namespace TcpCommunication
{
    public class TestClient : INetworkAction
    {
        private static readonly object LOCKOBJECT = new object();

        public ClientService Client;

        public bool StillWorking = true;

        public void NetworkStateChanged(NetworkState a_eState, StateObject a_oStateObj = null)
        {
            lock (LOCKOBJECT)
            {

                switch (a_eState)
                {
                    case NetworkState.Sending:
                        break;

                    case NetworkState.Sent:
                        break;

                    case NetworkState.Connecting:
                        break;

                    case NetworkState.Connected:
                        OnConnected(a_oStateObj);

                        break;

                    case NetworkState.Receiving:                        
                        break;

                    case NetworkState.Received:
                        OnReceived(a_oStateObj);
                        break;

                    case NetworkState.Error:
                        break;
                }
            }
        }

        protected void OnReceived(StateObject a_oStateObj)
        {
            var _client = a_oStateObj.GetObject<ClientService>();

            var _message = MessageFactory.Instance.Create(_client.Data.BufferWithData);

            try
            {
                _message.ProcessResponse(a_oStateObj);
            }
            catch (Exception e)
            {
                Console.WriteLine($"Wystąpił błąd! {e.Message}");
            }

            _client.AsyncReceive();
        }

        protected void OnConnected(StateObject a_oStateObj)
        {
            var _client = a_oStateObj.GetObject<ClientService>();

            Console.Write("Podaj login:");

            var _loginTelegram = new LoginMessage
            {
                Login = Console.ReadLine()
            };

            _client.AsyncSend(_loginTelegram.AsNetworkData());

            OnReceived(_client.SyncReceive());
        }
        public virtual void MenuDisplay()
        {
            Console.WriteLine();
            Console.WriteLine("1 - zaloguj");
            Console.WriteLine("2 - wyślij wiadomość do wszystkich");
            Console.WriteLine("3 - wyślij wiadomość do użytkownika");
            Console.WriteLine("0 - wyjdź");
            Console.WriteLine();
        }

        public virtual void SendMessage(bool a_bToAll)
        {
            Console.WriteLine("Wprowadz dane:");
            string _sTo = "*";

            if (!a_bToAll)
            {
                Console.Write("Do:");
                _sTo = Console.ReadLine();
            }

            Console.Write("Wiadomosc:");
            string _sText = Console.ReadLine();

            TextMessage _msgTo = new TextMessage
            {
                From = Client.Identifier,
                To = _sTo,
                Text = _sText
            };

            Client.AsyncSend(_msgTo.AsNetworkData());
        }

        public virtual void Run()
        {
            Client = new ClientService(IPAddress.Loopback, 1000)
            {
                NetworkAction = this
            };

            MenuDisplay();

            while (StillWorking)
            {
                if (Console.KeyAvailable)
                {
                    switch (Console.ReadKey(true).Key)
                    {
                        case ConsoleKey.D0:
                            StillWorking = false; break;

                        case ConsoleKey.D1:
                            if (!Client.IsConnected)
                                Client.Establish();
                            break;

                        case ConsoleKey.D2:
                            SendMessage(true);
                            break;

                        case ConsoleKey.D3:
                            SendMessage(false);
                            break;

                    }
                }
                else
                    Thread.Sleep(10);
            }
        }
    }
}
