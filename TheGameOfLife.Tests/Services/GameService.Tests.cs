using System;
using System.Collections.Generic;
using System.Linq;
using AutoFixture;
using FluentAssertions;
using TheGameOfLife;
using TheGameOfLife.Services;
using Xunit;

namespace TheGameOfLife.Tests.Services
{
    public class GameServiceTests
    {
        private readonly GameService _service;
        private readonly Fixture _fixture = new Fixture();
        private readonly List<List<PersonState>> _testGeneration;

        public GameServiceTests()
        {
            _service = new GameService();
            var rand = new Random();
            _fixture.Register<bool>(() => rand.Next() % 2 == 0);

            _testGeneration = new List<List<PersonState>>()
            {
                _fixture.CreateMany<PersonState>(4).ToList(),
                _fixture.CreateMany<PersonState>(4).ToList(),
                _fixture.CreateMany<PersonState>(4).ToList(),
                _fixture.CreateMany<PersonState>(4).ToList()
            };
        }

        [Fact]
        public void GetGrid_ReturnsANewGrid()
        {
            var grid = _service.GetGrid();
            grid.Should().BeOfType<List<List<PersonState>>>();
        }

        [Fact]
        public void CalculateNewGeneration_ShouldReturnANewGen()
        {
            var newGeneration = _service.CalculateNewGeneration(_testGeneration);

            newGeneration.Should().BeOfType<List<List<PersonState>>>();
        }

        [Fact]
        public void GetNumOfAliveNeighbors_ReturnsTheNumberOfAliveNeighbors()
        {
            var testGeneration1 = new List<List<PersonState>>()
            {
                new List<PersonState>(){new PersonState(false), new PersonState(true), new PersonState(false)},
                new List<PersonState>(){new PersonState(false), new PersonState(false), new PersonState(false)},
                new List<PersonState>(){new PersonState(false), new PersonState(true), new PersonState(false)},
            };
            var testGeneration2 = new List<List<PersonState>>()
            {
                new List<PersonState>(){new PersonState(true), new PersonState(true), new PersonState(false)},
                new List<PersonState>(){new PersonState(false), new PersonState(false), new PersonState(true)},
                new List<PersonState>(){new PersonState(false), new PersonState(true), new PersonState(false)},
            };
            var testGeneration3 = new List<List<PersonState>>()
            {
                new List<PersonState>(){new PersonState(true), new PersonState(true), new PersonState(true)},
                new List<PersonState>(){new PersonState(false), new PersonState(false), new PersonState(true)},
                new List<PersonState>(){new PersonState(true), new PersonState(true), new PersonState(false)},
            };

            var numOfNeighbors1 = _service.GetNumOfAliveNeighbors(testGeneration1, 1, 1);
            var numOfNeighbors2 = _service.GetNumOfAliveNeighbors(testGeneration2, 0, 1);
            var numOfNeighbors3 = _service.GetNumOfAliveNeighbors(testGeneration3, 2, 0);

            numOfNeighbors1.Should().Be(2);
            numOfNeighbors2.Should().Be(2);
            numOfNeighbors3.Should().Be(1);
        }

        [Fact]
        public void NeighborIsAlive_ShouldReturnTrueIfNeighborStateIsAlive()
        {
            // Make sure that the spot we are checking is dead
            _testGeneration[0] = _fixture.Build<PersonState>()
                .With(x => x.IsAlive, true)
                .CreateMany(4).ToList();
            bool isAlive = _service.NeighborIsAlive(_testGeneration, 0, 0);
            isAlive.Should().BeTrue();
        }
        
        [Fact]
        public void NeighborIsAlive_ShouldReturnFalseIfNeighborStateIsNotAlive()
        {
            // Make sure that the spot we are checking is dead
            _testGeneration[0] = _fixture.Build<PersonState>()
                .With(x => x.IsAlive, false)
                .CreateMany(4).ToList();
            
            bool isAlive = _service.NeighborIsAlive(_testGeneration, 0, 0);
            isAlive.Should().BeFalse();
        }
        
        [Fact]
        public void NeighborIsAlive_ShouldReturnFalseIfNeighborStateDoesNotExist()
        {
            bool isAlive = _service.NeighborIsAlive(_testGeneration, -1, -1);
            isAlive.Should().BeFalse();
        }

        [Fact]
        public void CalculateNewState_ReturnsTrueIfNumOfNeighborsIs2or3AndIsAlive()
        {
            var state = new PersonState(true);
            bool newState1 = _service.CalculateNewState(state, 2);
            bool newState2 = _service.CalculateNewState(state, 3);

            newState1.Should().BeTrue();
            newState2.Should().BeTrue();
        }
        
        [Fact]
        public void CalculateNewState_ReturnsFalseIfNumOfNeighborsIsBelow2OrOver3AndIsAlive()
        {
            var state = new PersonState(true);
            bool newState1 = _service.CalculateNewState(state, 1);
            bool newState2 = _service.CalculateNewState(state, 4);

            newState1.Should().BeFalse();
            newState2.Should().BeFalse();
        }
        
        [Fact]
        public void CalculateNewState_ReturnsTrueIfNumOfNeighborsIs3AndStateIsDead()
        {
            var state = new PersonState(false);
            bool newState1 = _service.CalculateNewState(state, 3);

            newState1.Should().BeTrue();
        }
        
        [Fact]
        public void CalculateNewState_ReturnsFalseIfNumOfNeighborsIsNot3AndStateIsDead()
        {
            var state = new PersonState(false);
            bool newState1 = _service.CalculateNewState(state, 1);

            newState1.Should().BeFalse();
        }

        [Fact]
        public void CheckPopulation_PopulatesGridIfPopulationIsLow()
        {
            // Gets empty 20x20 grid
            var testGeneration = _service.GetGrid();

            var newGrid = _service.CheckPopulation(testGeneration);
            var numOfLiving = _service.CalculateTotalNumberOfLiving(newGrid);

            numOfLiving.Should().BeGreaterThan(0);
        }
        
        [Fact]
        public void CheckPopulation_ReturnsInputGridIfPopulationIsOkay()
        {
            // Arrange by setting up a grid with populated values (previous test proves that if population low,
            // the function populates the grid
            var testGeneration = _service.GetGrid();
            testGeneration = _service.CheckPopulation(testGeneration);
            var beforeNumOfLiving = _service.CalculateTotalNumberOfLiving(testGeneration);

            var newGrid = _service.CheckPopulation(testGeneration);
            var afterNumOfLiving = _service.CalculateTotalNumberOfLiving(newGrid);

            afterNumOfLiving.Should().Be(beforeNumOfLiving);
        }

        [Fact]
        public void CalculateTotalNumberOfLiving_ReturnsNumberOfLivingStates()
        {
            var testGeneration1 = new List<List<PersonState>>()
            {
                new List<PersonState>(){new PersonState(false), new PersonState(true), new PersonState(false)},
                new List<PersonState>(){new PersonState(false), new PersonState(false), new PersonState(false)},
                new List<PersonState>(){new PersonState(false), new PersonState(true), new PersonState(false)},
            };
            var testGeneration2 = new List<List<PersonState>>()
            {
                new List<PersonState>(){new PersonState(true), new PersonState(true), new PersonState(false)},
                new List<PersonState>(){new PersonState(false), new PersonState(false), new PersonState(true)},
                new List<PersonState>(){new PersonState(false), new PersonState(true), new PersonState(false)},
            };
            var testGeneration3 = new List<List<PersonState>>()
            {
                new List<PersonState>(){new PersonState(true), new PersonState(true), new PersonState(true)},
                new List<PersonState>(){new PersonState(false), new PersonState(false), new PersonState(true)},
                new List<PersonState>(){new PersonState(true), new PersonState(true), new PersonState(false)},
            };

            int livingStates1 = _service.CalculateTotalNumberOfLiving(testGeneration1);
            int livingStates2 = _service.CalculateTotalNumberOfLiving(testGeneration2);
            int livingStates3 = _service.CalculateTotalNumberOfLiving(testGeneration3);

            livingStates1.Should().Be(2);
            livingStates2.Should().Be(4);
            livingStates3.Should().Be(6);
        }
    }
}