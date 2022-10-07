namespace SistemasDistribuidos
{
    class Program
    {
        static void Main(string[] args)
        {
            LocalLinear localLinear = new LocalLinear();
            localLinear.Start();

            //LocalParalela localParalela = new LocalParalela();
            //localParalela.Start();
        }
    }
}
