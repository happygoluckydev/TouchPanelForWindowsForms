using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TouchPanel
{
    /// <summary>
    /// [クラス]
    /// 接触点
    /// </summary>
    public class TouchPoint
    {
        /// <summary>
        /// [フィールド]
        /// 接触点の水平位置( 1 / 100 ピクセル )
        /// </summary>
        int x;

        /// <summary>
        /// [フィールド]
        /// 接触点の垂直位置( 1 / 100 ピクセル )
        /// </summary>
        int y;

        /// <summary>
        /// [プロパティ]
        /// 接触点の水平位置( 1 / 100 ピクセル )
        /// </summary>
        public int X
        {
            get { return this.x; }
        }

        /// <summary>
        /// [プロパティ]
        /// 接触点の垂直位置( 1 / 100 ピクセル )
        /// </summary>
        public int Y
        {
            get { return this.y; }
        }

        /// <summary>
        /// [プロパティ]
        /// 接触点の水平位置( ピクセル )
        /// </summary>
        public int XforPixel
        {
            get { return this.x / 100; }
        }

        /// <summary>
        /// [プロパティ]
        /// 接触点の垂直位置( ピクセル )
        /// </summary>
        public int YforPixel
        {
            get { return this.y / 100; }
        }

        /// <summary>
        /// [コンストラクタ]
        /// </summary>
        /// <param name="x">接触点の水平位置( 1 / 100 ピクセル )</param>
        /// <param name="y">接触点の垂直位置( 1 / 100 ピクセル )</param>
        public TouchPoint( int x, int y )
        {
            this.x = x;
            this.y = y;
        }

        /// <summary>
        /// [内部メソッド]
        /// 距離を取得する( 1 / 100 ピクセル )
        /// </summary>
        /// <param name="touchPoint">基準となる接触点</param>
        /// <returns>                基準点との距離  </returns>
        public int GetDistanceFrom( TouchPoint baseTouchPoint )
        {
            return Calculator.CalcurateDistance( baseTouchPoint.X, baseTouchPoint.Y, this.X, this.Y );
        }
    }
}
