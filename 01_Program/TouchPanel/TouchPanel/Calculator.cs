using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TouchPanel
{
    /// <summary>
    /// [クラス]
    /// 接触点を使った計算を行うクラス
    /// </summary>
    internal class Calculator
    {
        /// <summary>
        /// [静的メソッド]
        /// 角度からスワイプの方向を決定する
        /// </summary>
        /// <returns>スワイプの方向</returns>
        internal static SwipeDirection AngleToDirection( int angle )
        {
            if ( -112 < angle && angle <= -67 )
            {
                return SwipeDirection.Up;
            }
            else if ( 67 < angle && angle <= 112 )
            {
                return SwipeDirection.Down;
            }
            else if ( -22 < angle && angle <= 22 )
            {
                return SwipeDirection.Right;
            }
            else if ( -67 < angle && angle <= -22 )
            {
                return SwipeDirection.UpperRight;
            }
            else if ( -157 < angle && angle <= 112 )
            {
                return SwipeDirection.UpperLeft;
            }
            else if ( 22 < angle && angle <= 67 )
            {
                return SwipeDirection.BottomRight;
            }
            else if ( 112 < angle && angle <= 157 )
            {
                return SwipeDirection.BottomLeft;
            }

            return SwipeDirection.Left;
        }

        /// <summary>
        /// [静的メソッド]
        /// ピンチ区分を取得する
        /// </summary>
        /// <returns>ピンチ区分</returns>
        internal static PinchType DistanceToPinchType( int baseDistance, int currentDistance )
        {
            if ( currentDistance < baseDistance )
            {
                return PinchType.PinchIn;
            }

            return PinchType.PinchOut;
        }

        /// <summary>
        /// [静的メソッド]
        /// 二点間の角度を計算する
        /// </summary>
        /// <param name="x1">地点 1 の 水平位置</param>
        /// <param name="y1">地点 1 の 垂直位置</param>
        /// <param name="x2">地点 2 の 水平位置</param>
        /// <param name="y2">地点 2 の 垂直位置</param>
        /// <returns>        角度              </returns>
        internal static int CalculateAngle( int x1, int y1, int x2, int y2 )
        {
            double radian = Math.Atan2( y2 - y1, x2 - x1 );
            return (int) ( radian * 180d / Math.PI );
        }

        /// <summary>
        /// [静的メソッド]
        /// 二点間の距離を計算する
        /// </summary>
        /// <param name="x1">地点 1 の 水平位置</param>
        /// <param name="y1">地点 1 の 垂直位置</param>
        /// <param name="x2">地点 2 の 水平位置</param>
        /// <param name="y2">地点 2 の 垂直位置</param>
        /// <returns>        距離              </returns>
        internal static int CalcurateDistance( int x1, int y1, int x2, int y2 )
        {
            double distance = Math.Sqrt( Math.Pow( x2 - x1, 2 ) + Math.Pow( y2 - y1, 2 ) );

            return Math.Abs( (int) distance );
        }
    }
}
