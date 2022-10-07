using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.ServiceModel;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Wcf.Client.ClientSingleThread
{
    [ServiceContract]
    public interface IMessageService
    {
        [OperationContract]
        byte[] MultiplicaMatrizes(byte[] matrizABytes, byte[] matrizBBytes);
    }
    class ClientSingleThread
    {
        //static void Main(string[] args)
        //{

        //    // populate your array or whatever

            

        //    FazerComunicação();
        //}

        private static void FazerComunicação()
        {
            Console.WriteLine("press any key to cont...");
            Console.ReadLine();
            string uri = "net.tcp://localhost:6565/MessageService";
            NetTcpBinding binding = new NetTcpBinding(SecurityMode.None);
            binding.Security.Mode = SecurityMode.Transport;
            binding.Security.Transport.ClientCredentialType = TcpClientCredentialType.Windows;
            binding.Security.Transport.ProtectionLevel = System.Net.Security.ProtectionLevel.EncryptAndSign;
            binding.MaxReceivedMessageSize = 1000000000;
            binding.OpenTimeout = TimeSpan.FromMinutes(40);
            binding.SendTimeout = TimeSpan.FromMinutes(40);
            binding.ReceiveTimeout = TimeSpan.FromMinutes(40);

            var channel = new ChannelFactory<IMessageService>(binding);
            var endpoint = new EndpointAddress(uri);
            var proxy = channel.CreateChannel(endpoint);
            
            //Fazer em bytes
            IFormatter formatter = new BinaryFormatter();
            var ms = new MemoryStream();

            var matrizALida = LerMatriz("matA.txt");
            formatter.Serialize(ms, matrizALida);
            var matrizABytes = ms.ToArray();

            var matrizBLida = LerMatriz("matB.txt");
            formatter.Serialize(ms, matrizBLida);
            var matrizBBytes = ms.ToArray();


            var matrizC1Bytes = Task.Run(() =>
            {
                return proxy?.MultiplicaMatrizes(matrizABytes, matrizBBytes);
            });

            var matrizC2Bytes = Task.Run(() =>
            {
                return MultiplicaMatrizesDe0A2048(matrizALida, matrizBLida);
            });




            // SalvaMatrizCEmArquivo(result);

            Task.WaitAll(matrizC1Bytes, matrizC2Bytes);

            //Começou às 22 horas e 33 min.

            DateTime.Now.ToString("HH:mm:ss tt");
            Console.WriteLine("Concluído!");

            double[,] matrizC1Desserializada;

            using (MemoryStream ms2 = new MemoryStream(matrizC1Bytes.Result))
            {
                IFormatter br = new BinaryFormatter();
                matrizC1Desserializada = (double[,])br.Deserialize(ms2);

                //Console.WriteLine(matrizA.ToString());
            }

            SalvaMatrizCEmArquivo(matrizC2Bytes.Result, matrizC1Desserializada);

        }

        public static double[,] MultiplicaMatrizesDe0A2048(double[,] matrizA, double[,] matrizB)
        {
            double acumula = 0;
            double[,] matriz = new double[4096, 4096];

            //cada iteração representa uma linha da matriz A
            for (int linha = 0; linha < 2048; linha++)
            {
                //em cada linha de A, itera nas colunas de B
                for (int coluna = 0; coluna < 4096; coluna++)
                {
                    //itera, ao mesmo tempo, entre os elementos da linha de A e da coluna de B
                    for (int i = 0; i < 4096; i++)
                    {
                        //acumula representa os valores que estávamos reservando
                        acumula = acumula + matrizA[linha, i] * matrizB[i, coluna];
                    }
                    //quando a execução está aqui, já se tem mais um elemento da matriz AB
                    matriz[linha, coluna] = acumula;

                    //a variável então é zerada para que possa referenciar um novo elemento de AB
                    acumula = 0;
                }
            }

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Matrizes de 0 à 2048 multiplicadas com sucesso.");
            Console.ResetColor();

            return matriz;
        }


        public static double[,] LerMatriz(string matriz)
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("Multiplicação de Matrizes com uma única thread em máquinas separadas.");
            Console.ResetColor();

            Thread.CurrentThread.CurrentCulture = new CultureInfo("en-US");
            return PrencherMatrizes(matriz);

        }


        private static double[,] PrencherMatrizes(string nomeMatriz)
        {
            var f = File.ReadAllLines(@"C:\Projetos IFSC\WCF\WCF.Client\" + nomeMatriz);

            double[,] matriz = new double[4096, 4096];



            for (int i = 0; i < f.Length; i++)
            {
                string[] strlist = f[i].Split(' ');

                for (int j = 0; j < f.Length; j++)
                {
                    matriz[i, j] = double.Parse(strlist[j]);
                }
            }

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Matriz " + nomeMatriz + " lida com sucesso.");
            Console.ResetColor();

            return matriz;
        }
        private static void SalvaMatrizCEmArquivo(double[,] matrizA, double[,] matrizB)
        {
            using (TextWriter tw = new StreamWriter("matrizC.txt"))
            {
                for (int j = 0; j < 2048; j++)
                {
                    for (int i = 0; i < 4096; i++)
                    {
                        if (i != 0)
                        {
                            tw.Write(" ");
                        }

                        tw.Write(String.Format("{0:0.0000}", matrizA[j, i]));
                    }

                    tw.WriteLine();
                }

                for (int j = 2048; j < 4096; j++)
                {
                    for (int i = 0; i < 4096; i++)
                    {
                        if (i != 0)
                        {
                            tw.Write(" ");
                        }

                        tw.Write(String.Format("{0:0.0000}", matrizB[j, i]));
                    }

                    tw.WriteLine();
                }
            }

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Matriz salva com sucesso.");
            Console.ResetColor();
        }
    }
}