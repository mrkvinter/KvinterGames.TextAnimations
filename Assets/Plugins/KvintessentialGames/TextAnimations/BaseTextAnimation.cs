using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using UnityEngine;

namespace KvintessentialGames.TextAnimations
{
    public abstract class BaseTextAnimation : MonoBehaviour
    {
        [Title("Base Settings")]
        [SerializeField] private bool animationEnabled = true;

        [SerializeField] private bool tagAnimation;

        [ShowIf("tagAnimation"), ValueDropdown(nameof(GetTags), IsUniqueList = true), SerializeField]
        private string animationTag;

        [SerializeField] private bool wholeAnimation;
        
        public bool IsAnimationEnabled => animationEnabled;
        public bool IsTagAnimation => tagAnimation;
        public string AnimationTag => animationTag;
        public bool IsWholeAnimation => wholeAnimation;

        public virtual void OnStartAnimation()
        {
        }

        public virtual void OnEndAnimation()
        {
        }

        public abstract void ApplyCharTransform(ref CharTransformation charTransformation);

        private IEnumerable GetTags()
        {
            var text = GetComponent<TMP_AnimatedText>();
            var tags = new List<string> {""};
            tags.AddRange(text.textInfo.linkInfo.Select(e => e.GetLinkID()));
            
            return tags;
        }
    }

    public struct CharTransformation
    {
        public Vector3 Position;
        public float Rotation;
        public Vector3 Scale;
        public Color32 Color;

        public Dictionary<BaseTextAnimation, Vector3> PositionAdditionalData; 
        public Dictionary<BaseTextAnimation, float> RotationAdditionalData;
    }
}