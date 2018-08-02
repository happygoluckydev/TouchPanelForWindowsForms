using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Drawing;

namespace TouchPanel
{
    /// <summary>
    /// [構造体]
    /// スワイプ操作データ
    /// </summary>
    public struct SwipeData
    {
        /// <summary>
        /// [フィールド]
        /// スワイプを始めた接触点
        /// </summary>
        private TouchPoint firstTouchPoint;

        /// <summary>
        /// [フィールド]
        /// スワイプを終えた接触点
        /// </summary>
        private TouchPoint lastTouchPoint;
        
        /// <summary>
        /// [フィールド]
        /// 開始地点からみた終了地点の角度
        /// 右を基準( 0 )に上方向に負の値( 真上は -90 )、下方向に正の値( 真下は 90 )
        /// </summary>
        private int angle;
        /// <summary>
        /// [フィールド]
        /// スワイプの方向
        /// </summary>
        private int distance;

        /// <summary>
        /// [フィールド]
        /// 開始地点と終了地点の距離( 1/100 ピクセル )
        /// </summary>
        private SwipeDirection direction;

        /// <summary>
        /// [プロパティ]
        /// スワイプを始めた接触点
        /// </summary>
        public TouchPoint FirstTouchPoint
        {
            get { return this.firstTouchPoint; }
        }

        /// <summary>
        /// [プロパティ]
        /// スワイプを終えた接触点
        /// </summary>
        public TouchPoint LastTouchPoint
        {
            get { return this.lastTouchPoint; }
        }

        /// <summary>
        /// [プロパティ]
        /// 開始地点からみた終了地点の角度
        /// 右を基準( 0 )に上方向に負の値( 真上は -90 )、下方向に正の値( 真下は 90 )
        /// </summary>
        public int Angle
        {
            get { return this.angle; }
        }

        /// <summary>
        /// [プロパティ]
        /// 開始地点と終了地点の距離( 1/100 ピクセル )
        /// </summary>
        public int Distance
        {
            get { return this.distance; }
        }

        /// <summary>
        /// [プロパティ]
        /// スワイプの方向
        /// </summary>
        public SwipeDirection Direction
        {
            get { return this.direction; }
        }

        /// <summary>
        /// [コンストラクタ]
        /// </summary>
        /// <param name="firstTouchPoint">スワイプを始めた接触点               </param>
        /// <param name="lastTouchPoint"> スワイプを始めた接触点               </param>
        /// <param name="angle">          開始地点からみた終了地点の角度       </param>
        /// <param name="currentDistance"> 開始地点と終了地点の距離( ピクセル )</param>
        /// <param name="direction">      スワイプの方向                       </param>
        public SwipeData( TouchPoint firstTouchPoint, TouchPoint lastTouchPoint, int angle, int distance, SwipeDirection direction )
        {
            this.firstTouchPoint = firstTouchPoint;
            this.lastTouchPoint  = lastTouchPoint;
            this.angle           = angle;
            this.distance        = distance;
            this.direction   　  = direction;
        }
    }
}
