using Sirenix.OdinInspector;
using UnityEngine;

namespace KvintessentialGames.TextAnimations
{
    public class SineScaleTextAnimation : BaseSineTextAnimation
    {
        [Title("Sine Scale Settings")]
        [SerializeField] private Vector3 scaleOffset;
        [SerializeField] private Vector3 magnitude;
        [SerializeField] private bool usePosition;

        public override void ApplyCharTransform(ref CharTransformation charTransformation)
        {
            var position = charTransformation.Position;
            var x = GetSinValue(usePosition ? position.y : 0) * magnitude.x;
            var y = GetSinValue(usePosition ? position.x : 0) * magnitude.y;

            charTransformation.Scale += new Vector3(x, y) + scaleOffset;
        }
    }
}