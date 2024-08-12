using Sirenix.OdinInspector;
using UnityEngine;

namespace KvintessentialGames.TextAnimations
{
    public class SinePositionTextAnimation : BaseSineTextAnimation
    {
        [Title("Sine Position Settings")]
        [SerializeField] private Vector2 magnitude;

        public override void ApplyCharTransform(ref CharTransformation charTransformation)
        {
            var position = charTransformation.Position;
            var x = GetSinValue(position.y) * magnitude.x;
            var y = GetSinValue(position.x) * magnitude.y;

            charTransformation.Position += new Vector3(x, y);
        }
    }
}