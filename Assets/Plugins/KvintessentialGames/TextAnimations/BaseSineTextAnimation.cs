using Sirenix.OdinInspector;
using UnityEngine;

namespace KvintessentialGames.TextAnimations
{
    public abstract class BaseSineTextAnimation : BaseTextAnimation
    {
        [Title("Sine Settings")]
        [SerializeField] private float speed = 5;
        [SerializeField] private float sinFrequency = 0.1f;
        [SerializeField] private float timeOffset;
        [SerializeField] private bool useAnimationCurve;
        [SerializeField, ShowIf(nameof(useAnimationCurve))] private AnimationCurve animationCurve = AnimationCurve.Linear(0, 0, 1, 1);

        protected float GetSinValue(float additionalOffset = 0)
        {
            var offset = Time.time * speed * 1f / sinFrequency + timeOffset;
            var sin = Mathf.Sin((offset + additionalOffset) * sinFrequency) * 0.5f + 0.5f;
            if (useAnimationCurve)
            {
                sin = animationCurve.Evaluate(sin);
            }
                
            return sin;
        }
    }
}