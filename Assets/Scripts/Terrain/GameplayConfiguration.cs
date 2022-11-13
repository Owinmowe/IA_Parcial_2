using System;
using System.Collections.Generic;
using UnityEngine;
using IA.Gameplay;
using Random = UnityEngine.Random;

namespace IA.Configurations
{
    [CreateAssetMenu(fileName = "Gameplay Configuration", menuName = "ScriptableObjects/Gameplay Configuration", order = 1)]
    public class GameplayConfiguration : ScriptableObject
    {
        [Header("Terrain")]
        [SerializeField] private GameObject terrainPrefab = default;
        [SerializeField] private Vector2Int terrainCount = Vector2Int.zero;
        [SerializeField] private Vector3 eachTerrainSize = Vector3.one;

        [Header("Agents")] 
        [SerializeField] private Agent topAgentPrefabs = default;
        [SerializeField] private Agent botAgentPrefabs = default;
        [SerializeField] private Vector3 agentSpawnOffset = Vector3.zero;

        [Header("Food")] 
        [SerializeField] private Food foodPrefab = default;
        [SerializeField] private Vector3 foodSpawnOffset = Vector3.zero;
        
        [Header("General")]
        [SerializeField] private int turnsPerGeneration = 500;
        
        public List<Agent> RedAgentsList { get; private set; }
        public List<Agent> GreenAgentList { get; private set; }

        private int _greenAgentsAmount = 0;
        private int _redAgentsAmount = 0;
        
        private List<Food> _foodList;
        public Vector2Int TerrainCount => terrainCount;
        public int TurnsPerGeneration => turnsPerGeneration;

        public void CreateTerrain(Transform parent)
        {
            for (int i = 0; i < terrainCount.x; i++)
            {
                for (int j = 0; j < terrainCount.y; j++)
                {
                    Vector3 position = Vector3.zero;
                    position.x = eachTerrainSize.x * i;
                    position.z = eachTerrainSize.z * j;
                    Instantiate(terrainPrefab, position, Quaternion.identity, parent);
                }
            }
        }

        public void CreateAgents(int greenAgentAmount, int redAgentAmount)
        {
            _greenAgentsAmount = greenAgentAmount;
            GreenAgentList.Clear();
            GreenAgentList = new List<Agent>(greenAgentAmount);
            
            List<int> possiblePositions = new List<int>(terrainCount.x);
            for (int i = 0; i < terrainCount.x; i++)
            {
                possiblePositions.Add(i);
            }
            ShuffleList(possiblePositions);

            for (int i = 0; i < greenAgentAmount; i++)
            {
                Vector3 position = Vector3.zero;
                position.x = possiblePositions[i];
                position.z = 0;
                position += agentSpawnOffset;
                
                var agent = Instantiate(botAgentPrefabs, position, Quaternion.identity);

                Vector2Int positionInt = new Vector2Int
                {
                    x = possiblePositions[i],
                    y = 0
                };

                agent.SetTerrainConfiguration(positionInt, this);
                
                GreenAgentList.Add(agent);
            }
            
            _redAgentsAmount = redAgentAmount;
            RedAgentsList.Clear();
            RedAgentsList = new List<Agent>(redAgentAmount);
            
            possiblePositions.Clear();
            for (int i = 0; i < terrainCount.x; i++)
            {
                possiblePositions.Add(i);
            }
            ShuffleList(possiblePositions);
            
            for (int i = 0; i < redAgentAmount; i++)
            {
                Vector3 position = Vector3.zero;
                position.x = possiblePositions[i];
                position.z = (terrainCount.y - 1) * eachTerrainSize.z;
                position += agentSpawnOffset;
                
                var agent = Instantiate(topAgentPrefabs, position, Quaternion.identity);
                
                Vector2Int positionInt = new Vector2Int
                {
                    x = possiblePositions[i],
                    y = terrainCount.y - 1
                };
                
                agent.SetTerrainConfiguration(positionInt, this);
                RedAgentsList.Add(agent);
            }
            
        }

        public void CreateFood()
        {
            _foodList.Clear();
            _foodList = new List<Food>(_greenAgentsAmount + _redAgentsAmount);
            for (int i = 0; i < _greenAgentsAmount + _redAgentsAmount; i++)
            {
                Vector3 position = Vector3.zero;
                Vector2Int intPosition = new Vector2Int
                {
                    x = Random.Range(0, terrainCount.x),
                    y = Random.Range(1, terrainCount.y - 1)
                };

                if (!_foodList.Exists(food => food.CurrentPosition == intPosition))
                {
                    position.x = eachTerrainSize.x * intPosition.x;
                    position.z = eachTerrainSize.z * intPosition.y;
                    position += foodSpawnOffset;
                    var food = Instantiate(foodPrefab, position, Quaternion.identity);
                    
                    _foodList.Add(food);
                }
                else
                {
                    i--;
                }
            }
        }

        public void ClearAllAgentsAndFood()
        {
            foreach (var food in _foodList)
            {
                Destroy(food.gameObject);
            }
            _foodList.Clear();
            
            foreach (var botAgent in GreenAgentList)
            {
                Destroy(botAgent.gameObject);
            }
            GreenAgentList.Clear();
            
            foreach (var topAgent in RedAgentsList)
            {
                Destroy(topAgent.gameObject);
            }
            RedAgentsList.Clear();
        }
        
        public Vector3 GetPostMovementPosition(Agent agent, Movement.MoveDirection direction)
        {
            var positionInt = agent.CurrentPosition;
            
            switch (direction)
            {
                case Movement.MoveDirection.Down:
                    if (positionInt.y > 0)
                    {
                        positionInt.y--;
                    }
                    break;
                
                case Movement.MoveDirection.Up:
                    if (positionInt.y < terrainCount.y - 1)
                    {
                        positionInt.y++;
                    }
                    break;
                
                case Movement.MoveDirection.Left:
                    if (positionInt.x == 0) positionInt.x = terrainCount.x - 1;
                    else positionInt.x--;
                    break;
                
                case Movement.MoveDirection.Right:
                    if (positionInt.x == terrainCount.x - 1) positionInt.x = 0;
                    else positionInt.x++;
                    break;
            }
            
            Vector3 position = new Vector3
            {
                x = positionInt.x * eachTerrainSize.x,
                z = positionInt.y * eachTerrainSize.z
            };

            agent.CurrentPosition = positionInt;
            
            return position;
        }

        public Vector2Int GetClosestFood(Vector2Int agentPosition)
        {
            Vector2Int closestFoodPosition = agentPosition;
            int closestFoodDistance = terrainCount.x + terrainCount.y + 1;

            foreach (var food in _foodList)
            {
                var newPosition = food.CurrentPosition;
                int newClosestFoodDistance = GetManhattanDistance(agentPosition, newPosition);
                if (newClosestFoodDistance < closestFoodDistance)
                {
                    closestFoodDistance = newClosestFoodDistance;
                    closestFoodPosition = newPosition;
                }
            }

            return closestFoodPosition;
        }
        
        private void ShuffleList<T> (List<T> list)
        {
            var count = list.Count;
            var last = count - 1;
            for (var i = 0; i < last; ++i)
            {
                var r = Random.Range(i, count);
                (list[i], list[r]) = (list[r], list[i]);
            }
        }

        private int GetManhattanDistance(Vector2Int from, Vector2Int to)
        {
            
        }
        
    }
}
