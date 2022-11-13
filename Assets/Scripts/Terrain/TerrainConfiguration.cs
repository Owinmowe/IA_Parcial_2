using System.Collections.Generic;
using UnityEngine;
using IA.Gameplay;

namespace IA.Configurations
{
    [CreateAssetMenu(fileName = "Terrain Configuration", menuName = "ScriptableObjects/Terrain Configuration", order = 1)]
    public class TerrainConfiguration : ScriptableObject
    {
        [Header("Terrain")]
        [SerializeField] private GameObject terrainPrefab = default;
        [SerializeField] private Vector2Int terrainCount = Vector2Int.zero;
        [SerializeField] private Vector3 eachTerrainSize = Vector3.one;

        [Header("Agents")] 
        [SerializeField] private Agent topAgentPrefabs = default;
        [SerializeField] private Agent botAgentPrefabs = default;
        [SerializeField] private Vector3 agentSpawnOffset = Vector3.zero;
        [SerializeField] private int agentAmount = 0;

        [Header("Food")] 
        [SerializeField] private Food foodPrefab = default;
        [SerializeField] private Vector3 foodSpawnOffset = Vector3.zero;
        
        public List<Agent> TopAgentList { get; private set; }
        public List<Agent> BotAgentList { get; private set; }
        private List<Food> _foodList;
        public Vector2Int TerrainCount => terrainCount;
        
        private void OnValidate()
        {
            agentAmount = ValidatedAgentAmount();
        }

        private int ValidatedAgentAmount()
        {
            if (agentAmount > terrainCount.x) return terrainCount.x;
            if (agentAmount < 0) return 0;
            return agentAmount;
        }

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

        public void CreateAgents()
        {
            
            BotAgentList.Clear();
            BotAgentList = new List<Agent>(agentAmount);
            
            List<int> possiblePositions = new List<int>(terrainCount.x);
            for (int i = 0; i < terrainCount.x; i++)
            {
                possiblePositions.Add(i);
            }
            ShuffleList(possiblePositions);

            for (int i = 0; i < agentAmount; i++)
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
                
                BotAgentList.Add(agent);
            }

            
            TopAgentList.Clear();
            TopAgentList = new List<Agent>(agentAmount);
            
            possiblePositions.Clear();
            for (int i = 0; i < terrainCount.x; i++)
            {
                possiblePositions.Add(i);
            }
            ShuffleList(possiblePositions);
            
            for (int i = 0; i < agentAmount; i++)
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
                TopAgentList.Add(agent);
            }
            
        }

        public void CreateFood()
        {
            _foodList.Clear();
            _foodList = new List<Food>(agentAmount * 2);
            for (int i = 0; i < agentAmount * 2; i++)
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
            
            foreach (var botAgent in BotAgentList)
            {
                Destroy(botAgent.gameObject);
            }
            BotAgentList.Clear();
            
            foreach (var topAgent in TopAgentList)
            {
                Destroy(topAgent.gameObject);
            }
            TopAgentList.Clear();
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
            Vector2Int closestFood = new Vector2Int(terrainCount.x + 1, terrainCount.y + 1);
            int manhattanDistance = closestFood.x + closestFood.y;

            foreach (var food in _foodList)
            {
                var newPosition = food.CurrentPosition;
                if (newPosition.x + newPosition.y < manhattanDistance)
                {
                    closestFood = newPosition;
                }
            }

            return closestFood;
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
        
    }
}
