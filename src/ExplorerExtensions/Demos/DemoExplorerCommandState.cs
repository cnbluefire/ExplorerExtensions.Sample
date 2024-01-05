using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.Marshalling;
using System.Text;
using System.Threading.Tasks;
using Windows.Win32.UI.Shell;

using _EXPCMDSTATE = Windows.Win32.UI.Shell._EXPCMDSTATE;
using IPropertyBag = Windows.Win32.System.Com.StructuredStorage.IPropertyBag;

namespace ExplorerExtensions.Demos
{
    [GeneratedComClass]
    [Guid("67E0A595-8696-426D-A41B-6E17A926A516")]
    internal unsafe partial class DemoExplorerCommandState : IExplorerCommandState, IInitializeCommand
    {
        #region IExplorerCommandState

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

        #endregion IExplorerCommandState

        #region IInitializeCommand

        private static IPropertyBag* _pPropBag;

        public unsafe int Initialize([MarshalAs(UnmanagedType.LPWStr)] string pszCommandName, IPropertyBag* ppb)
        {
            fixed (Guid* iid = &IPropertyBag.IID_Guid)
            fixed (IPropertyBag** ptr = &_pPropBag)
            {
                DllMain.SetInterface(ptr, iid, (void*)ppb);
            }
            return DllMain.S_OK;
        }

        #endregion IInitializeCommand
    }
}
