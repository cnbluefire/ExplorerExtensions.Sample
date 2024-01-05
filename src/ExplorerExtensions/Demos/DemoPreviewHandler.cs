using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.Marshalling;
using System.Runtime.Versioning;
using System.Text;
using System.Threading.Tasks;
using Windows.Win32.Foundation;
using Windows.Win32.System.Com;

using WNDCLASS_STYLES = Windows.Win32.UI.WindowsAndMessaging.WNDCLASS_STYLES;
using WNDCLASSEXW = Windows.Win32.UI.WindowsAndMessaging.WNDCLASSEXW;
using WINDOW_STYLE = Windows.Win32.UI.WindowsAndMessaging.WINDOW_STYLE;
using DRAW_TEXT_FORMAT = Windows.Win32.Graphics.Gdi.DRAW_TEXT_FORMAT;


namespace ExplorerExtensions.Demos
{
    [GeneratedComClass]
    [Guid("D0CCA119-3218-4EC9-B090-19C95031E349")]
    internal unsafe partial class DemoPreviewHandler : IObjectWithSite, IPreviewHandler, IOleWindow, IInitializeWithStream
    {
        private nint _hwndParent;
        private RECT _rcParent;
        private nint _hwndPreview;

        #region IObjectWithSite

        private IUnknown* _pUnkSite;

        public unsafe int SetSite(IUnknown* pUnkSite)
        {
            fixed (Guid* iid = &IUnknown.IID_Guid)
            fixed (IUnknown** ptr = &_pUnkSite)
            {
                DllMain.SetInterface(ptr, iid, pUnkSite);
            }

            return DllMain.S_OK;
        }

        public unsafe int GetSite(global::System.Guid* riid, void** ppvSite)
        {
            if (_pUnkSite != (void*)0)
            {
                return _pUnkSite->QueryInterface(riid, ppvSite);
            }

            return DllMain.S_OK;
        }

        #endregion IObjectWithSite


        #region IPreviewHandler

        public int DoPreview()
        {
            try
            {
                const string WindowClassName = "Preview_BA3CB2BB98A4";
                const string WindowName = "Preview_BA3CB2BB98A4";

                fixed (char* pClassName = WindowClassName)
                fixed (char* pWindowName = WindowName)
                {
                    var wndClassEx = new WNDCLASSEXW()
                    {
                        cbSize = (uint)Marshal.SizeOf<WNDCLASSEXW>(),
                        style = WNDCLASS_STYLES.CS_VREDRAW | WNDCLASS_STYLES.CS_HREDRAW | WNDCLASS_STYLES.CS_DBLCLKS,
                        lpfnWndProc = &WndProc,
                        cbClsExtra = 0,
                        cbWndExtra = 0,
                        hInstance = DllMain.HINSTANCE,
                        hIcon = Windows.Win32.UI.WindowsAndMessaging.HICON.Null,
                        hCursor = Windows.Win32.PInvoke.LoadCursor((HINSTANCE)0, Windows.Win32.PInvoke.IDC_ARROW),
                        hbrBackground = (Windows.Win32.Graphics.Gdi.HBRUSH)((int)Windows.Win32.Graphics.Gdi.SYS_COLOR_INDEX.COLOR_WINDOW + 1),
                        lpszMenuName = null,
                        lpszClassName = pClassName,
                        hIconSm = Windows.Win32.UI.WindowsAndMessaging.HICON.Null
                    };
                    Windows.Win32.PInvoke.RegisterClassEx(&wndClassEx);

                    _hwndPreview = Windows.Win32.PInvoke.CreateWindowEx(
                        0,
                        pClassName,
                        pWindowName,
                        WINDOW_STYLE.WS_CHILD | WINDOW_STYLE.WS_VSCROLL | WINDOW_STYLE.WS_VISIBLE,
                        _rcParent.left, _rcParent.top, _rcParent.Width, _rcParent.Height,
                        (HWND)_hwndParent, default, DllMain.HINSTANCE, (void*)0);
                }

                return DllMain.S_OK;
            }
            catch (Exception ex)
            {
                return ex.HResult;
            }
        }

        public unsafe int QueryFocus(nint* phwnd)
        {
            if (phwnd != (void*)0)
            {
                *phwnd = Windows.Win32.PInvoke.GetFocus();
                if (*phwnd != 0)
                {
                    return DllMain.S_OK;
                }
                else
                {
                    return Marshal.GetHRForLastWin32Error();
                }
            }

            return DllMain.E_INVALIDARG;
        }

        public int SetFocus()
        {
            if (_hwndPreview != 0)
            {
                Windows.Win32.PInvoke.SetFocus((HWND)_hwndPreview);
                return DllMain.S_OK;
            }

            return DllMain.S_FALSE;
        }

        public unsafe int SetRect(RECT* prc)
        {
            if (prc != (void*)0)
            {
                _rcParent = *prc;
                if (_hwndPreview != 0)
                {
                    Windows.Win32.PInvoke.SetWindowPos(
                        (HWND)_hwndPreview,
                        (HWND)0,
                        _rcParent.left,
                        _rcParent.top,
                        _rcParent.Width,
                        _rcParent.Height,
                        Windows.Win32.UI.WindowsAndMessaging.SET_WINDOW_POS_FLAGS.SWP_NOMOVE
                            | Windows.Win32.UI.WindowsAndMessaging.SET_WINDOW_POS_FLAGS.SWP_NOZORDER
                            | Windows.Win32.UI.WindowsAndMessaging.SET_WINDOW_POS_FLAGS.SWP_NOACTIVATE);
                }
                return DllMain.S_OK;
            }

            return DllMain.E_INVALIDARG;
        }

        public unsafe int SetWindow(nint hwnd, RECT* prc)
        {
            if (hwnd != 0 && prc != (void*)0)
            {
                _hwndParent = hwnd;
                _rcParent = *prc;

                if (_hwndPreview != 0)
                {
                    Windows.Win32.PInvoke.SetParent((HWND)_hwndPreview, (HWND)_hwndParent);
                    Windows.Win32.PInvoke.SetWindowPos(
                        (HWND)_hwndPreview,
                        (HWND)0,
                        _rcParent.left,
                        _rcParent.top,
                        _rcParent.Width,
                        _rcParent.Height,
                        Windows.Win32.UI.WindowsAndMessaging.SET_WINDOW_POS_FLAGS.SWP_NOMOVE
                            | Windows.Win32.UI.WindowsAndMessaging.SET_WINDOW_POS_FLAGS.SWP_NOZORDER
                            | Windows.Win32.UI.WindowsAndMessaging.SET_WINDOW_POS_FLAGS.SWP_NOACTIVATE);

                    Windows.Win32.PInvoke.InvalidateRect((HWND)_hwndPreview, bErase: true);
                }
            }

            return DllMain.S_OK;
        }

        public unsafe int TranslateAccelerator(Windows.Win32.UI.WindowsAndMessaging.MSG* pmsg)
        {
            void* pFrame = (void*)0;

            fixed (Guid* iid = &Windows.Win32.UI.Shell.IPreviewHandlerFrame.IID_Guid)
            {
                if (_pUnkSite != (void*)0
                    && _pUnkSite->QueryInterface(iid, &pFrame).Succeeded)
                {
                    var pTypedFrame = (Windows.Win32.UI.Shell.IPreviewHandlerFrame*)pFrame;

                    try
                    {
                        pTypedFrame->TranslateAccelerator(pmsg);
                        return DllMain.S_OK;
                    }
                    catch (Exception e)
                    {
                        return e.HResult;
                    }
                    finally
                    {
                        DllMain.SafeRelease(&pTypedFrame);
                    }
                }
            }
            return DllMain.S_FALSE;
        }

        public int Unload()
        {
            fixed (IStream** ptr = &_pstream)
            {
                DllMain.SafeRelease(ptr);
            }

            if (_hwndPreview != 0)
            {
                //previewForm = null;
                Windows.Win32.PInvoke.DestroyWindow((HWND)_hwndPreview);
                _hwndPreview = 0;
            }
            return DllMain.S_OK;
        }

        #endregion IPreviewHandler

        #region IOleWindow

        public int ContextSensitiveHelp([MarshalAs(UnmanagedType.Bool)] bool fEnterMode)
        {
            return DllMain.E_NOTIMPL;
        }

        public unsafe int GetWindow(nint* phwnd)
        {
            if (phwnd != (void*)0)
            {
                *phwnd = _hwndParent;
                return DllMain.S_OK;
            }
            return DllMain.E_INVALIDARG;
        }

        #endregion IOleWindow


        #region IInitializeWithStream

        private Windows.Win32.System.Com.IStream* _pstream;

        public unsafe int Initialize(Windows.Win32.System.Com.IStream* pstream, uint grfMode)
        {
            if (pstream != (void*)0)
            {
                fixed (IStream** ptr = &_pstream)
                {
                    DllMain.SafeRelease(ptr);
                }

                _pstream = pstream;
                ((Windows.Win32.System.Com.IUnknown*)pstream)->AddRef();

                return DllMain.S_OK;
            }

            return DllMain.E_INVALIDARG;
        }

        #endregion IInitializeWithStream

        [UnmanagedCallersOnly(CallConvs = new[] { typeof(CallConvStdcall) })]
        private static LRESULT WndProc(HWND hWnd, uint uMsg, WPARAM wParam, LPARAM lParam)
        {
            if (uMsg == Windows.Win32.PInvoke.WM_PAINT)
            {
                var hdc = Windows.Win32.PInvoke.BeginPaint(hWnd, out var lp);
                Windows.Win32.PInvoke.FillRect(
                    hdc,
                    &lp.rcPaint,
                    (Windows.Win32.Graphics.Gdi.HBRUSH)(Windows.Win32.PInvoke.GetStockObject(Windows.Win32.Graphics.Gdi.GET_STOCK_OBJECT_FLAGS.GRAY_BRUSH)).Value);

                var oldObj = Windows.Win32.PInvoke.SelectObject(hdc, Windows.Win32.PInvoke.GetStockObject(Windows.Win32.Graphics.Gdi.GET_STOCK_OBJECT_FLAGS.BLACK_BRUSH));

                const string text = "This is a dotnet preview control";

                fixed (char* pText = text)
                {
                    var rect = new RECT(0, 0, 1000, 1000);
                    Windows.Win32.PInvoke.DrawText(hdc, pText, -1, &rect, DRAW_TEXT_FORMAT.DT_LEFT | DRAW_TEXT_FORMAT.DT_SINGLELINE);
                }

                Windows.Win32.PInvoke.SelectObject(hdc, oldObj);
                Windows.Win32.PInvoke.EndPaint(hWnd, lp);
            }

            return Windows.Win32.PInvoke.DefWindowProc(hWnd, uMsg, wParam, lParam);
        }
    }

}
