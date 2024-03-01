using System;

namespace Core
{
    /// <summary> UI层级特性描述，用于标记UI层级 </summary>
    public class UILayerAttribute : Attribute
    {
        //只读的层级信息
        public UIPanelLayer layer { get; }

        /// <summary> 指定面板层级 </summary>
        /// <param name="layer"></param>
        public UILayerAttribute(UIPanelLayer layer)
        {
            this.layer = layer;
        }
    }
}