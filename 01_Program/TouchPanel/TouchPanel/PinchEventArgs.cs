using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TouchPanel
{
    /// <summary>
    /// [クラス]
    /// ピンチイベントデータ
    /// </summary>
    public class PinchEventArgs : EventArgs
    {
        /// <summary>
        /// [フィールド]
        /// ピンチ操作データ
        /// </summary>
        private PinchData pinchData;

        /// <summary>
        /// [プロパティ]
        /// ピンチ操作データ
        /// </summary>
        public PinchData PinchData
        {
            get { return this.pinchData; }
        }

        /// <summary>
        /// [コンストラクタ]
        /// </summary>
        /// <param name="pinchData">ピンチ操作データ</param>
        public PinchEventArgs( PinchData pinchData )
        {
            this.pinchData = pinchData;
        }
    }
}
