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

        private List<Vector2Int> _agentsPositions;
        private List<Vector2Int> _foodPositions;

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

        public void CreateTerrain()
        {
            for (int i = 0; i < terrainCount.x; i++)
            {
                for (int j = 0; j < terrainCount.y; j++)
                {
                    Vector3 position = Vector3.zero;
                    position.x = eachTerrainSize.x * i;
                    position.z = eachTerrainSize.z * j;
                    Instantiate(terrainPrefab, position, Quaternion.identity);
                }
            }
        }

        public void CreateAgents()
        {
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
                
                _agentsPositions.Add(positionInt);
            }

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
                _agentsPositions.Add(positionInt);
            }
            
        }

        public void CreateFood()
        {
            _foodPositions.Clear();
            _foodPositions = new List<Vector2Int>(agentAmount * 2);
            for (int i = 0; i < agentAmount * 2; i++)
            {
                Vector3 position = Vector3.zero;
                Vector2Int intPosition = new Vector2Int
                {
                    x = Random.Range(0, terrainCount.x),
                    y = Random.Range(1, terrainCount.y - 1)
                };

                if (!_foodPositions.Contains(intPosition))
                {
                    _foodPositions.Add(intPosition);
                    
                    position.x = eachTerrainSize.x * intPosition.x;
                    position.z = eachTerrainSize.z * intPosition.y;
                    position += foodSpawnOffset;
                    
                    Instantiate(foodPrefab, position, Quaternion.identity);
                }
                else
                {
                    i--;
                }
            }
        }

        public Vector3 GetPostMovementPosition(Agent agent, Movement.MoveDirection direction)
        {
            int agentIndex = _agentsPositions.FindIndex(i=> i == agent.CurrentPosition);

            var positionInt = _agentsPositions[agentIndex];
            
            switch (direction)
            {
                case Movement.MoveDirection.Down:
                    if (positionInt.y > 0)
                    {
                        positionInt.y--;
                    }
                    break;
                
                case Movement.MoveDirection.Up:
                    if (positionInt.y < terrainCount.y)
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

            _agentsPositions[agentIndex] = positionInt;
            agent.CurrentPosition = positionInt;
            
            return position;
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
