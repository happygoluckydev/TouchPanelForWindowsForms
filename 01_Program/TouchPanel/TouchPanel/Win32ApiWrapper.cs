using System;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace TouchPanel
{
    /// <summary>
    /// タッチ入力
    /// </summary>
    /// <remarks>
    /// 参考URL 
    /// http://nobu-macsuzuki.hatenablog.com/entry/20160925/1474756690
    /// </remarks>
    static class Win32ApiWrapper
    {
        /// <summary>
        /// [定数]
        /// タッチ入力を識別するWindowsメッセージID
        /// </summary>
        public const int WM_TOUCH = 0x0240;

        /// <summary>
        /// [DLLインポート]
        /// ウインドウをタッチ対応であるとして登録する
        /// </summary>
        /// <param name="hWnd">   登録するウインドウのハンドル          </param>
        /// <param name="ulFlags">オプションの変更を指定するビットフラグ</param>
        /// <returns>             ゼロ以外:成功, ゼロ:失敗              </returns>
        /// <remarks>
        /// Win32 API<br/>
        /// MSDN 
        /// https://msdn.microsoft.com/ja-jp/library/windows/desktop/gg132828%28v=vs.85%29.aspx?f=255&MSPPError=-2147217396
        /// </remarks>
        [DllImport( "user32.dll" )]
        [return: MarshalAs( UnmanagedType.Bool )]
        public static extern bool RegisterTouchWindow( IntPtr hWnd, uint ulFlags );

        /// <summary>
        /// [DLLインポート]
        /// 特定のタッチ入力ハンドルに関連付けられたタッチ入力に関する詳細情報を取得する
        /// </summary>
        /// <param name="hTouchInput">タッチ メッセージの LPARAM で受け取ったタッチ入力ハンドル                                                     </param>
        /// <param name="cInputs">    配列内の構造体の数                                                                                            </param>
        /// <param name="pInputs">    指定されたタッチ入力ハンドルに関連付けられた接触点に関する情報を受け取る TOUCHINPUT 構造体の配列へのポインター</param>
        /// <param name="cbSize">     単一の TOUCHINPUT 構造体のサイズ (バイト)                                                                     </param>
        /// <returns>                 ゼロ以外: 成功, ゼロ:失敗                                                                                     </returns>
        /// <remarks>
        /// Win32 API<br/>
        /// MSDN 
        /// https://msdn.microsoft.com/ja-jp/library/windows/desktop/dd371582%28v=vs.85%29.aspx?f=255&MSPPError=-2147217396
        /// </remarks>
        [DllImport( "user32.dll" )]
        [return: MarshalAs( UnmanagedType.Bool )]
        public static extern bool GetTouchInputInfo( IntPtr hTouchInput, int cInputs, [In, Out] TouchEventData[] pInputs, int cbSize );

        /// <summary>
        /// [構造体]
        /// タッチ入力用データ
        /// </summary>
        /// <remarks>
        /// MSDN 
        /// https://msdn.microsoft.com/ja-jp/library/windows/desktop/dd317334%28v=vs.85%29.aspx?f=255&MSPPError=-2147217396
        /// </remarks>
        public struct TouchEventData
        {
            /// <summary>
            /// [フィールド]
            /// タッチ入力の X 座標 (水平方向の点)<br/>
            /// 物理画面座標の 1/100 ピクセル単位で示されます
            /// </summary>
            public int x;

            /// <summary>
            /// [フィールド]
            /// タッチ入力の Y 座標 (垂直方向の点)<br/>
            /// 物理画面座標の 1/100 ピクセル単位で示されます
            /// </summary>
            public int y;

            /// <summary>
            /// [フィールド]
            /// ソース入力デバイスのデバイス ハンドル<br/>
            /// 各デバイスには、実行時にタッチ入力プロバイダーによって一意のプロバイダーが割り当てられます
            /// </summary>
            public IntPtr hSource;

            /// <summary>
            /// [フィールド]
            /// 特定のタッチ入力を識別する接触点識別子                                  <br/>
            /// この値は、接触が開始してから終了するまで接触シーケンス内で一貫しています<br/>
            /// ID は、後続の接触で再利用される場合があります
            /// </summary>
            public int dwID;

            /// <summary>
            /// [フィールド]
            /// 接触点を押したり放したり動かしたりするさまざまな操作を指定するビット フラグのセット
            /// </summary>
            public TouchEventFlags dwEventFlags;

            /// <summary>
            /// [フィールド]
            /// 有効な値を含む構造体のオプション フィールドを指定するビット フラグのセット <br/>
            /// オプション フィールドの有効な情報を利用できるかどうかは、デバイスに固有です<br/>
            /// アプリケーションでは、対応するビットが dwMask で設定されている場合にのみ   <br/>
            /// オプション フィールドの値を使用する必要があります
            /// </summary>
            public TouchInputMaskFlags dwMaskFlags;

            /// <summary>
            /// [フィールド]
            /// イベントのタイム スタンプ (ミリ秒単位)                                                <br/>
            /// 利用側のアプリケーションでは、システムによってこのフィールドの検証が実行されないことに<br/>
            /// 注意する必要があります                                                                <br/>
            /// TOUCHINPUTMASKF_TIMEFROMSYSTEM フラグが設定されていない場合、                         <br/>
            /// このフィールドの値の精度と順序は、タッチ入力プロバイダーに完全に依存します
            /// </summary>
            public int dwTime;

            /// <summary>
            /// [フィールド]
            /// タッチ イベントに関連付けられている追加の値
            /// </summary>
            public IntPtr dwExtraInfo;

            /// <summary>
            /// [フィールド]
            /// 接触領域の幅 ( 物理画面座標の 1/100 ピクセル単位 )<br/>
            /// TOUCHEVENTFMASK メンバーに CONTACTAREA フラグが設定されている場合のみ有効です
            /// </summary>
            public int cxContact;

            /// <summary>
            /// [フィールド]
            /// 接触領域の高さ ( 物理画面座標の 1/100 ピクセル単位 )<br/>
            /// TOUCHEVENTFMASK メンバーに CONTACTAREA フラグが設定されている場合のみ有効です
            /// </summary>
            public int cyContact;
        }

        /// <summary>
        /// [列挙型]
        /// 接触点を押したり放したり動かしたりする<br/>
        /// さまざまな操作を指定するビット フラグのセット
        /// </summary>
        /// <remarks>
        /// MSDN
        /// https://msdn.microsoft.com/ja-jp/library/windows/desktop/dd317334%28v=vs.85%29.aspx?f=255&MSPPError=-2147217396
        /// </remarks>
        [Flags]
        public enum TouchEventFlags
        {
            /// <summary>
            /// 移動が発生しました。<br/>
            /// TOUCHEVENTF_DOWN と組み合わせることはできません。
            /// </summary>
            MOVE = 0x0001,

            /// <summary>
            /// 対応する接触点が新しい接触によって確立されました。<br/>
            /// TOUCHEVENTF_MOVE または TOUCHEVENTF_UP と組み合わせることはできません。
            /// </summary>
            DOWN = 0x0002,

            /// <summary>
            /// 接触点が削除されました。
            /// </summary>
            UP = 0x0004,

            /// <summary>
            /// 接触点が範囲内にあります。                                               <br/>
            /// 互換性のあるハードウェアでタッチ ホバーをサポートするために使用されます。<br/>
            /// ホバーのサポートが不要なアプリケーションでは、このフラグは無視できます。
            /// </summary>
            INRANGE = 0x0008,

            /// <summary>
            /// この TOUCHINPUT 構造体が第 1 接触点に対応することを示します。
            /// </summary>
            PRIMARY = 0x0010,

            /// <summary>
            /// この入力は、GetTouchInputInfo を<br/>
            /// 使用して受け取られるときに一体化されませんでした。
            /// </summary>
            NOCOALESCE = 0x0020,

            /// <summary>
            /// このタッチ イベントは<br/>
            /// ユーザーの手のひらによるものです。
            /// </summary>
            PALM = 0x0080
        }

        /// <summary>
        /// 有効な値を含む構造体のオプション フィールドを指定するビット フラグのセット。 <br/>
        /// オプション フィールドの有効な情報を利用できるかどうかは、デバイスに固有です。<br/>
        /// アプリケーションでは、対応するビットが dwMask で設定されている場合にのみ、   <br/>
        /// オプション フィールドの値を使用する必要があります。
        /// </summary>
        /// <remarks>
        /// https://msdn.microsoft.com/ja-jp/library/windows/desktop/dd317334%28v=vs.85%29.aspx?f=255&MSPPError=-2147217396
        /// </remarks>
        [Flags]
        public enum TouchInputMaskFlags
        {
            /// <summary>
            /// cxContact と cyContact が有効です。
            /// </summary>
            CONTACTAREA = 0x0004,

            /// <summary>
            /// dwExtraInfo が有効です。
            /// </summary>
            EXTRAINFO = 0x0002,

            /// <summary>
            /// システム時刻が TOUCHINPUT 構造体で設定されました。
            /// </summary>
            TIMEFROMSYSTEM = 0x0001
        }

        /// <summary>
        /// [静的メソッド]
        /// Windowsメッセージからタッチイベントデータを取得する
        /// </summary>
        /// <param name="winMessage">WindowsMessage                                                      </param>
        /// <returns>                タッチイベントデータ( メッセージがタッチ操作ではない場合は空を返す )</returns>
        public static TouchEventData[] GetTouchEventData( Message winMessage )
        {
            // タッチ操作ではないWindowsメッセージの場合は空を返す
            if ( winMessage.Msg != WM_TOUCH )
            {
                return new TouchEventData[ 0 ];
            }

          　int  inputCount = (int) ( winMessage.WParam.ToInt32() & 0xFFFF );
            var  inputs     = new TouchEventData[ inputCount ];
            bool result     = GetTouchInputInfo( winMessage.LParam, inputCount, inputs, Marshal.SizeOf( inputs[ 0 ] ) );

            return inputs;
        }

        /// <summary>
        /// [拡張メソッド]
        /// タッチイベントフラグが DOWN イベントか
        /// </summary>
        /// <param name="flags">タッチイベントフラグ</param>
        /// <returns>           DOWN イベントか     </returns>
        public static bool IsDown( this TouchEventFlags flags )
        {
            return ( flags & TouchEventFlags.DOWN ) == TouchEventFlags.DOWN;
        }

        /// <summary>
        /// [拡張メソッド]
        /// タッチイベントフラグが MOVE イベントか
        /// </summary>
        /// <param name="flags">タッチイベントフラグ</param>
        /// <returns>           MOVEMENTS イベントか</returns>
        public static bool IsMove( this TouchEventFlags flags )
        {
            return ( flags & TouchEventFlags.MOVE ) == TouchEventFlags.MOVE;
        }

        /// <summary>
        /// [拡張メソッド]
        /// タッチイベントフラグが UP イベントか
        /// </summary>
        /// <param name="flags">タッチイベントフラグ</param>
        /// <returns>           UP イベントか       </returns>
        public static bool IsUp( this TouchEventFlags flags )
        {
            return ( flags & TouchEventFlags.UP ) == TouchEventFlags.UP;
        }
    }
}
