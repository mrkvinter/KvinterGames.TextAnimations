using System.Collections.Generic;
using UnityEngine;

namespace KvintessentialGames.TextAnimations
{
    public class SmoothShakingTextAnimation : BaseTextAnimation
    {
        [SerializeField] private float frequency;
        [SerializeField] private float strength = 2.5f;
        [SerializeField] private float angle = 5f;

        private float timer;
        private bool animating;

        public override void OnStartAnimation()
        {
            timer += Time.deltaTime;
            animating = false;
            
            if (timer > frequency)
            {
                timer = 0;
                animating = true;
            }
        }

        public override void ApplyCharTransform(ref CharTransformation charTransformation)
        {
            if (animating)
            {
                var vectorShift = Random.insideUnitCircle * (strength * 0.1f);
                charTransformation.PositionAdditionalData[this] = vectorShift;
                charTransformation.RotationAdditionalData[this] = Random.Range(-angle, angle);
                charTransformation.FloatParamAdditionalData[this] = 0;
            }

            charTransformation.FloatParamAdditionalData[this] += Time.deltaTime / (frequency <= 0 ? Time.deltaTime : frequency);
            var t = charTransformation.FloatParamAdditionalData[this];
            charTransformation.Position = Vector3.Lerp(charTransformation.Position, charTransformation.PositionAdditionalData.GetValueOrDefault(this), t);
            charTransformation.Rotation = Mathf.Lerp(charTransformation.Rotation, charTransformation.RotationAdditionalData.GetValueOrDefault(this), t);
        }
    }
}