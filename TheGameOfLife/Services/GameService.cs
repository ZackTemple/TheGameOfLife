using System;
using System.Collections.Generic;

namespace TheGameOfLife.Services
{
    public class GameService : IGameService
    {
        public List<List<PersonState>> GetGrid()
        {
            var rand = new Random();
            var gen = new List<List<PersonState>>();
            for (int i = 0; i < 20; i++)
            {
                var row = new List<PersonState>();
                for (int j = 0; j < 20; j++)
                {
                    int value = rand.Next();
                    row.Add(new PersonState(false));
                }
                gen.Add(row);
            }

            return gen;
        }
        
        public List<List<PersonState>> CalculateNewGeneration(List<List<PersonState>> currentGeneration)
        {
            var newGeneration = new List<List<PersonState>>();
            
            for (int i = 0; i < currentGeneration.Count; i++)
            {
                var newRow = new List<PersonState>();
                for (int j = 0; j < currentGeneration[0].Count; j++)
                {
                    int numOfAliveNeighbors = GetNumOfAliveNeighbors(currentGeneration, i, j);
                    bool isAliveNextGen = CalculateNewState(currentGeneration[i][j], numOfAliveNeighbors);
                    newRow.Add(new PersonState(isAliveNextGen));
                }
                newGeneration.Add(newRow);
            }
            return newGeneration;
        }

        public int GetNumOfAliveNeighbors(List<List<PersonState>> currentGeneration, int i, int j)
        {
            int numOfAliveNeighbors = 0;

            for (int m = i - 1; m < i + 2; m++)
            {
                for (int n = j - 1; n < j + 2; n++)
                {
                    if ((m != i || n != j) && NeighborIsAlive(currentGeneration, m, n)) 
                    { 
                        numOfAliveNeighbors++;
                    }
                }
            }

            return numOfAliveNeighbors;
        }

        public bool NeighborIsAlive(List<List<PersonState>> currentGeneration, int m, int n)
        {
            try
            {
                bool neighborIsAlive = currentGeneration[m][n].IsAlive;
                return neighborIsAlive;
            }
            catch (System.ArgumentOutOfRangeException) 
            {
                // Return false because neighbor does not exist
                return false;
            }
        }

        public bool CalculateNewState(PersonState currentState, int numOfAliveNeighbors)
        {
            // Rule 1 and 2: In order to stay alive, cannot be under or over populated
            var staysAlive = currentState.IsAlive && (numOfAliveNeighbors == 2 || numOfAliveNeighbors == 3);
            // Rule 3: In order to be resurrected, have to have exactly 3 alive neighbors
            var comesBackToLife = !currentState.IsAlive && numOfAliveNeighbors == 3;

            return staysAlive || comesBackToLife;
        }
        
        public List<List<PersonState>> CheckPopulation(List<List<PersonState>> currentGeneration)
        {
            var minNumOfAlive = GrabMinNumberAlive(currentGeneration);
            if (CalculateTotalNumberOfLiving(currentGeneration) < minNumOfAlive)
            {
                Console.WriteLine("Population low. Repopulating grid.");
                return PopulateGrid(currentGeneration);
            }

            return currentGeneration;
        }

        private int GrabMinNumberAlive(List<List<PersonState>> currentGeneration)
        {
            int rows = currentGeneration.Count;
            int columns = currentGeneration[0].Count;
        
            var sqrtNumOfStates =  Math.Sqrt(rows * columns);
            return Convert.ToInt32(sqrtNumOfStates);
        }
        
        public int CalculateTotalNumberOfLiving(List<List<PersonState>> currentGeneration)
        {
            int totalNumAlive = 0;
            
            for (int i = 0; i < currentGeneration.Count; i++)
            {
                for (int j = 0; j < currentGeneration[0].Count; j++)
                {
                    if (currentGeneration[i][j].IsAlive)
                    {
                        totalNumAlive++;
                    }
                }
            }

            return totalNumAlive;
        }

        private List<List<PersonState>> PopulateGrid(List<List<PersonState>> currentGeneration)
        {
            // Dynamic repopulating
            // int generationNumber = currentGeneration.Count * currentGeneration[0].Count / 8;
            int generationNumber = 60;
            var rand = new Random();
            for (int i = 0; i < generationNumber; i++)
            {
                var index1 = rand.Next(currentGeneration.Count);
                var index2 = rand.Next(currentGeneration[0].Count);
                currentGeneration[index1][index2] = new PersonState(true);
            }

            return currentGeneration;
        }
    }
}