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
            var context = CreateInvokeContext(this, psiItemArray);

            var thread = new Thread(static state =>
            {
                var sb = new StringBuilder();

                ((InvokeContext)state!).Unwrap(out var command, out var shellItemArray, out var folderItem, out var hWnd);

                sb.Append("Dll Location: ").AppendLine(DllModule.Location);
                sb.Append("BaseDirectory: ").AppendLine(DllModule.BaseDirectory);
                sb.Append("CurrentPackagePath: ").AppendLine(DllModule.CurrentPackagePath);

                if (folderItem != null)
                {
                    folderItem->GetDisplayName(Windows.Win32.UI.Shell.SIGDN.SIGDN_DESKTOPABSOLUTEPARSING, out var pDisplayName);

                    var displayName = pDisplayName.ToString();
                    sb.Append("Folder: ").AppendLine(displayName);
                }

                if (shellItemArray != null)
                {
                    uint count = 0;
                    shellItemArray->GetCount(&count);

                    for (uint i = 0; i < count; i++)
                    {
                        var shellItem = (Windows.Win32.UI.Shell.IShellItem*)0;
                        shellItemArray->GetItemAt(i, &shellItem);

                        shellItem->GetDisplayName(Windows.Win32.UI.Shell.SIGDN.SIGDN_DESKTOPABSOLUTEPARSING, out var pDisplayName);

                        var displayName = pDisplayName.ToString();
                        sb.Append("File: ").AppendLine(displayName);

                        Marshal.FreeCoTaskMem((nint)pDisplayName.Value);
                    }
                }

                Windows.Win32.PInvoke.MessageBox((HWND)hWnd, sb.ToString(), "DemoExplorerCommandVerb", Windows.Win32.UI.WindowsAndMessaging.MESSAGEBOX_STYLE.MB_OK);


            })
            {
                IsBackground = true,
                Name = "DemoExplorerCommandVerb::Invoke",
            };
            thread.SetApartmentState(ApartmentState.MTA);
            thread.Start(context);

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
            if (_punkSite != null)
            {
                return _punkSite->QueryInterface(riid, ppvSite);
            }
            else
            {
                *ppvSite = null;
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

        private static InvokeContext CreateInvokeContext(DemoExplorerCommandVerb command, Windows.Win32.UI.Shell.IShellItemArray* shellItemArray)
        {
            Windows.Win32.UI.Shell.IShellItem* folder = null;
            nint hWnd = 0;

            fixed (Guid* riid_IShellItem = &Windows.Win32.UI.Shell.IShellItem.IID_Guid)
            fixed (Guid* riid_IUnknown = &IUnknown.IID_Guid)
            {
                IUnknown* site = null;
                try
                {
                    var hr = (HRESULT)command.GetSite(riid_IUnknown, (void**)&site);

                    if (hr.Succeeded)
                    {
                        hr = Windows.Win32.PInvoke.IUnknown_GetWindow(site, out var _hWnd);
                        if (hr.Succeeded) hWnd = _hWnd.Value;

                        hr = (HRESULT)DllMain.GetFolderFromSite((nint)site, riid_IShellItem, (void**)&folder);
                        if (hr.Failed) folder = null;

                        return new InvokeContext(command, shellItemArray, folder, hWnd);
                    }
                }
                finally
                {
                    if (folder != null) folder->Release();
                    if (site != null) site->Release();
                }
            }
            return new InvokeContext(command, shellItemArray, null, default);
        }


        private class InvokeContext
        {
            private IStream* shellItemArrayStream;
            private IStream* folderStream;
            private DemoExplorerCommandVerb? command;
            private nint hWnd;
            public InvokeContext(
                DemoExplorerCommandVerb command,
                Windows.Win32.UI.Shell.IShellItemArray* shellItemArray,
                Windows.Win32.UI.Shell.IShellItem* folder,
                nint hWnd)
            {
                this.command = command;
                this.hWnd = hWnd;

                if (folder != null)
                {
                    fixed (Guid* riid_IShellItemArray = &Windows.Win32.UI.Shell.IShellItemArray.IID_Guid)
                    fixed (IStream** pStream1 = &shellItemArrayStream)
                    {
                        var hr = Windows.Win32.PInvoke.CoMarshalInterThreadInterfaceInStream(riid_IShellItemArray, (IUnknown*)shellItemArray, pStream1);
                        if (hr.Failed) shellItemArrayStream = null;
                    }
                }

                if (folder != null)
                {
                    fixed (Guid* riid_IShellItem = &Windows.Win32.UI.Shell.IShellItem.IID_Guid)
                    fixed (IStream** pStream2 = &folderStream)
                    {
                        var hr = Windows.Win32.PInvoke.CoMarshalInterThreadInterfaceInStream(riid_IShellItem, (IUnknown*)folder, pStream2);
                        if (hr.Failed) folderStream = null;

                    }
                }
            }
            public void Unwrap(
                out DemoExplorerCommandVerb? command,
                out Windows.Win32.UI.Shell.IShellItemArray* shellItemArray,
                out Windows.Win32.UI.Shell.IShellItem* folder,
                out nint hWnd)
            {
                command = this.command;

                shellItemArray = null;
                folder = null;
                hWnd = this.hWnd;

                this.command = null;
                this.hWnd = 0;

                if (shellItemArrayStream != null)
                {
                    fixed (Guid* riid_IShellItemArray = &Windows.Win32.UI.Shell.IShellItemArray.IID_Guid)
                    fixed (Windows.Win32.UI.Shell.IShellItemArray** pShellItemArray = &shellItemArray)
                    {
                        var hr = Windows.Win32.PInvoke.CoGetInterfaceAndReleaseStream(shellItemArrayStream, riid_IShellItemArray, (void**)pShellItemArray);
                        if (hr.Failed) shellItemArray = null;
                        shellItemArrayStream = null;
                    }
                }

                if (folderStream != null)
                {
                    fixed (Guid* riid_IShellItem = &Windows.Win32.UI.Shell.IShellItem.IID_Guid)
                    fixed (Windows.Win32.UI.Shell.IShellItem** pFolder = &folder)
                    {
                        var hr = Windows.Win32.PInvoke.CoGetInterfaceAndReleaseStream(folderStream, riid_IShellItem, (void**)pFolder);
                        if (hr.Failed) folder = null;
                        folderStream = null;
                    }
                }
            }
        }
    }
}
