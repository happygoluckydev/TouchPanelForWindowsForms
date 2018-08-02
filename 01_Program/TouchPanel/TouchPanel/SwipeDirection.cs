using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TouchPanel
{
    /// <summary>
    /// [列挙型]
    /// スワイプの方向
    /// </summary>
    public enum SwipeDirection
    {
        /// <summary>
        /// 上
        /// </summary>
        Up,

        /// <summary>
        /// 下
        /// </summary>
        Down,

        /// <summary>
        /// 右
        /// </summary>
        Right,

        /// <summary>
        /// 左
        /// </summary>
        Left,

        /// <summary>
        /// 右上
        /// </summary>
        UpperRight,
        
        /// <summary>
        /// 左上
        /// </summary>
        UpperLeft,

        /// <summary>
        /// 右下
        /// </summary>
        BottomRight,
        
        /// <summary>
        /// 左下
        /// </summary>
        BottomLeft
    }
}
