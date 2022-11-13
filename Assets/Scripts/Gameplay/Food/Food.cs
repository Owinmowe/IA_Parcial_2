using UnityEngine;

namespace IA.Gameplay
{
    public class Food : MonoBehaviour, IFood
    {
        public Vector2Int CurrentPosition { get; set; }
        public void GetEaten(Agent agent)
        {
            agent.FoodTaken++;
            Destroy(gameObject);
        }
    }
    
}

