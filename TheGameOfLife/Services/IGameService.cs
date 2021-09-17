using System.Collections.Generic;

namespace TheGameOfLife.Services
{
    public interface IGameService
    {
        public List<List<PersonState>> GetGrid();
        public List<List<PersonState>> CalculateNewGeneration(List<List<PersonState>> currentGeneration);
        public int GetNumOfAliveNeighbors(List<List<PersonState>> currentGeneration, int i, int j);
        public bool NeighborIsAlive(List<List<PersonState>> currentGeneration, int m, int n);
        public bool CalculateNewState(PersonState currentState, int numOfAliveNeighbors);
        public List<List<PersonState>> CheckPopulation(List<List<PersonState>> currentGeneration);
    }
}