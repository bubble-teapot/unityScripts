using System;

namespace Core
{
    /// <summary> UI�㼶�������������ڱ��UI�㼶 </summary>
    public class UILayerAttribute : Attribute
    {
        //ֻ���Ĳ㼶��Ϣ
        public UIPanelLayer layer { get; }

        /// <summary> ָ�����㼶 </summary>
        /// <param name="layer"></param>
        public UILayerAttribute(UIPanelLayer layer)
        {
            this.layer = layer;
        }
    }
}