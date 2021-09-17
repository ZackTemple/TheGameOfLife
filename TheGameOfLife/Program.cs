using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TheGameOfLife.Services;

namespace TheGameOfLife
{
    class Program
    {

        static void Main(string[] args)
        {
            Console.WriteLine("Welcome to the Game of Life by John Conway!\n");
            var service = new GameService();
            
            var currentGeneration = service.GetGrid();
            var foobar = service.CheckPopulation(currentGeneration);
            Console.WriteLine("Current Generation:");
            PrintGeneration(currentGeneration);
            System.Threading.Thread.Sleep(1000);
            
            for (int i = 0; i < 100; i++)
            {
                var nextGeneration = service.CalculateNewGeneration(currentGeneration);
                nextGeneration = service.CheckPopulation(nextGeneration);

                Console.WriteLine("New Generation:");
                PrintGeneration(nextGeneration);
                currentGeneration = nextGeneration;
                System.Threading.Thread.Sleep(250);
            }
            
        }

        private static void PrintGeneration(List<List<PersonState>> generation)
        {
            for (int i = 0; i < generation.Count; i++)
            {
                for (int j = 0; j < generation[0].Count; j++)
                {
                    Console.Write(generation[i][j].ToString() + " ");
                }
                Console.WriteLine();
            }
        }
    }
    
}