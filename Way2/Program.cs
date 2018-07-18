using System;
using System.Globalization;
using System.IO;
using Way2.Adapter;

namespace Way2
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {   // Open the text file using a stream reader.
                using (StreamReader sr = new StreamReader(args[0]))
                {
                    using (StreamWriter outputFile = new StreamWriter("output.csv"))
                    {
                        string line;
                        while ((line = sr.ReadLine()) != null)
                        {
                            string[] linha = line.Split(' ');
                            try
                            {
                                Client client = new Client(new SocketAdapter(), linha[0], int.Parse(linha[1]), int.Parse(linha[2]), int.Parse(linha[3]));
                                client.Connect();

                                outputFile.WriteLine(client.GetNumeroSerie());

                                var indices = client.GetRegistroStatus();
                                var firstIndice = indices[0];
                                var lastIndice = indices[1];


                                for (int i = indices[0]; i <= indices[1]; i++)
                                {
                                    client.SetIndiceToRead((short)i);
                                    var date = client.GetDataHora();
                                    var value = client.GetValorEnergia();
                                    outputFile.WriteLine(i + ";" + date.ToString(CultureInfo.CreateSpecificCulture("pt-BR")) + ";" + value.ToString(CultureInfo.CreateSpecificCulture("pt-BR")));
                                }

                                client.Disconnect();
                            }
                            catch (Exception e)
                            {
                                Console.WriteLine(e.Message);
                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }
    }
}
