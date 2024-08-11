using UnityEngine;

namespace KvintessentialGames.TextAnimations
{
    public class SineColorTextAnimation : BaseSineTextAnimation
    {
        [SerializeField] private Color32 fromColor;
        [SerializeField] private Color32 toColor;
        
        public override void ApplyCharTransform(ref CharTransformation charTransformation)
        {
            var sin = GetSinValue(charTransformation.Position.x);
            var color = Color32.Lerp(fromColor, toColor, sin);
            
            charTransformation.Color = color;
        }
    }
}