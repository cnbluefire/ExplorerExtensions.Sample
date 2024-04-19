using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.Marshalling;
using System.Text;
using System.Threading.Tasks;
using Windows.Win32.Foundation;
using Windows.Win32.System.Com;

using _EXPCMDSTATE = Windows.Win32.UI.Shell._EXPCMDSTATE;
using _EXPCMDFLAGS = Windows.Win32.UI.Shell._EXPCMDFLAGS;
using IPropertyBag = Windows.Win32.System.Com.StructuredStorage.IPropertyBag;
using System.Runtime.CompilerServices;

namespace ExplorerExtensions.Demos
{
    [GeneratedComClass]
    [Guid("C41D6460-8AC9-40B7-A62E-584237875943")]
    internal unsafe partial class DemoExplorerCommandVerb : IExplorerCommand, IInitializeCommand, IObjectWithSite
    {
        private readonly string contextMenuName;
        private readonly IExplorerCommand[]? childCommands;

        internal DemoExplorerCommandVerb() : this("Demo Context Menu", null) { }

        internal DemoExplorerCommandVerb(string contextMenuName, IExplorerCommand[]? childCommands)
        {
            this.contextMenuName = contextMenuName;
            this.childCommands = childCommands;
        }


        #region IExplorerCommand

        private IStream* _pstmShellItemArray;


        public unsafe int EnumSubCommands(out IEnumExplorerCommand? ppEnum)
        {
            if (childCommands == null)
            {
                ppEnum = null;
                return DllMain.E_NOTIMPL;
            }

            ppEnum = new DemoExplorerCommandEnumerator(childCommands);
            return DllMain.S_OK;
        }

        public unsafe int GetCanonicalName(Guid* pguidCommandName)
        {
            *pguidCommandName = Guid.Empty;
            return DllMain.E_NOTIMPL;
        }

        public unsafe int GetFlags(uint* pFlags)
        {
            *pFlags = (uint)_EXPCMDFLAGS.ECF_DEFAULT;
            if (childCommands != null && childCommands.Length > 0)
            {
                *pFlags |= (uint)_EXPCMDFLAGS.ECF_HASSUBCOMMANDS;
            }
            return DllMain.S_OK;
        }

        public unsafe int GetIcon(Windows.Win32.UI.Shell.IShellItemArray* psiItemArray, PWSTR* ppszIcon)
        {
            *ppszIcon = new PWSTR((char*)0);
            return DllMain.E_NOTIMPL;
        }

        public unsafe int GetState(Windows.Win32.UI.Shell.IShellItemArray* psiItemArray, [MarshalAs(UnmanagedType.Bool)] bool fOkToBeSlow, uint* pCmdState)
        {
            if (fOkToBeSlow)
            {
                *pCmdState = (uint)_EXPCMDSTATE.ECS_ENABLED;
                return DllMain.S_OK;
            }
            else
            {
                *pCmdState = (uint)_EXPCMDSTATE.ECS_DISABLED;
                return DllMain.E_PENDING;
            }
        }

        public unsafe int GetTitle(Windows.Win32.UI.Shell.IShellItemArray* psiItemArray, PWSTR* ppszName)
        {
            fixed (char* pStr = contextMenuName)
            {
                return Windows.Win32.PInvoke.SHStrDup(pStr, ppszName).Value;
            }
        }

        public unsafe int GetToolTip(Windows.Win32.UI.Shell.IShellItemArray* psiItemArray, PWSTR* ppszInfotip)
        {
            *ppszInfotip = new PWSTR((char*)0);
            return DllMain.E_NOTIMPL;
        }

        public unsafe int Invoke(Windows.Win32.UI.Shell.IShellItemArray* psiItemArray, IBindCtx* pbc)
        {
            if (Windows.Win32.PInvoke.IUnknown_GetWindow(_punkSite, out var hwnd).Failed)
            {
                hwnd = (HWND)0;
            }

            fixed (Guid* iid = &Windows.Win32.UI.Shell.IShellItemArray.IID_Guid)
            fixed (IStream** pp = &_pstmShellItemArray)
            {
                var _hr = Windows.Win32.PInvoke.CoMarshalInterThreadInterfaceInStream(iid, (IUnknown*)psiItemArray, pp);

                if (!_hr.Succeeded) return _hr;
            }

            var iunk = DllMain.ComWrappers.GetOrCreateComInterfaceForObject(this, CreateComInterfaceFlags.None);

            ((IUnknown*)iunk)->AddRef();

            Task.Run(() =>
            {
                var hr = Windows.Win32.PInvoke.CoGetInterfaceAndReleaseStream(_pstmShellItemArray, Windows.Win32.UI.Shell.IShellItemArray.IID_Guid, out var ppv);
                _pstmShellItemArray = default;
                if (hr.Succeeded)
                {
                    var array = (Windows.Win32.UI.Shell.IShellItemArray*)ppv;

                    try
                    {
                        uint count = 0;
                        array->GetCount(&count);

                        var sb = new StringBuilder();

                        for (uint i = 0; i < count; i++)
                        {
                            var shellItem = (Windows.Win32.UI.Shell.IShellItem*)0;
                            array->GetItemAt(i, &shellItem);

                            shellItem->GetDisplayName(Windows.Win32.UI.Shell.SIGDN.SIGDN_PARENTRELATIVEPARSING, out var pDisplayName);

                            var displayName = pDisplayName.ToString();
                            sb.AppendLine(displayName);

                            Marshal.FreeCoTaskMem((nint)pDisplayName.Value);
                        }

                        Windows.Win32.PInvoke.MessageBox(hwnd, sb.ToString(), "DemoExplorerCommandVerb", Windows.Win32.UI.WindowsAndMessaging.MESSAGEBOX_STYLE.MB_OK);
                    }
                    catch { }
                }

                ((IUnknown*)iunk)->Release();
            });

            return DllMain.S_OK;
        }

        #endregion IExplorerCommand

        #region IInitializeCommand

        public unsafe int Initialize([MarshalAs(UnmanagedType.LPWStr)] string pszCommandName, IPropertyBag* ppb)
        {
            return DllMain.S_OK;
        }

        #endregion IInitializeCommand


        #region IObjectWithSite

        private IUnknown* _punkSite;

        public unsafe int GetSite(Guid* riid, void** ppvSite)
        {
            *ppvSite = (void*)0;
            if (_punkSite != (void*)0)
            {
                return _punkSite->QueryInterface(riid, ppvSite);
            }
            return DllMain.E_FAIL;
        }


        public unsafe int SetSite(IUnknown* pUnkSite)
        {
            fixed (Guid* iid = &IUnknown.IID_Guid)
            fixed (IUnknown** ptr = &_punkSite)
            {
                DllMain.SetInterface(ptr, iid, pUnkSite);
            }

            return DllMain.S_OK;
        }

        #endregion IObjectWithSite
    }
}
