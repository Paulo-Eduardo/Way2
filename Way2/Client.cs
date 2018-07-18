using System;
using System.Collections;
using System.Net;
using System.Text;
using Way2.Adapter;

namespace Way2
{
    public class Client
    {
        string ip;
        int porta;
        ISocket clientSock_cliente;
        int indiceInicial;
        int indiceFinal;

        public Client(ISocket socket, string ip, int porta, int indiceInicial, int indiceFinal)
        {
            this.indiceInicial = indiceInicial;
            this.indiceFinal = indiceFinal;
            this.ip = ip;
            this.porta = porta;
            clientSock_cliente = socket;
        }

        public void Connect()
        {
            clientSock_cliente.Connect(ip, porta);
        }

        public void Disconnect()
        {
            clientSock_cliente.Close();
        }
        public string GetNumeroSerie()
        {
            byte[] data = { 0x7D, 0x00, 0x01, 0x01 };
            clientSock_cliente.Send(data);
            byte[] buffer = new byte[50];
            byte[] result = clientSock_cliente.Receive(buffer);

            return Encoding.ASCII.GetString(result, 3, result[1] - 1);
        }

        public void SetIndiceToRead(Int16 i)
        {
            byte[] indice = BitConverter.GetBytes(i);
            byte[] data = { 0x7D, 0x02, 0x03, indice[1], indice[0], (byte)(0x02 ^ 0x03 ^ indice[0] ^ indice[1]) };
            string teste = BitConverter.ToString(data);
            clientSock_cliente.Send(data);
            byte[] result = new byte[50];
            clientSock_cliente.Receive(result);
        }

        public double GetValorEnergia()
        {
            byte[] data = { 0x7D, 0x00, 0x05, 0x05 };
            clientSock_cliente.Send(data);
            byte[] buffer = new byte[50];
            var result = clientSock_cliente.Receive(buffer);

            return GetFloatFromByte(result);
        }

        private static double GetFloatFromByte(byte[] result)
        {
            if (result[1] == 0)
                return -1;

            byte[] byteValue = new byte[4];
            for (int i = 0; i < 4; i++)
            {
                byteValue[i] = result[i + 3];
            }

            Array.Reverse(byteValue);

            var bigValue = BitConverter.ToSingle(byteValue, 0);
            return Math.Round(bigValue, 2);
        }

        public DateTime GetDataHora()
        {
            byte[] data = { 0x7D, 0x00, 0x04, 0x04 };
            clientSock_cliente.Send(data);
            byte[] buffer = new byte[50];
            var result = clientSock_cliente.Receive(buffer);
            return GetDateTime(result);
        }

        private DateTime GetDateTime(byte[] result)
        {
            byte[] dataHora = new byte[5];
            for (int i = 0; i < 5; i++)
            {
                dataHora[i] = result[i + 3];
            }

            Array.Reverse(dataHora);
            var teste = GetBit(dataHora);
            var ano = getIntFromBitArray(teste, 28, 12);
            var mes = getIntFromBitArray(teste, 24, 4);
            var dia = getIntFromBitArray(teste, 19, 5);
            var hora = getIntFromBitArray(teste, 14, 5);
            var minuto = getIntFromBitArray(teste, 8, 6);
            var segundo = getIntFromBitArray(teste, 2, 6);

            return new DateTime(ano, mes, dia, hora, minuto, segundo);
        }

        private static int getIntFromBitArray(BitArray bitArray, int biteInicial, int tamanho)
        {
            int value = 0;
            for (int i = biteInicial; i < biteInicial + tamanho; i++)
            {
                if (bitArray[i])
                    value += Convert.ToInt16(Math.Pow(2, i - biteInicial));
            }

            return value;
        }

        private BitArray GetBit(byte[] b)
        {
            return new BitArray(b);
        }

        public int[] GetRegistroStatus()
        {
            byte[] data = { 0x7D, 0x00, 0x02, 0x02 };
            clientSock_cliente.Send(data);
            byte[] buffer = new byte[8];
            var result = clientSock_cliente.Receive(buffer);
            Array.Reverse(result);

            int first = BitConverter.ToUInt16(result, 3);
            int last = BitConverter.ToUInt16(result, 1);

            if (indiceInicial > first)
                if (last > indiceInicial)
                    first = indiceInicial;
                else
                {
                    first = 0;
                    last = 0;
                }

            if (indiceFinal < last)
                if (first < indiceFinal)
                    last = indiceFinal;
                else
                {
                    first = 0;
                    last = 0;
                }


            return new int[] { first, last };
        }
    }
}
