using System;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Threading;

namespace SistemasDistribuidos
{
    class LocalLinear
    {
        public void Start()
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("Multiplicação de Matrizes com uma única thread na máquina local.");
            Console.ResetColor();

            Thread.CurrentThread.CurrentCulture = new CultureInfo("en-US");
            var matrizA = LeMatrizes("matrizA.txt");
            var matrizB = LeMatrizes("matrizB.txt");

            var timer = new Stopwatch();
            timer.Start();

            double[,] matrizC = MultiplicarMatrizes(matrizA, matrizB);

            timer.Stop();

            TimeSpan timeTaken = timer.Elapsed;
            string tempoFinal = "Tempo levado: " + timeTaken.ToString(@"m\:ss\.fff");
            Console.WriteLine(tempoFinal);

            SalvaMatrizCEmArquivo(matrizC);
            Console.WriteLine("Aguardando finalização...");
            Console.ReadLine();
        }

        private static void SalvaMatrizCEmArquivo(double[,] matrizC)
        {
            using (TextWriter tw = new StreamWriter("matrizC.txt"))
            {
                for (int j = 0; j < 4096; j++)
                {
                    for (int i = 0; i < 4096; i++)
                    {
                        if (i != 0)
                        {
                            tw.Write(" ");
                        }

                        tw.Write(String.Format("{0:0.0000}", matrizC[j, i]));
                    }

                    tw.WriteLine();
                }
            }

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Matriz salva com sucesso.");
            Console.ResetColor();
        }

        private static double[,] MultiplicarMatrizes(double[,] matrizA, double[,] matrizB)
        {
            double[,] matriz = new double[4096, 4096];
            double acumula = 0;

            //cada iteração representa uma linha da matriz A
            for (int linha = 0; linha < 4096; linha++)
            {
                Console.WriteLine(linha);

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
                Console.Clear();
            }

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Matrizes multiplicadas com sucesso.");
            Console.ResetColor();

            return matriz;
        }

        private static double[,] LeMatrizes(string caminho)
        {
            var f = File.ReadAllLines(@"C:\matrizes\" + caminho);

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
            Console.WriteLine("Matriz " + caminho + " lida com sucesso.");
            Console.ResetColor();

            return matriz;
        }
    }
}
