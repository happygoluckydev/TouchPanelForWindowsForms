using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using System.Reflection;

namespace TouchPanel.DerivedControl
{
    /// <summary>
    /// [クラス]
    /// タッチパネルコントロールを派生させた描画コントロール
    /// </summary>
    [ToolboxItem(true)]
    [Description("タッチパネルによる操作による描画を可能にするコントロール")]
    public partial class DrawPanel : PictureBox
    {
        /// <summary>
        /// [デリゲート]
        /// スワイプしたときのイベントハンドラ
        /// </summary>
        /// <param name="sender">イベントを発生させたオブジェクト</param>
        /// <param name="e">     イベントデータ                  </param>
        public delegate void SwipeEventHandler( object sender, SwipeEventArgs e );

        /// <summary>
        /// [デリゲート]
        /// ピンチしたときのイベントハンドラ
        /// </summary>
        /// <param name="sender">イベントを発生させたオブジェクト</param>
        /// <param name="e">     イベントデータ                  </param>
        public delegate void PinchEventHandler( object sender, PinchEventArgs e );

        /// <summary>
        /// [デリゲート]
        /// タッチイベントのイベントハンドラ
        /// </summary>
        /// <param name="sender">イベントを発生させたオブジェクト</param>
        /// <param name="e">     イベントデータ                  </param>
        public delegate void TouchEventHandler( object sender, TouchEventArgs e );

        /// <summary>
        /// [イベントハンドラ]
        /// スワイプイベントハンドラ
        /// </summary>
        public event SwipeEventHandler Swipe = null;

        /// <summary>
        /// [イベントハンドラ]
        /// ピンチイベントハンドラ
        /// </summary>
        public event PinchEventHandler Pinch = null;

        /// <summary>
        /// [イベントハンドラ]
        /// タップイベントハンドラ
        /// </summary>
        public event TouchEventHandler Tap = null;

        /// <summary>
        /// [イベントハンドラ]
        /// ダブルタップイベントハンドラ
        /// </summary>
        public event TouchEventHandler DoubleTap = null;

        /// <summary>
        /// [イベントハンドラ]
        /// タッチダウンイベント
        /// パネルへ接触したとき
        /// </summary>
        public event TouchEventHandler TouchDown = null;

        /// <summary>
        /// [イベントハンドラ]
        /// タッチムーブイベント
        /// タッチしながら動かしたとき
        /// </summary>
        public event TouchEventHandler TouchMove = null;

        /// <summary>
        /// [イベントハンドラ]
        /// タッチアップイベント
        /// タッチを離したとき
        /// </summary>
        public event TouchEventHandler TouchUp = null;

        /// <summary>
        /// [フィールド]
        /// 接触点の移動を認識するための間隔( 1/100 ピクセル )
        /// </summary>
        private int moveIntervalCentiPixel = 100;

        /// <summary>
        /// [フィールド]
        /// タップであると認識するためのタッチしてから離すまでの間隔( ミリ秒 )
        /// </summary>
        private int tapIntervalMilliseconds = 1000;

        /// <summary>
        /// [フィールド]
        /// ダブルタップであると認識するためのタップとタップの間隔( ミリ秒 )
        /// </summary>
        private int doubleTapIntervalMilliseconds = 1000;

        /// <summary>
        /// [フィールド]
        /// スワイプを検出するための移動距離の間隔( 1 / 100 ピクセル )
        /// </summary>
        private int swipeIntervalCentiPixel = 1000;

        /// <summary>
        /// [フィールド]
        /// ピンチを検出するための移動距離の間隔( 1 / 100 ピクセル )
        /// </summary>
        private int pinchIntervalCentiPixel = 1000;

        /// <summary>
        /// [フィールド]
        /// 接触開始した時間
        /// </summary>
        private DateTime touchDownTime = DateTime.Now.AddDays( -1 );

        /// <summary>
        /// [フィールド]
        /// 前回のタップ時間
        /// </summary>
        private DateTime previousTapTime = DateTime.Now.AddDays( -1 );

        /// <summary>
        /// [フィールド]
        /// ピンチ操作を行ったか
        /// </summary>
        private bool hasPinched = false;

        /// <summary>
        /// [フィールド]
        /// イベントの基準となる接触点
        /// </summary>
        private TouchPoint baseTouchPoint =new TouchPoint( -1, -1 );

        /// <summary>
        /// [フィールド]
        /// ピンチ操作の基準となる距離( 1/100 ピクセル )
        /// </summary>
        private int baseTwoTouchDistanceCentiPixel = -1;

        /// <summary>
        /// [フィールド]
        /// タッチしてから離れるまでの軌道
        /// </summary>
        private List<TouchPoint> orbit = new List<TouchPoint>();

        /// <summary>
        /// [フィールド]
        /// 線の色
        /// </summary>
        private Color penColor = Color.Salmon;

        /// <summary>
        /// [フィールド]
        /// 線の太さ
        /// </summary>
        private int penWidth = 5;

        /// <summary>
        /// [フィールド]
        /// タッチ入力による描画を許可するか
        /// </summary>
        private bool allowTouchDraw = false;

        /// <summary>
        /// [プロパティ]
        /// 接触点の移動を認識するための間隔( 1/100 ピクセル )
        /// </summary>
        [Description( "接触点の移動を認識するための間隔( 1/100 ピクセル )" )]
        public int MoveIntervalCentiPixel
        {
            set { moveIntervalCentiPixel = value; }
            get { return moveIntervalCentiPixel; }
        }

        /// <summary>
        /// [プロパティ]
        /// タップであると認識するためのタッチしてから離すまでの間隔( ミリ秒 )
        /// </summary>
        [Description( "タップであると認識するためのタッチしてから離すまでの間隔( ミリ秒 )" )]
        public int TapIntervalMilliseconds
        {
            set { tapIntervalMilliseconds = value; }
            get { return tapIntervalMilliseconds; }
        }

        /// <summary>
        /// [プロパティ]
        /// ダブルタップであると認識するためのタップとタップの間隔( ミリ秒 )
        /// </summary>
        [Description( "ダブルタップであると認識するためのタップとタップの間隔( ミリ秒 )" )]
        public int DoubleTapIntervalMilliseconds
        {
            set { doubleTapIntervalMilliseconds = value; }
            get { return doubleTapIntervalMilliseconds; }
        }

        /// <summary>
        /// [プロパティ]
        /// スワイプを検出するための移動距離の間隔( 1 / 100 ピクセル )
        /// </summary>
        [Description( "スワイプを検出するための移動距離の間隔( 1 / 100 ピクセル )" )]
        public int SwipeIntervalCentiPixel
        {
            set { swipeIntervalCentiPixel = value; }
            get { return swipeIntervalCentiPixel; }
        }

        /// <summary>
        /// [プロパティ]
        /// 線の色
        /// </summary>
        [Description( "タッチ入力で描画する線の色" )]
        public Color PenColor
        {
            set { this.penColor = value; }
            get { return this.penColor; }
        }

        /// <summary>
        /// [プロパティ]
        /// 線の太さ
        /// </summary>
        [Description( "タッチ入力で描画する線の幅" )]
        public int PenWidth
        {
            set { this.penWidth = value; }
            get { return this.penWidth; }
        }

        /// <summary>
        /// [プロパティ]
        /// タッチ入力による描画を許可するか
        /// </summary>
        [Description( "タッチ入力による描画を許可するか" )]
        public bool AllowTouchDraw
        {
            set { this.allowTouchDraw = value; }
            get { return this.allowTouchDraw; }
        }

        /// <summary>
        /// [コンストラクタ]
        /// </summary>
        public DrawPanel()
        {
            // ウィンドウをタッチ入力に対応させる
            Win32ApiWrapper.RegisterTouchWindow( this.Handle, 0 );

            // ちらつきを防止する
            this.DoubleBuffered = true;

            // 軌道を初期化する
            this.orbit = new List<TouchPoint>( this.Size.Height + this.Size.Width );

            TouchMove += new TouchEventHandler( drowWhenTouchMove );
        }

        /// <summary>
        /// [メソッド]
        /// Windows メッセージを処理します。
        /// </summary>
        /// <param name="m">Windowsメッセージ</param>
        protected override void WndProc( ref Message m )
        {
            // WMthis.TOUCHのメッセージハンドラを実装する
            if ( m.Msg == Win32ApiWrapper.WM_TOUCH )
            {
                Win32ApiWrapper.TouchEventData[] eventData = Win32ApiWrapper.GetTouchEventData( m );
                
                // 基本イベントを割り当てる
                allocateBasicEvent( eventData[ 0 ] );

                // 排他的イベントを割り当てる
                allocateExclusiveEvent( eventData );

                // 背面のコントロールを呼び出す
                var  touchPoint = new TouchPoint( eventData[ 0 ].x, eventData[ 0 ].y );
                bool isCalled   = callBackControl( touchPoint, m );
                if ( isCalled )
                {
                    return;
                }
            }

            base.WndProc( ref m );
        }

        /// <summary>
        /// [内部メソッド]
        /// 背面のコントロールを呼び出す
        /// </summary>
        private bool callBackControl( TouchPoint touchPoint, Message winMessage )
        {
            foreach ( Control control in Parent.Controls )
            {
                if ( control.Visible == false || control.Enabled == false )
                {
                    continue;
                }
                
                // 接触点がコントロールの位置か判別する
                var  rect = new Rectangle( touchPoint.X, touchPoint.Y, 1, 1 );
                bool isIntersects = control.Bounds.IntersectsWith( rect );
                if ( isIntersects == false )
                {
                    continue;
                }

                // 対象のコントロールのWindowsメッセージ処理メソッドを呼び出す
                MethodInfo mInfo = control.GetType().GetMethod( "WndProc" );
                var        args  = new object[] { winMessage };
                mInfo.Invoke( control, args );
                return true;
            }

            return false;
        }

        /// <summary>
        /// [内部メソッド]
        /// 基本イベントを割り当てる
        /// </summary>
        /// <param name="eventData">イベントデータ</param>
        private void allocateBasicEvent( Win32ApiWrapper.TouchEventData eventData )
        {
            // タッチイベントフラグを取得する
            Win32ApiWrapper.TouchEventFlags eventFlags = eventData.dwEventFlags;

            if ( eventFlags.IsDown() )
            {
                executeDownEvent( eventData );
            }
            else if ( eventFlags.IsMove() )
            {
                executeMoveEvent( eventData );
            }
            else if ( eventFlags.IsUp() )
            {
                executeUpEvent( eventData );
            }
        }

        /// <summary>
        /// [内部メソッド]
        /// DOWN ベントを実行する
        /// </summary>
        /// <param name="eventData">イベントデータ</param>
        private void executeDownEvent( Win32ApiWrapper.TouchEventData eventData )
        {
            // タッチ操作のデータを初期化する
            resetFields();

            // タッチの基準点を保持しておく
            this.touchDownTime = DateTime.Now;
            this.baseTouchPoint = new TouchPoint( eventData.x, eventData.y );

            // 接触点を軌道に追加する
            this.orbit.Add( this.baseTouchPoint );

            // 「タッチダウン」イベントハンドラを実行する
            var teArgs = new TouchEventArgs( this.baseTouchPoint, this.orbit );
            OnTouchDown( teArgs );
        }

        /// <summary>
        /// [内部メソッド]
        /// MOVE イベントを実行する
        /// </summary>
        /// <param name="eventData">イベントデータ</param>
        private void executeMoveEvent( Win32ApiWrapper.TouchEventData eventData )
        {
            // 移動距離が規定の間隔に満たない場合は処理しない
            TouchPoint lastTouchPoint = this.orbit.Last();
            int        distance       = Calculator.CalcurateDistance( lastTouchPoint.X, lastTouchPoint.Y, eventData.x, eventData.y );
            if ( distance < this.moveIntervalCentiPixel )
            {
                return;
            }

            //接触点を軌道に追加する
            var touchPoint = new TouchPoint( eventData.x, eventData.y );
            this.orbit.Add( touchPoint );

            // 「タッチムーブ」イベントハンドラを実行する
            var teArgs = new TouchEventArgs( touchPoint, this.orbit );
            OnTouchMove( teArgs );
        }
        
        /// <summary>
        /// [内部メソッド]
        /// UP イベントデータを実行する
        /// </summary>
        /// <param name="eventData">イベントデータ</param>
        private void executeUpEvent(Win32ApiWrapper.TouchEventData eventData )
        {
            // 接触点を軌道に追加する
            var touchPoint = new TouchPoint( eventData.x, eventData.y );
            this.orbit.Add( touchPoint );

            // 「タッチアップ」イベントハンドラを実行する
            var teArgs     = new TouchEventArgs( touchPoint, this.orbit );
            OnTouchUp( teArgs );
        }

        /// <summary>
        /// [内部メソッド]
        /// 排他的イベントを割り当てる
        /// </summary>
        private void allocateExclusiveEvent( Win32ApiWrapper.TouchEventData[] eventData )
        {
            // タッチイベントフラグを取得する
            Win32ApiWrapper.TouchEventFlags eventFlags = eventData[ 0 ].dwEventFlags;

            // 接触点の座標情報を設定する
            TouchPoint touchPoint1 = new TouchPoint( eventData[ 0 ].x, eventData[ 0 ].y );
            TouchPoint touchPoint2 = null;
            if ( eventData.Length > 1 )
            {
                touchPoint2 = new TouchPoint( eventData[ 1 ].x, eventData[ 1 ].y );
            }

            // 各イベントを割り当てる
            if ( eventData.Length > 1 && eventData[ 1 ].dwEventFlags.IsDown() )
            {
                // 「ピンチ」操作の開始を検知する
                this.baseTwoTouchDistanceCentiPixel = Calculator.CalcurateDistance( touchPoint1.X, touchPoint1.Y, touchPoint2.X, touchPoint2.Y );
            }
            else if ( eventData.Length > 1 && eventData[ 1 ].dwEventFlags.IsMove() )
            {
                // 「ピンチ」イベントを実行する
                executePinchEvent( touchPoint1, touchPoint2 );
            }
            else if ( eventFlags.IsUp() )
            {
                if ( isSwipe( touchPoint1 ) )
                {
                    // 「スワイプ」イベントを実行する
                    executeSwipeEvent( touchPoint1 );
                }
                else if ( isDoubleTap() )
                {
                    // 「ダブルタップ」イベントを実行する
                    executeDoubleTapEvent( touchPoint1 );
                }
                else if ( isTap() )
                {
                    // 「タップ」イベントを実行する
                    executeTapEvent( touchPoint1 );
                }
            }
        }

        /// <summary>
        /// [内部メソッド]
        /// 「ピンチ」イベントを実行する
        /// </summary>
        /// <param name="touchPoint1">接触点1</param>
        /// <param name="touchPoint2">接触点2</param>
        private void executePinchEvent( TouchPoint touchPoint1, TouchPoint touchPoint2 )
        {
            // 接触点1と接触点2の距離から「ピンチ」イベントハンドラを実行するか
            int distance = Calculator.CalcurateDistance( touchPoint1.X, touchPoint1.Y, touchPoint2.X, touchPoint2.Y );
            if ( Math.Abs( this.baseTwoTouchDistanceCentiPixel - distance ) < pinchIntervalCentiPixel )
            {
                return;
            }

            // 接触点1と接触点2の角度を計算する
            int angle = Calculator.CalculateAngle( touchPoint1.X, touchPoint1.Y, touchPoint2.X, touchPoint2.Y );
            
            // 基準となる距離と接触点1,2の距離からピンチ区分を決定する
            PinchType type = Calculator.DistanceToPinchType( this.baseTwoTouchDistanceCentiPixel, distance );
            
            // 「ピンチ」イベントハンドラを実行する
            var pinchData = new PinchData( touchPoint1, touchPoint2, angle, this.baseTwoTouchDistanceCentiPixel, distance, type );
            var peArgs    = new PinchEventArgs( pinchData );
            OnPinch( peArgs );
            this.baseTwoTouchDistanceCentiPixel = distance;
        }

        /// <summary>
        /// [内部メソッド]
        /// 「スワイプ」イベントを実行する
        /// </summary>
        /// <param name="touchPoint">接触点</param>
        private void executeSwipeEvent( TouchPoint touchPoint )
        {
            // 基準点と接触点の距離を取得する
            int distance = touchPoint.GetDistanceFrom( this.baseTouchPoint );

            // 基準点と接触点の角度を計算する
            int angle = Calculator.CalculateAngle( this.baseTouchPoint.X, this.baseTouchPoint.Y, touchPoint.X, touchPoint.Y );
            
            // 角度からスワイプの方向を決定する
            SwipeDirection direction = Calculator.AngleToDirection( angle );
            
            // 「スワイプ」イベントハンドラを実行する
            var swipeData = new SwipeData( this.baseTouchPoint, touchPoint, angle, distance, direction );
            var seArgs    = new SwipeEventArgs( swipeData, this.orbit );
            OnSwipe( touchPoint, seArgs );
        }

        /// <summary>
        /// [内部メソッド]
        /// 「ダブルタップ」イベントを実行する
        /// </summary>
        private void executeDoubleTapEvent( TouchPoint touchPoint )
        {
            // 「ダブルタップ」イベントハンドラを実行する
            var teArgs = new TouchEventArgs( touchPoint, this.orbit );
            OnDoubleTap( teArgs );
        }

        /// <summary>
        /// [内部メソッド]
        /// 「タップ」イベントを実行する
        /// </summary>
        /// <param name="touchPoint"></param>
        private void executeTapEvent( TouchPoint touchPoint )
        {
            this.previousTapTime = DateTime.Now;
            var teArgs = new TouchEventArgs( touchPoint, this.orbit );
            OnTap( teArgs );
        }

        /// <summary>
        /// [メソッド]
        /// スワイプイベントハンドラを実行する
        /// </summary>
        /// <param name="e">イベントデータ</param>
        protected virtual void OnSwipe( TouchPoint lastTouchPoint, SwipeEventArgs e )
        {
            if ( Swipe != null )
            {
                Swipe( this, e );
            }
        }

        /// <summary>
        /// [メソッド]
        /// ピンチイベントハンドラを実行する
        /// </summary>
        /// <param name="e">イベントデータ</param>
        protected virtual void OnPinch( PinchEventArgs e )
        {
            this.hasPinched = true;

            if ( Pinch != null )
            {
                Pinch( this, e );
            }
        }

        /// <summary>
        /// [メソッド]
        /// タップイベントハンドラを実行する
        /// </summary>
        /// <param name="e">イベントデータ</param>
        protected virtual void OnTap( TouchEventArgs e )
        {
            if ( Tap != null )
            {
                Tap( this, e );
            }
        }

        /// <summary>
        /// [メソッド]
        /// ダブルタップイベントハンドラを実行する
        /// </summary>
        /// <param name="e">イベントデータ</param>
        protected virtual void OnDoubleTap( TouchEventArgs e )
        {
            if ( DoubleTap != null )
            {
                DoubleTap( this, e );
            }
        }

        /// <summary>
        /// [メソッド]
        /// タッチダウンイベントハンドラを実行する
        /// </summary>
        /// <param name="e">イベントデータ</param>
        protected virtual void OnTouchDown( TouchEventArgs e )
        {
            if ( TouchDown != null )
            {
                TouchDown( this, e );
            }
        }

        /// <summary>
        /// [メソッド]
        /// タッチムーブイベントハンドラを実行する
        /// </summary>
        /// <param name="e">イベントデータ</param>
        protected virtual void OnTouchMove( TouchEventArgs e )
        {
            if ( TouchMove != null )
            {
                TouchMove( this, e );
            }
        }

        /// <summary>
        /// [メソッド]
        /// タッチアップイベントハンドラを実行する
        /// </summary>
        /// <param name="e">イベントデータ</param>
        protected virtual void OnTouchUp( TouchEventArgs e )
        {
            if ( TouchUp != null )
            {
                TouchUp( this, e );
            }
        }

        /// <summary>
        /// [内部メソッド]
        /// 「スワイプ」操作か
        /// </summary>
        /// <returns>「スワイプ」操作か</returns>
        private bool isSwipe( TouchPoint lastTouchPoint )
        {
            if ( this.baseTouchPoint.X < 0 || this.baseTouchPoint.Y < 0 )
            {
                return false;
            }

            if ( lastTouchPoint.X < 0 || lastTouchPoint.Y < 0 )
            {
                return false;
            }

            if ( this.hasPinched )
            {
                return false;
            }

            int distance = lastTouchPoint.GetDistanceFrom( this.baseTouchPoint );

            return distance > swipeIntervalCentiPixel;
        }

        /// <summary>
        /// [内部メソッド]
        /// 「ダブルタップ」操作か
        /// </summary>
        /// <returns>「ダブルタップ」操作か</returns>
        private bool isDoubleTap()
        {
            TimeSpan tapSpan   = DateTime.Now - this.previousTapTime;

            if( isTap() == false ){

                return false;
            }

            if ( tapSpan.TotalMilliseconds > this.doubleTapIntervalMilliseconds )
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// [内部メソッド]
        /// 「タップ」操作か
        /// </summary>
        /// <returns>「タップ」操作か</returns>
        private bool isTap()
        {
            // 接触してから離すまでの時間を計算する
            TimeSpan touchSpan = DateTime.Now - this.touchDownTime;

            return touchSpan.TotalMilliseconds < this.tapIntervalMilliseconds;
        }

        /// <summary>
        /// [内部メソッド]
        /// フィールドを初期化する
        /// </summary>
        private void resetFields()
        {
            this.baseTwoTouchDistanceCentiPixel = -1;
            this.baseTouchPoint = new TouchPoint( -1, -1 );
            this.orbit          = new List<TouchPoint>( this.Size.Height + this.Size.Width );
            this.hasPinched     = false;
        }

        /// <summary>
        /// [イベントハンドラ]
        /// DrawPanel上でタッチ操作を行ったとき
        /// </summary>
        /// <param name="sender">イベントを発生させたオブジェクト</param>
        /// <param name="e">     イベントデータ                  </param>
        private void drowWhenTouchMove( object sender, TouchEventArgs e )
        {
            if ( allowTouchDraw == false )
            {
                return;
            }
            
            // 直近の接触点と一つ前の接触点を取得する
            int orbitLength = e.Orbit.Count;
            int absX1 = e.Orbit[ orbitLength - 2 ].XforPixel;
            int absY1 = e.Orbit[ orbitLength - 2 ].YforPixel;
            int absX2 = e.Orbit[ orbitLength - 1 ].XforPixel;
            int absY2 = e.Orbit[ orbitLength - 1 ].YforPixel;

            Point point1 = this.PointToClient( new Point( absX1, absY1 ) );
            Point point2 = this.PointToClient( new Point( absX2, absY2 ) );

            // 線を描く
            drawLine( point1, point2 );

            // 再描画する
            this.Refresh();
        }

        /// <summary>
        /// [内部メソッド]
        /// 線を描く
        /// </summary>
        /// <param name="Point1">開始地点</param>
        /// <param name="Point2">終了地点</param>
        private void drawLine( Point point1, Point point2 )
        {
            // 描画領域のBitmapデータを取得する
            var canvas = new Bitmap( this.Image, this.Width, this.Height );
            Graphics g = Graphics.FromImage( canvas );

            // 線を描くペンの設定を行う( 色, 太さ )
            var pen    = new Pen( this.PenColor, this.PenWidth );

            // Bitmapデータに線を描く
            g.DrawLine( pen, point1, point2 );
            g.Dispose();

            // 描画領域に線を描いたBitmapデータを設定する
            this.Image = canvas;
        }
    }
}
