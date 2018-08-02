using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace TouchPanel
{
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
        /// [定数]
        /// ピンチを検出するための移動距離の間隔( 1 / 100 ピクセル )
        /// </summary>
        public static readonly int PinchInterval = 1000;

        /// <summary>
        /// [定数]
        /// スワイプを検出するための移動距離の間隔( 1 / 100 ピクセル )
        /// </summary>
        public static readonly int SwipeInterval = 1000;

        /// <summary>
        /// [フィールド]
        /// ピンチ操作を行ったか
        /// </summary>
        private bool _hasPinched = false;

        /// <summary>
        /// [フィールド]
        /// イベントの基準となる接触点
        /// </summary>
        private TouchPoint _baseTouchPoint =new TouchPoint( -1, -1 );

        /// <summary>
        /// [フィールド]
        /// イベントの基準となる距離( 1/100 ピクセル )
        /// </summary>
        private int _baseTwoTouchDistance = -1;

        /// <summary>
        /// [フィールド]
        /// タップであると認識するためのタッチしてから離すまでの間隔( ミリ秒 )
        /// </summary>
        private int _tapInterval = 1000;

        /// <summary>
        /// [フィールド]
        /// ダブルタップであると認識するためのタップとタップの間隔( ミリ秒 )
        /// </summary>
        private int _doubleTapInterval = 1000;

        /// <summary>
        /// [フィールド]
        /// 接触点の移動を認識するための間隔( 1/100 ピクセル )
        /// </summary>
        private int _moveInterval = 100;

        /// <summary>
        /// [フィールド]
        /// 接触開始した時間
        /// </summary>
        private DateTime _touchDownTime = DateTime.Now.AddDays( -1 );

        /// <summary>
        /// [フィールド]
        /// 前回のタップ時間
        /// </summary>
        private DateTime _previousTapTime = DateTime.Now.AddDays( -1 );

        /// <summary>
        /// [フィールド]
        /// タッチしてから離れるまでの軌道
        /// </summary>
        private List<TouchPoint> _orbit = new List<TouchPoint>();
        
        /// <summary>
        /// [コンストラクタ]
        /// </summary>
        public DrawPanel()
        {
            // ウィンドウをタッチ入力に対応させる
            Win32ApiWrapper.RegisterTouchWindow( this.Handle, 0 );

            // 透明の背景色を有効にする
            enableTransparentBackColor();
            
            // ちらつき防止
            this.DoubleBuffered = true;

            // 軌道を初期化する
            _orbit = new List<TouchPoint>( this.Size.Height + this.Size.Width );
        }

        /// <summary>
        /// [内部メソッド]
        /// 透明の背景色を有効にする
        /// </summary>
        private void enableTransparentBackColor()
        {
            // コントロールを独自描画する
            SetStyle( ControlStyles.UserPaint, true );

            // 透明色を有効にする
            SetStyle( ControlStyles.SupportsTransparentBackColor, true );

            // 背景の自動描画を無効にする
            SetStyle( ControlStyles.Opaque, true );

            // バッファへの描画をオフにする
            // false に設定しないと意図した通りの描画にならない
            SetStyle( ControlStyles.OptimizedDoubleBuffer, false );
        }

        /// <summary>
        /// [メソッド]
        /// Windows メッセージを処理します。
        /// </summary>
        /// <param name="m">Windowsメッセージ</param>
        protected override void WndProc( ref Message m )
        {
            // WM_TOUCHのメッセージハンドラを実装する
            if ( m.Msg == Win32ApiWrapper.WM_TOUCH )
            {
                Win32ApiWrapper.TouchEventData[] eventData = Win32ApiWrapper.GetTouchEventData( m );
                
                // 基本イベントを割り当てる
                allocateBasicEvent( eventData[ 0 ] );

                // 排他的イベントを割り当てる
                allocateExclusiveEvent( eventData );
            }

            base.WndProc( ref m );
        }

        /// <summary>
        /// [イベントハンドラ]
        /// 描画するとき
        /// </summary>
        /// <param name="e">イベントデータ</param>
        protected override void OnPaint( PaintEventArgs e )
        {
            var sBrush = new SolidBrush( this.BackColor );
            e.Graphics.FillRectangle( sBrush, e.Graphics.VisibleClipBounds ); 
            base.OnPaint( e );
        }

        /// <summary>
        /// [内部メソッド]
        /// 基本イベントを割り当てる
        /// </summary>
        /// <param name="eventData">イベントデータ</param>
        private void allocateBasicEvent( Win32ApiWrapper.TouchEventData eventData )
        {
            Win32ApiWrapper.TouchEventFlags eventFlags = eventData.dwEventFlags;

            if ( ( eventFlags & Win32ApiWrapper.TouchEventFlags.DOWN ) == Win32ApiWrapper.TouchEventFlags.DOWN )
            {
                // 「タッチダウン」イベントハンドラを実行する
                resetFields();
                _touchDownTime  = DateTime.Now;
                _baseTouchPoint = new TouchPoint( eventData.x, eventData.y );
                var teArgs      = new TouchEventArgs( _baseTouchPoint, _orbit );
                _orbit.Add( _baseTouchPoint );
                OnTouchDown( teArgs );
            }
            else if ( ( eventFlags & Win32ApiWrapper.TouchEventFlags.MOVE ) == Win32ApiWrapper.TouchEventFlags.MOVE )
            {
                // 移動距離が規定の間隔に満たない場合は処理しない
                TouchPoint lastTouchPoint = _orbit.Last();
                int        distance       = Calculator.CalcurateDistance( lastTouchPoint.X, lastTouchPoint.Y, eventData.x, eventData.y );
                if ( distance < _moveInterval )
                {
                    return;
                }

                // 「タッチムーブ」イベントハンドラを実行する
                var touchPoint = new TouchPoint( eventData.x, eventData.y );
                var teArgs     = new TouchEventArgs( touchPoint, _orbit );
                _orbit.Add( touchPoint );
                OnTouchMove( teArgs );
            }
            else if ( ( eventFlags & Win32ApiWrapper.TouchEventFlags.UP ) == Win32ApiWrapper.TouchEventFlags.UP )
            {
                // 「タッチアップ」イベントハンドラを実行する
                var touchPoint = new TouchPoint( eventData.x, eventData.y );
                var teArgs     = new TouchEventArgs( touchPoint, _orbit );
                _orbit.Add( touchPoint );
                OnTouchUp( teArgs );
            }
        }

        /// <summary>
        /// [内部メソッド]
        /// 排他的イベントを割り当てる
        /// </summary>
        private void allocateExclusiveEvent( Win32ApiWrapper.TouchEventData[] eventData )
        {
            Win32ApiWrapper.TouchEventFlags eventFlags = eventData[ 0 ].dwEventFlags;

            // 接触点の座標情報を設定する
            TouchPoint touchPoint1 = new TouchPoint( eventData[ 0 ].x, eventData[ 0 ].y );
            TouchPoint touchPoint2 = null;
            if ( eventData.Length > 1 )
            {
                touchPoint2 = new TouchPoint( eventData[ 1 ].x, eventData[ 1 ].y );
            }

            // 各イベントを割り当てる
            if ( eventData.Length > 1 && ( eventData[ 1 ].dwEventFlags & Win32ApiWrapper.TouchEventFlags.DOWN ) == Win32ApiWrapper.TouchEventFlags.DOWN )
            {
                // ピンチ操作の開始を検知する
                _baseTwoTouchDistance = Calculator.CalcurateDistance( touchPoint1.X, touchPoint1.Y, touchPoint2.X, touchPoint2.Y );
            }
            else if ( eventData.Length > 1 && ( eventData[ 1 ].dwEventFlags & Win32ApiWrapper.TouchEventFlags.MOVE ) == Win32ApiWrapper.TouchEventFlags.MOVE )
            {
                // 「ピンチ」イベントハンドラを実行するか
                int distance = Calculator.CalcurateDistance( touchPoint1.X, touchPoint1.Y, touchPoint2.X, touchPoint2.Y );
                if ( Math.Abs( _baseTwoTouchDistance - distance ) > PinchInterval )
                {
                    // 「ピンチ」イベントハンドラを実行する
                    int       angle     = Calculator.CalculateAngle( touchPoint1.X, touchPoint1.Y, touchPoint2.X, touchPoint2.Y );
                    PinchType type      = Calculator.DistanceToPinchType( _baseTwoTouchDistance, distance );
                    var       pinchData = new PinchData( touchPoint1, touchPoint2, angle, _baseTwoTouchDistance, distance, type );
                    var       peArgs    = new PinchEventArgs( pinchData );
                    OnPinch( peArgs );
                    _baseTwoTouchDistance = distance;
                }
            }
            else if ( ( eventFlags & Win32ApiWrapper.TouchEventFlags.UP ) == Win32ApiWrapper.TouchEventFlags.UP )
            {
                int      distance  = Calculator.CalcurateDistance( _baseTouchPoint.X, _baseTouchPoint.Y, touchPoint1.X, touchPoint1.Y );
                TimeSpan touchSpan = DateTime.Now - _touchDownTime;
                TimeSpan tapSpan   = DateTime.Now - _previousTapTime;
                
                if ( shouldExecSwipeEvent( touchPoint1, distance ) )
                {
                    // 「スワイプ」イベントハンドラを実行する
                    int angle     = Calculator.CalculateAngle( _baseTouchPoint.X, _baseTouchPoint.Y, touchPoint1.X, touchPoint1.Y );
                    SwipeDirection direction = Calculator.AngleToDirection( angle );
                    var swipeData = new SwipeData( _baseTouchPoint, touchPoint1, angle, distance, direction );
                    var seArgs    = new SwipeEventArgs( swipeData, _orbit );
                    OnSwipe( touchPoint1, seArgs );
                    resetFields();
                }
                else if ( touchSpan.TotalMilliseconds < _tapInterval &&  tapSpan.TotalMilliseconds < _doubleTapInterval )
                {
                    // 「ダブルタップ」イベントハンドラを実行する
                    var teArgs = new TouchEventArgs( touchPoint1, _orbit );
                    OnDoubleTap( teArgs );
                }
                else if ( touchSpan.TotalMilliseconds < _tapInterval )
                {
                    // 「タップ」イベントハンドラを実行する
                    _previousTapTime = DateTime.Now;
                    var teArgs = new TouchEventArgs( touchPoint1, _orbit );
                    OnTap( teArgs );
                }
            }
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
            _hasPinched = true;

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
        /// スワイプイベントを実行するか
        /// </summary>
        /// <returns>実行するか</returns>
        private bool shouldExecSwipeEvent( TouchPoint lastTouchPoint, int distance )
        {
            if ( _baseTouchPoint.X < 0 || _baseTouchPoint.Y < 0 )
            {
                return false;
            }

            if ( lastTouchPoint.X < 0 || lastTouchPoint.Y < 0 )
            {
                return false;
            }

            if ( _hasPinched )
            {
                return false;
            }

            return distance > SwipeInterval;
        }

        /// <summary>
        /// [内部メソッド]
        /// フィールドを初期化する
        /// </summary>
        private void resetFields()
        {
            _baseTouchPoint       = new TouchPoint( -1, -1 );
            _baseTwoTouchDistance = -1;
            _orbit                = new List<TouchPoint>( this.Size.Height + this.Size.Width );
            _hasPinched           = false;
        }
    }
}
