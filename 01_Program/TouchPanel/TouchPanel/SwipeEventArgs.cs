using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TouchPanel
{
    /// <summary>
    /// [クラス]
    /// スワイプイベントデータ
    /// </summary>
    public class SwipeEventArgs : EventArgs
    {
        /// <summary>
        /// [フィールド]
        /// スワイプ操作データ
        /// </summary>
        private SwipeData swipeData;

        /// <summary>
        /// [フィールド]
        /// 接触点の軌道
        /// </summary>
        private List<TouchPoint> orbit = new List<TouchPoint>();

        /// <summary>
        /// [プロパティ]
        /// スワイプ操作データ
        /// </summary>
        public SwipeData SwipeData
        {
            get { return this.swipeData; }
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
        /// <param name="swipeData">スワイプ操作データ</param>
        /// <param name="orbit">    接触点の軌道      </param>
        public SwipeEventArgs( SwipeData swipeData, List<TouchPoint> orbit )
        {
            this.swipeData = swipeData;
            this.orbit     = orbit;
        }
    }
}
