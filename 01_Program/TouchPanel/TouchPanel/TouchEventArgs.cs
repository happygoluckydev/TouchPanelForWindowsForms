using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TouchPanel
{
    /// <summary>
    /// [クラス]
    /// タッチイベントデータ
    /// </summary>
    public class TouchEventArgs : EventArgs
    {
        /// <summary>
        /// [フィールド]
        /// 接触点
        /// </summary>
        private TouchPoint touchPoint = new TouchPoint( -1, -1 );

        /// <summary>
        /// [フィールド]
        /// 接触点の軌道
        /// </summary>
        private List<TouchPoint> orbit = new List<TouchPoint>();

        /// <summary>
        /// [プロパティ]
        /// 接触点
        /// </summary>
        public TouchPoint TouchPoint
        {
            get { return this.touchPoint; }
        }

        /// <summary>
        /// [プロパティ]
        /// 接触点の軌道
        /// </summary>
        public List<TouchPoint> Orbit
        {
            get { return this.orbit; }
        }

        /// <summary>
        /// [コンストラクタ]
        /// </summary>
        /// <param name="touchPoint">接触点</param>
        public TouchEventArgs( TouchPoint touchPoint, List<TouchPoint> orbit )
        {
            this.touchPoint = touchPoint;
            this.orbit      = orbit;
        }
    }
}
