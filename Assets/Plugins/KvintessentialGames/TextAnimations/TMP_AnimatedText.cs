using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using Object = System.Object;

namespace KvintessentialGames.TextAnimations
{
    public class TMP_AnimatedText : TextMeshProUGUI
    {
        private AnimationTagTextPreprocessor animationTextPreprocessor;
        private BaseTextAnimation[] animations;

        private bool textChanged;
        private TMP_MeshInfo[] cachedMeshInfo;
        private List<Vector3[]> originalVertices;
        private CharTransformation[] charTransforms;
        private Dictionary<string, CharTransformation> wholeCharTransforms;

        protected override void Awake()
        {
            base.Awake();
            animationTextPreprocessor = new AnimationTagTextPreprocessor();
            textPreprocessor = animationTextPreprocessor;
            animations = GetComponents<BaseTextAnimation>();

            TMPro_EventManager.TEXT_CHANGED_EVENT.Add(OnTextChanged);

            SetText(text);
            ForceMeshUpdate();
            textChanged = true;
            cachedMeshInfo = textInfo.CopyMeshInfoVertexData();
        }

        private void OnTextChanged(Object obj)
        {
            if (obj is TMP_AnimatedText tmpAnimatedText && tmpAnimatedText == this)
            {
                textChanged = true;
            }
        }

        protected void Update()
        {
#if UNITY_EDITOR
            if (!Application.isPlaying)
            {
                return;
            }
#endif
            if (textChanged)
            {
                textChanged = false;
                cachedMeshInfo = textInfo.CopyMeshInfoVertexData();
                originalVertices = new List<Vector3[]>();
                charTransforms = new CharTransformation[textInfo.characterCount];
                wholeCharTransforms = new Dictionary<string, CharTransformation>();
                for (var i = 0; i < charTransforms.Length; i++)
                {
                    charTransforms[i].PositionAdditionalData = new Dictionary<BaseTextAnimation, Vector3>();
                    charTransforms[i].RotationAdditionalData = new Dictionary<BaseTextAnimation, float>();
                }
                //all text
                wholeCharTransforms[""] = new CharTransformation
                {
                    PositionAdditionalData = new Dictionary<BaseTextAnimation, Vector3>(),
                    RotationAdditionalData = new Dictionary<BaseTextAnimation, float>()
                };
                foreach (var (key, _) in animationTextPreprocessor.AnimationTags)
                {
                    var wholeCharTransform = new CharTransformation
                    {
                        PositionAdditionalData = new Dictionary<BaseTextAnimation, Vector3>(),
                        RotationAdditionalData = new Dictionary<BaseTextAnimation, float>()
                    };
                    wholeCharTransforms[key] = wholeCharTransform;
                }
                for (var i = 0; i < cachedMeshInfo.Length; i++)
                {
                    var array = new Vector3[cachedMeshInfo[i].vertices.Length];
                    Array.Copy(cachedMeshInfo[i].vertices, array, cachedMeshInfo[i].vertices.Length);
                    originalVertices.Add(array);
                }
            }

            var characterCount = textInfo.characterCount;
            if (characterCount == 0)
            {
                return;
            }

            foreach (var textAnimation in animations)
            {
                textAnimation.OnStartAnimation();
            }

            var wholeAnimation = new Dictionary<BaseTextAnimation, List<int>>();
            for (var i = 0; i < characterCount; i++)
            {
                var charInfo = textInfo.characterInfo[i];

                if (!charInfo.isVisible)
                {
                    continue;
                }

                var materialIndex = charInfo.materialReferenceIndex;
                var vertexIndex = charInfo.vertexIndex;
                var sourceVertices = originalVertices[materialIndex];
                var destinationVertices = textInfo.meshInfo[materialIndex].vertices;
                var colors = textInfo.meshInfo[materialIndex].colors32;

                var middle = (sourceVertices[vertexIndex + 0] + sourceVertices[vertexIndex + 2]) / 2;
                var charTransform = charTransforms[i];
                charTransform.Position = middle;
                charTransform.Scale = Vector3.one;
                charTransform.Rotation = 0;
                charTransform.Color = colors[vertexIndex];
                foreach (var textAnimation in animations)
                {
                    if (!textAnimation.IsAnimationEnabled)
                        continue;

                    if (!textAnimation.IsTagAnimation ||
                        (animationTextPreprocessor.AnimationTags.TryGetValue(textAnimation.AnimationTag,
                            out var range) && i >= range.Item1 && i < range.Item2))
                    {
                        if (textAnimation.IsWholeAnimation)
                        {
                            if (!wholeAnimation.ContainsKey(textAnimation))
                            {
                                wholeAnimation[textAnimation] = new List<int>();
                            }

                            wholeAnimation[textAnimation].Add(i);
                        }
                        else
                        {
                            textAnimation.ApplyCharTransform(ref charTransform);
                        }
                    }
                }
                
                charTransforms[i] = charTransform;

                var matrix = Matrix4x4.TRS(middle - charTransform.Position,
                    Quaternion.Euler(0, 0, charTransform.Rotation),
                    charTransform.Scale);

                destinationVertices[vertexIndex + 0] = sourceVertices[vertexIndex + 0] - middle;
                destinationVertices[vertexIndex + 1] = sourceVertices[vertexIndex + 1] - middle;
                destinationVertices[vertexIndex + 2] = sourceVertices[vertexIndex + 2] - middle;
                destinationVertices[vertexIndex + 3] = sourceVertices[vertexIndex + 3] - middle;

                destinationVertices[vertexIndex + 0] =
                    matrix.MultiplyPoint3x4(destinationVertices[vertexIndex + 0]);
                destinationVertices[vertexIndex + 1] =
                    matrix.MultiplyPoint3x4(destinationVertices[vertexIndex + 1]);
                destinationVertices[vertexIndex + 2] =
                    matrix.MultiplyPoint3x4(destinationVertices[vertexIndex + 2]);
                destinationVertices[vertexIndex + 3] =
                    matrix.MultiplyPoint3x4(destinationVertices[vertexIndex + 3]);

                destinationVertices[vertexIndex + 0] += middle;
                destinationVertices[vertexIndex + 1] += middle;
                destinationVertices[vertexIndex + 2] += middle;
                destinationVertices[vertexIndex + 3] += middle;
                
                colors[vertexIndex + 0] = charTransform.Color;
                colors[vertexIndex + 1] = charTransform.Color;
                colors[vertexIndex + 2] = charTransform.Color;
                colors[vertexIndex + 3] = charTransform.Color;
            }

            foreach (var (textAnimation, indexes) in wholeAnimation)
            {
                var middles = new List<Vector3>();
                foreach (var index in indexes)
                {
                    var charInfo = textInfo.characterInfo[index];
                    var materialIndex = charInfo.materialReferenceIndex;
                    var vertexIndex = charInfo.vertexIndex;
                    var sourceVertices = originalVertices[materialIndex];
                    var middle = (sourceVertices[vertexIndex + 0] + sourceVertices[vertexIndex + 2]) / 2;
                    middles.Add(middle);
                }

                var center = Vector3.zero;
                foreach (var middle in middles)
                {
                    center += middle;
                }

                center /= middles.Count;

                var firstIndex = indexes[0];
                var charTransform = wholeCharTransforms[textAnimation.AnimationTag];
                charTransform.Position = center;
                charTransform.Scale = Vector3.one;
                charTransform.Rotation = 0;
                charTransform.Color = textInfo.characterInfo[firstIndex].color;
                textAnimation.ApplyCharTransform(ref charTransform);

                var matrix = Matrix4x4.TRS(center - charTransform.Position,
                    Quaternion.Euler(0, 0, charTransform.Rotation),
                    charTransform.Scale);
                
                wholeCharTransforms[textAnimation.AnimationTag] = charTransform;

                foreach (var index in indexes)
                {
                    var charInfo = textInfo.characterInfo[index];
                    var materialIndex = charInfo.materialReferenceIndex;
                    var vertexIndex = charInfo.vertexIndex;
                    var destinationVertices = textInfo.meshInfo[materialIndex].vertices;
                    var colors = textInfo.meshInfo[materialIndex].colors32;

                    destinationVertices[vertexIndex + 0] -= center;
                    destinationVertices[vertexIndex + 1] -= center;
                    destinationVertices[vertexIndex + 2] -= center;
                    destinationVertices[vertexIndex + 3] -= center;

                    destinationVertices[vertexIndex + 0] =
                        matrix.MultiplyPoint3x4(destinationVertices[vertexIndex + 0]);
                    destinationVertices[vertexIndex + 1] =
                        matrix.MultiplyPoint3x4(destinationVertices[vertexIndex + 1]);
                    destinationVertices[vertexIndex + 2] =
                        matrix.MultiplyPoint3x4(destinationVertices[vertexIndex + 2]);
                    destinationVertices[vertexIndex + 3] =
                        matrix.MultiplyPoint3x4(destinationVertices[vertexIndex + 3]);

                    destinationVertices[vertexIndex + 0] += center;
                    destinationVertices[vertexIndex + 1] += center;
                    destinationVertices[vertexIndex + 2] += center;
                    destinationVertices[vertexIndex + 3] += center;
                    
                    colors[vertexIndex + 0] = charTransform.Color;
                    colors[vertexIndex + 1] = charTransform.Color;
                    colors[vertexIndex + 2] = charTransform.Color;
                    colors[vertexIndex + 3] = charTransform.Color;
                }
            }

            foreach (var textAnimation in animations)
            {
                textAnimation.OnEndAnimation();
            }

            for (var i = 0; i < textInfo.meshInfo.Length; i++)
            {
                textInfo.meshInfo[i].mesh.vertices = textInfo.meshInfo[i].vertices;
                textInfo.meshInfo[i].mesh.colors32 = textInfo.meshInfo[i].colors32;
                UpdateGeometry(textInfo.meshInfo[i].mesh, i);
            }
        }
    }
}