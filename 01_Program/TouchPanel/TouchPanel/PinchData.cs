using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Drawing;

namespace TouchPanel
{
    /// <summary>
    /// [構造体]
    /// ピンチ操作データ
    /// </summary>
    public struct PinchData
    {
        /// <summary>
        /// [フィールド]
        /// 接触点 1
        /// </summary>
        private TouchPoint touchPoint1;

        /// <summary>
        /// [フィールド]
        /// 接触点 2
        /// </summary>
        private TouchPoint touchPoint2;

        /// <summary>
        /// [フィールド]
        /// 接触点 1 から見た 接触点 2 の角度
        /// 右を基準( 0 )に上方向に負の値( 真上は -90 )、下方向に正の値( 真下は 90 )
        /// </summary>
        private int angle;

        /// <summary>
        /// [フィールド]
        /// 基準となる距離( 1/100 ピクセル )
        /// </summary>
        private int baseDistance;

        /// <summary>
        /// [フィールド]
        /// 接触点 1 と 2 の距離( 1/100 ピクセル )
        /// </summary>
        private int currentDistance;

        /// <summary>
        /// [フィールド]
        /// ピンチ区分
        /// </summary>
        private PinchType pinchType;

        /// <summary>
        /// [プロパティ]
        /// 接触点 1
        /// </summary>
        private TouchPoint TouchPoint1
        {
            get { return this.touchPoint1; }
        }

        /// <summary>
        /// [プロパティ]
        /// 接触点 2
        /// </summary>
        private TouchPoint TouchPoint2
        {
            get { return this.touchPoint2; }
        }
        
        /// <summary>
        /// [プロパティ]
        /// 接触点 1 から見た 接触点 2 の角度
        /// 右を基準( 0 )に上方向に負の値( 真上は -90 )、下方向に正の値( 真下は 90 )
        /// </summary>
        public int Angle
        {
            get { return this.angle; }
        }

        /// <summary>
        /// [プロパティ]
        /// 基準となる距離( 1/100 ピクセル )
        /// </summary>
        public int BaseDistance
        {
            get { return this.baseDistance; }
        }

        /// <summary>
        /// [プロパティ]
        /// 接触点 1 と 2 の距離( 1/100 ピクセル )
        /// </summary>
        public int CurrentDistance
        {
            get { return this.currentDistance; }
        }

        /// <summary>
        /// [プロパティ]
        /// 基準となる距離と現在の距離の差分( 1/100 ピクセル )
        /// </summary>
        public int VariationValue
        {
            get { return this.currentDistance - this.baseDistance; }
        }

        /// <summary>
        /// [プロパティ]
        /// ピンチ区分
        /// </summary>
        public PinchType PinchType
        {
            get { return this.pinchType; }
        }

        /// <summary>
        /// [コンストラクタ]
        /// </summary>
        /// <param name="touchPoint1">    接触点 1                         </param>
        /// <param name="touchPoint2">    接触点 2                         </param>
        /// <param name="angle">          接触点 1 から見た 接触点 2 の角度</param>
        /// <param name="baseDistance">   基準となる距離( 1/100 ピクセル ) </param>
        /// <param name="currentDistance">現在の距離( 1/100 ピクセル )     </param>
        /// <param name="pinchType">      ピンチ区分                       </param>
        public PinchData( TouchPoint touchPoint1, TouchPoint touchPoint2, int angle, int baseDistance, int currentDistance, PinchType pinchType )
        {
            this.touchPoint1     = touchPoint1;
            this.touchPoint2     = touchPoint2;
            this.angle           = angle;
            this.baseDistance    = baseDistance;
            this.currentDistance = currentDistance;
            this.pinchType       = pinchType;
        }
    }
}
