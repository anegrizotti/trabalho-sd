using System;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace SistemasDistribuidos
{
    class LocalParalela
    {
        public async void Start()
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("Multiplicação de Matrizes com duas ou mais threads na máquina local.");
            Console.ResetColor();

            Thread.CurrentThread.CurrentCulture = new CultureInfo("en-US");

            var matrizA = Task.Run(() =>
            {
                return LeMatrizA();
            });

            var matrizB = Task.Run(() =>
            {
                return LeMatrizB();
            });

            Task.WaitAll(matrizA, matrizB);

            var matrizCparteUm = Task.Run(() =>
            {
                return MultiplicaMatrizesDe0A1024(matrizA.Result, matrizB.Result);
            });

            var matrizCparteDois = Task.Run(() =>
            {
                return MultiplicaMatrizesDe1024A2048(matrizA.Result, matrizB.Result);
            });

            var matrizCparteTres = Task.Run(() =>
            {
                return MultiplicaMatrizesDe2048A3072(matrizA.Result, matrizB.Result);
            });

            var matrizCparteQuatro = Task.Run(() =>
            {
                return MultiplicaMatrizesDe3072A4096(matrizA.Result, matrizB.Result);
            });

            var timer = new Stopwatch();
            timer.Start();

            Task.WaitAll(matrizCparteUm, matrizCparteDois, matrizCparteTres, matrizCparteQuatro);

            timer.Stop();

            TimeSpan timeTaken = timer.Elapsed;
            string tempoFinal = "Tempo levado: " + timeTaken.ToString(@"m\:ss\.fff");
            Console.WriteLine(tempoFinal);

            SalvaMatrizCEmArquivo(matrizCparteUm.Result, matrizCparteDois.Result, matrizCparteTres.Result, matrizCparteQuatro.Result);
            Console.WriteLine("Aguardando finalização...");
            Console.ReadLine();
        }

        private double[,] LeMatrizA()
        {
            var f = File.ReadAllLines(@"C:\matrizes\matrizA.txt");

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
            Console.WriteLine("Matriz " + "matrizA.txt" + " lida com sucesso.");
            Console.ResetColor();

            return matriz;
        }
        private double[,] LeMatrizB()    
        {
            var f = File.ReadAllLines(@"C:\matrizes\matrizB.txt");

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
            Console.WriteLine("Matriz " + "matrizB.txt" + " lida com sucesso.");
            Console.ResetColor();

            return matriz;
        }
        public double[,] MultiplicaMatrizesDe0A1024(double[,] matrizA, double[,] matrizB)
        {
            double acumula = 0;
            double[,] matriz = new double[4096, 4096];

            //cada iteração representa uma linha da matriz A
            for (int linha = 0; linha < 1024; linha++)
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
            }

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Matrizes de 0 à 1024 multiplicadas com sucesso.");
            Console.ResetColor();

            return matriz;
        }
        public double[,] MultiplicaMatrizesDe1024A2048(double[,] matrizA, double[,] matrizB)
        {
            double acumula = 0;
            double[,] matriz = new double[4096, 4096];

            //cada iteração representa uma linha da matriz A
            for (int linha = 1024; linha < 2048; linha++)
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
            }

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Matrizes de 1024 à 2048 multiplicadas com sucesso.");
            Console.ResetColor();

            return matriz;
        }
        public double[,] MultiplicaMatrizesDe2048A3072(double[,] matrizA, double[,] matrizB)
        {
            double acumula = 0;
            double[,] matriz = new double[4096, 4096];

            //cada iteração representa uma linha da matriz A
            for (int linha = 2048; linha < 3072; linha++)
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
            }

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Matrizes de 2048 à 3072 multiplicadas com sucesso.");
            Console.ResetColor();

            return matriz;
        }
        public double[,] MultiplicaMatrizesDe3072A4096(double[,] matrizA, double[,] matrizB)
        {
            double acumula = 0;
            double[,] matriz = new double[4096, 4096];

            for (int linha = 3072; linha < 4096; linha++)
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
            Console.WriteLine("Matrizes de 3072 à 4096 multiplicadas com sucesso.");
            Console.ResetColor();

            return matriz;
        }
        private static void SalvaMatrizCEmArquivo(double[,] matrizA, double[,] matrizB, double[,] matrizC, double[,] matrizD)
        {
            using (TextWriter tw = new StreamWriter("matrizC.txt"))
            {
                for (int j = 0; j < 1024; j++)
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

                for (int j = 1024; j < 2048; j++)
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

                for (int j = 2048; j < 3072; j++)
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

                for (int j = 3072; j < 4096; j++)
                {
                    for (int i = 0; i < 4096; i++)
                    {
                        if (i != 0)
                        {
                            tw.Write(" ");
                        }

                        tw.Write(String.Format("{0:0.0000}", matrizD[j, i]));
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
