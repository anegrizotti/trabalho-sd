using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.ServiceModel;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Wcf.Client.ClientMultipleThread
{
    [ServiceContract]
    public interface IMessageService
    {
        [OperationContract]
        byte[] MultiplicaMatrizes(byte[] matrizABytes, byte[] matrizBBytes);
    }
    class ClientMultipleThread
    {
        static void Main(string[] args)
        {
            var timer = new Stopwatch();
            timer.Start();

            FazerComunicação();

            timer.Stop();

            TimeSpan timeTaken = timer.Elapsed;
            string tempoFinal = "Tempo levado: " + timeTaken.ToString(@"m\:ss\.fff");
            Console.WriteLine(tempoFinal);

            Console.WriteLine("Aguardando finalização...");
            Console.ReadLine();
        }

        private static void FazerComunicação()
        {
            Console.WriteLine("press any key to cont...");
            Console.ReadLine();
            string uri = "net.tcp://localhost:6565/MessageService";
            // evitar exceção de bloquear o socket após 10 seg
            NetTcpBinding binding = new NetTcpBinding(SecurityMode.None);
            binding.Security.Mode = SecurityMode.Transport;
            binding.Security.Transport.ClientCredentialType = TcpClientCredentialType.Windows;
            binding.Security.Transport.ProtectionLevel = System.Net.Security.ProtectionLevel.EncryptAndSign;
            binding.MaxReceivedMessageSize = 1000000000;

            // definir tempo máximo da comunicação
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
                return MultiplicaMatrizesDe0A1024(matrizALida, matrizBLida);
            });

            var matrizC3Bytes = Task.Run(() =>
            {
                return MultiplicaMatrizesDe1024A2048(matrizALida, matrizBLida);
            });




            // SalvaMatrizCEmArquivo(result);

            Task.WaitAll(matrizC1Bytes, matrizC2Bytes, matrizC3Bytes);

            //Começou às 22 horas e 33 min.

            DateTime.Now.ToString("HH:mm:ss tt");
             Console.WriteLine("Concluído!");

            double[,] matrizC2Desserializada;

            using (MemoryStream ms2 = new MemoryStream(matrizC1Bytes.Result))
            {
                IFormatter br = new BinaryFormatter();
                matrizC2Desserializada = (double[,])br.Deserialize(ms2);

                //Console.WriteLine(matrizA.ToString());
            }

            var matrizC1 = FormarMatrizResultante(matrizC2Bytes.Result, matrizC3Bytes.Result);
            SalvaMatrizCEmArquivo(matrizC1, matrizC2Desserializada);

        }

        public static double[,] MultiplicaMatrizesDe0A1024(double[,] matrizA, double[,] matrizB)
        {
            double acumula = 0;
            double[,] matriz = new double[4096, 4096];

            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("Iniciando Multiplicação das matrizes de 0 a 1024 no Client.");
            Console.ResetColor();

            //cada iteração representa uma linha da matriz A
            for (int linha = 0; linha < 1024; linha++)
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
            Console.WriteLine("Matrizes de 0 à 1024 multiplicadas com sucesso.");
            Console.ResetColor();

            return matriz;
        }
        public static double[,] MultiplicaMatrizesDe1024A2048(double[,] matrizA, double[,] matrizB)
        {
            double acumula = 0;
            double[,] matriz = new double[4096, 4096];

            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("Iniciando Multiplicação das matrizes de 1024 a 2048 no Client.");
            Console.ResetColor();

            //cada iteração representa uma linha da matriz A
            for (int linha = 1024; linha < 2048; linha++)
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
            Console.WriteLine("Matrizes de 1024 à 2048 multiplicadas com sucesso.");
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

        public static double[,] FormarMatrizResultante(double[,] matrizA, double[,] matrizB)
        {
            double[,] matrizResultante = new double[4096, 4096];

            for (int linha = 0; linha < 1024; linha++)
            {
                for (int coluna = 0; coluna < 4096; coluna++)
                {
                     matrizResultante[linha, coluna] = matrizA[linha, coluna];                    

                }

            }

            for (int coluna = 1024; coluna < 2048; coluna++)
            {
                for (int linha = 0; linha < 4096; linha++)
                {
                    matrizResultante[coluna, linha] = matrizB[coluna, linha];
                }
            }

            return matrizResultante;
        }
    }
}