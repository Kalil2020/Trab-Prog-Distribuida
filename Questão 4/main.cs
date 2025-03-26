using System;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Concurrent;
using System.Linq;

class SomaThread
{
    private int[] vetor;
    private int inicio, fim;
    private ConcurrentBag<int> resultados;

    public SomaThread(int[] vetor, int inicio, int fim, ConcurrentBag<int> resultados)
    {
        this.vetor = vetor;
        this.inicio = inicio;
        this.fim = fim;
        this.resultados = resultados;
    }

    public void CalcularSoma()
    {
        int somaParcial = 0;
        for (int i = inicio; i < fim; i++)
        {
            somaParcial += vetor[i];
        }
        resultados.Add(somaParcial);
    }
}

class Program
{
    static int[] vetor = new int[1000000];
    static object lockObject = new object();

    static void Main()
    {
        Random rand = new Random();
        for (int i = 0; i < vetor.Length; i++)
        {
            vetor[i] = rand.Next(0, 100);
        }

        // Abordagem Sequencial
        int somaSequencial = SomaSequencial(vetor);
        Console.WriteLine($"Soma Sequencial: {somaSequencial}");

        // Abordagem Paralela com SomaThread
        int somaParalela = SomaParalela(vetor, 10);
        Console.WriteLine($"Soma Paralela: {somaParalela}");
    }

    static int SomaSequencial(int[] vetor)
    {
        int soma = 0;
        foreach (int num in vetor)
        {
            soma += num;
        }
        return soma;
    }

    static int SomaParalela(int[] vetor, int numThreads)
    {
        int tamanho = vetor.Length;
        Thread[] threads = new Thread[numThreads];
        ConcurrentBag<int> resultados = new ConcurrentBag<int>();

        for (int i = 0; i < numThreads; i++)
        {
            int inicio = i * (tamanho / numThreads);
            int fim = (i == numThreads - 1) ? tamanho : (i + 1) * (tamanho / numThreads);
            SomaThread somaThread = new SomaThread(vetor, inicio, fim, resultados);
            threads[i] = new Thread(new ThreadStart(somaThread.CalcularSoma));
            threads[i].Start();
        }

        foreach (Thread t in threads)
        {
            t.Join();
        }

        return resultados.ToArray().Sum();
    }
}
