using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.Marshalling;
using System.Text;
using System.Threading.Tasks;

namespace ExplorerExtensions
{

    [GeneratedComClass]
    internal partial class ClassFactory : IClassFactory
    {
        private readonly Func<object> createFunc;

        public ClassFactory(Func<object> createFunc)
        {
            this.createFunc = createFunc;
        }

        public unsafe int CreateInstance([Optional] void* pUnkOuter, Guid* riid, void** ppvObject)
        {
            var obj = createFunc.Invoke();

            var result = DllMain.ComWrappers.GetOrCreateComInterfaceForObject(obj!, CreateComInterfaceFlags.None);

            var punk = ((Windows.Win32.System.Com.IUnknown*)result);
            var hr = punk->QueryInterface(*riid, out var pvObject);

            if (hr.Succeeded)
            {
                *ppvObject = pvObject;
            }

            punk->Release();

            return hr.Value;
        }

        public int LockServer(bool fLock)
        {
            if (fLock)
            {
                DllMain.DllAddRef();
            }
            else
            {
                DllMain.DllRelease();
            }

            return DllMain.S_OK;
        }
    }

}
