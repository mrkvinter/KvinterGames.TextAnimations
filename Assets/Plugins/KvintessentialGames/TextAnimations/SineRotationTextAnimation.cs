using Sirenix.OdinInspector;
using UnityEngine;

namespace KvintessentialGames.TextAnimations
{
    public class SineRotationTextAnimation : BaseSineTextAnimation
    {
        [Title("Sine Rotation Settings")]
        [SerializeField] private float magnitude;

        public override void ApplyCharTransform(ref CharTransformation charTransformation)
        {
            var rotation = GetSinValue() * magnitude;

            charTransformation.Rotation += rotation;
        }
    }
}