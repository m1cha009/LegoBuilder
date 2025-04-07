using UnityEngine;
using UnityEngine.Serialization;

namespace BrickBuilder
{
    public class BrickStats : MonoBehaviour
    {
        //Use this to set the hieght of individual bricks.  This helps when placing bricks.
        [FormerlySerializedAs("brickHeight")] [Tooltip("Plates = .0032 | Regular = .0096 | 2xRegular = .0192 | 3xRegular = .0288 | 4xRegular = .0384")]
        public float _brickHeight = 0.0032f;
        
        public float GetBrickHeight() => _brickHeight;
    }
}