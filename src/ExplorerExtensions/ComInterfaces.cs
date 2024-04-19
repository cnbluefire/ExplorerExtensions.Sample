using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.Marshalling;
using System.Text;
using System.Threading.Tasks;

namespace ExplorerExtensions
{

    [GeneratedComInterface]
    [Guid("00000001-0000-0000-C000-000000000046")]
    internal partial interface IClassFactory
    {
        [PreserveSig]
        unsafe int CreateInstance([Optional] void* pUnkOuter, global::System.Guid* riid, void** ppvObject);

        [PreserveSig]
        int LockServer([MarshalAs(UnmanagedType.Bool)] bool fLock);
    }

    [GeneratedComInterface]
    [Guid("8895B1C6-B41F-4C1C-A562-0D564250836F")]
    internal partial interface IPreviewHandler
    {
        [PreserveSig]
        unsafe int SetWindow(nint hwnd, Windows.Win32.Foundation.RECT* prc);

        [PreserveSig]
        unsafe int SetRect(Windows.Win32.Foundation.RECT* prc);

        [PreserveSig]
        int DoPreview();

        [PreserveSig]
        int Unload();

        [PreserveSig]
        int SetFocus();

        [PreserveSig]
        unsafe int QueryFocus(nint* phwnd);

        [PreserveSig]
        unsafe int TranslateAccelerator(Windows.Win32.UI.WindowsAndMessaging.MSG* pmsg);
    }

    [GeneratedComInterface]
    [Guid("B824B49D-22AC-4161-AC8A-9916E8FA3F7F")]
    internal partial interface IInitializeWithStream
    {
        [PreserveSig]
        unsafe int Initialize(Windows.Win32.System.Com.IStream* pstream, uint grfMode);
    }

    [GeneratedComInterface]
    [Guid("FC4801A3-2BA9-11CF-A229-00AA003D7352")]
    internal partial interface IObjectWithSite
    {
        [PreserveSig]
        unsafe int SetSite(Windows.Win32.System.Com.IUnknown* pUnkSite);

        [PreserveSig]
        unsafe int GetSite(global::System.Guid* riid, void** ppvSite);
    }

    [GeneratedComInterface]
    [Guid("00000114-0000-0000-C000-000000000046")]
    internal partial interface IOleWindow
    {
        [PreserveSig]
        unsafe int GetWindow(nint* phwnd);

        [PreserveSig]
        int ContextSensitiveHelp([MarshalAs(UnmanagedType.Bool)] bool fEnterMode);
    }

    [GeneratedComInterface]
    [Guid("A08CE4D0-FA25-44AB-B57C-C7B1C323E0B9")]
    internal partial interface IExplorerCommand
    {
        [PreserveSig]
        unsafe int GetTitle(Windows.Win32.UI.Shell.IShellItemArray* psiItemArray, Windows.Win32.Foundation.PWSTR* ppszName);

        [PreserveSig]
        unsafe int GetIcon(Windows.Win32.UI.Shell.IShellItemArray* psiItemArray, Windows.Win32.Foundation.PWSTR* ppszIcon);

        [PreserveSig]
        unsafe int GetToolTip(Windows.Win32.UI.Shell.IShellItemArray* psiItemArray, Windows.Win32.Foundation.PWSTR* ppszInfotip);

        [PreserveSig]
        unsafe int GetCanonicalName(global::System.Guid* pguidCommandName);

        [PreserveSig]
        unsafe int GetState(Windows.Win32.UI.Shell.IShellItemArray* psiItemArray, [MarshalAs(UnmanagedType.Bool)] bool fOkToBeSlow, uint* pCmdState);

        [PreserveSig]
        unsafe int Invoke(Windows.Win32.UI.Shell.IShellItemArray* psiItemArray, Windows.Win32.System.Com.IBindCtx* pbc);

        [PreserveSig]
        unsafe int GetFlags(uint* pFlags);

        [PreserveSig]
        unsafe int EnumSubCommands(out IEnumExplorerCommand? ppEnum);
    }

    [GeneratedComInterface]
    [Guid("85075ACF-231F-40EA-9610-D26B7B58F638")]
    internal partial interface IInitializeCommand
    {
        [PreserveSig]
        unsafe int Initialize([MarshalAs(UnmanagedType.LPWStr)] string pszCommandName, Windows.Win32.System.Com.StructuredStorage.IPropertyBag* ppb);
    }

    [GeneratedComInterface]
    [Guid("BDDACB60-7657-47AE-8445-D23E1ACF82AE")]
    internal partial interface IExplorerCommandState
    {
        [PreserveSig]
        unsafe int GetState(Windows.Win32.UI.Shell.IShellItemArray* psiItemArray, [MarshalAs(UnmanagedType.Bool)] bool fOkToBeSlow, uint* pCmdState);
    }


    [GeneratedComInterface]
    [Guid("A88826F8-186F-4987-AADE-EA0CEF8FBFE8")]
    internal partial interface IEnumExplorerCommand
    {
        [PreserveSig]
        unsafe int Next(uint celt, void** pUICommand, uint* pceltFetched);

        [PreserveSig]
        int Skip(uint celt);

        [PreserveSig]
        int Reset();

        [PreserveSig]
        unsafe int Clone(out IEnumExplorerCommand? ppenum);
    }
}
