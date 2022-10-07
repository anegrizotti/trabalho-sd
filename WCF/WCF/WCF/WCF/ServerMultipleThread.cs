using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace WCF.Server.ServerMultipleThread
{
    public class ServerMultipleThread
    {
        [ServiceContract]
        public interface IMessageService
        {
            [OperationContract]
            byte[] MultiplicaMatrizes(byte[] matrizABytes, byte[] matrizBBytes);

        }

        [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single)]
        public class MessageService : IMessageService
        {
            public byte[] MultiplicaMatrizes(byte[] matrizABytes, byte[] matrizBBytes)
            {
                double[,] matrizA = null, matrizB = null;

                using (MemoryStream ms = new MemoryStream(matrizABytes))
                {
                    IFormatter br = new BinaryFormatter();
                    matrizA = (double[,])br.Deserialize(ms);

                    //Console.WriteLine(matrizA.ToString());
                }

                using (MemoryStream ms = new MemoryStream(matrizABytes))
                {
                    IFormatter br = new BinaryFormatter();
                    matrizB = (double[,])br.Deserialize(ms);

                    // Console.WriteLine(matrizB.ToString());
                }

                var matrizC21 = Task.Run(() =>
                {
                    return MultiplicaMatrizesDe2048A3072(matrizA, matrizB);
                });

                var matrizC22 = Task.Run(() =>
                {
                    return MultiplicaMatrizesDe3072A4096(matrizA, matrizB);
                });


                IFormatter formatter = new BinaryFormatter();
                var msRetorno = new MemoryStream();

                formatter.Serialize(msRetorno, FormarMatrizResultante(matrizC21.Result, matrizC22.Result));
                var matrizC2Bytes = msRetorno.ToArray();

                return matrizC2Bytes;
            }
        }
        static void Main(string[] args)
        {
            var uris = new Uri[1];
            string address = "net.tcp://localhost:6565/MessageService";

            uris[0] = new Uri(address);
            IMessageService message = new MessageService();
            ServiceHost host = new ServiceHost(message, uris);
            var binding = new NetTcpBinding(SecurityMode.None);

            binding.Security.Mode = SecurityMode.Transport;
            binding.Security.Transport.ClientCredentialType = TcpClientCredentialType.Windows;
            binding.Security.Transport.ProtectionLevel = System.Net.Security.ProtectionLevel.EncryptAndSign;
            binding.MaxReceivedMessageSize = 1000000000;
            binding.OpenTimeout = TimeSpan.FromMinutes(40);
            binding.SendTimeout = TimeSpan.FromMinutes(40);
            binding.ReceiveTimeout = TimeSpan.FromMinutes(40);

            host.AddServiceEndpoint(typeof(IMessageService), binding, "");
            host.Opened += Host_Opened;
            host.Open();
            Console.ReadLine();
        }

        private static void Host_Opened(object sender, EventArgs e)
        {
            Console.WriteLine("Message service started");
        }

        public static double[,] MultiplicaMatrizesDe2048A3072(double[,] matrizA, double[,] matrizB)
        {
            double acumula = 0;
            double[,] matriz = new double[4096, 4096];

            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("Iniciando Multiplicação das matrizes de 2048 a 3072 no Server.");
            Console.ResetColor();

            //cada iteração representa uma linha da matriz A
            for (int linha = 2048; linha < 3072; linha++)
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
            Console.WriteLine("Matrizes de 2048 à 3072 multiplicadas com sucesso.");
            Console.ResetColor();

            return matriz;
        }
        public static double[,] MultiplicaMatrizesDe3072A4096(double[,] matrizA, double[,] matrizB)
        {
            double acumula = 0;
            double[,] matriz = new double[4096, 4096];

            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("Iniciando Multiplicação das matrizes de 3072 a 4096 no Server.");
            Console.ResetColor();

            //cada iteração representa uma linha da matriz A
            for (int linha = 3072; linha < 4096; linha++)
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
            Console.WriteLine("Matrizes de 3072 à 4096 multiplicadas com sucesso.");
            Console.ResetColor();

            return matriz;
        }

        public static double[,] FormarMatrizResultante(double[,] matrizA, double[,] matrizB)
        {
            double[,] matrizResultante = new double[4096, 4096];

            for (int j = 2048; j < 3072; j++)
            {
                for (int i = 0; i < 4096; i++)
                {
                    matrizResultante[j, i] = matrizA[j, i];
                }

            }

            for (int j = 3072; j < 4096; j++)
            {
                for (int i = 0; i < 4096; i++)
                {
                    matrizResultante[j, i] = matrizB[j, i];
                }
            }

            return matrizResultante;
        }
    }
}
