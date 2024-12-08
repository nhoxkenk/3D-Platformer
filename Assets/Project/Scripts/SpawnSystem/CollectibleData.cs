using UnityEngine;

namespace Platformer
{
    [CreateAssetMenu(fileName = "CollectibleData", menuName = "Data/Collectible Data")]
    public class CollectibleData: EntityData
    {
        public int score;
        //additional properties specific to collectibles
    }
}
