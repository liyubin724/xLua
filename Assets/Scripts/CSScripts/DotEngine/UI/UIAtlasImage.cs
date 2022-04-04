﻿using UnityEngine;
using UnityEngine.U2D;
using UnityEngine.UI;

namespace DotEngine.UI
{
    [AddComponentMenu("DotEngine/UI/Atlas Image", 10)]
    public class UIAtlasImage : Image
    {
        [SerializeField]
        private SpriteAtlas m_SpriteAtlas;
        public SpriteAtlas Atlas
        {
            get
            {
                return m_SpriteAtlas;
            }
            set
            {
                if (m_SpriteAtlas != value)
                {
                    m_SpriteAtlas = value;
                    ChangeSprite();
                }
            }
        }

        [SerializeField]
        private string m_SpriteName = "";
        public string SpriteName
        {
            get
            {
                return m_SpriteName;
            }
            set
            {
                if (m_SpriteName != value)
                {
                    m_SpriteName = value;
                    ChangeSprite();
                }
            }
        }

        protected override void Awake()
        {
            base.Awake();
            if (sprite == null)
            {
                if (Atlas != null && !string.IsNullOrEmpty(SpriteName))
                {
                    ChangeSprite();
                }
            }
            else
            {
                string sn = sprite.name.Replace("(Clone)", "");
                if (sn != SpriteName)
                {
                    ChangeSprite();
                }
            }
        }

        private void ChangeSprite()
        {
            if (Atlas == null || string.IsNullOrEmpty(SpriteName))
            {
                sprite = null;
            }
            else
            {
                sprite = Atlas.GetSprite(SpriteName);
            }
        }

        protected override void OnPopulateMesh(VertexHelper toFill)
        {
            if (Atlas == null || string.IsNullOrEmpty(SpriteName))
            {
                toFill.Clear();
            }
            else
            {
                base.OnPopulateMesh(toFill);
            }
        }

#if UNITY_EDITOR
        protected override void OnValidate()
        {
            ChangeSprite();
            base.OnValidate();
        }
#endif
    }

}

