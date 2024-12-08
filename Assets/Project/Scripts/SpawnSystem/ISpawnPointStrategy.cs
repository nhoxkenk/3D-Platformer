using System.Collections;
using UnityEngine;

namespace Platformer
{
    public interface ISpawnPointStrategy
    {
        public Transform NextSpawnPoint();
    }
}
