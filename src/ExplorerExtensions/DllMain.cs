using System.Runtime.InteropServices;
using System.Runtime.InteropServices.Marshalling;
using System.Text;

namespace ExplorerExtensions
{
    internal class DllMain
    {
        unsafe static DllMain()
        {
            var moduleName = $"{typeof(DllMain).Assembly.GetName().Name}.dll";
            fixed (char* lpModuleNameLocal = moduleName)
            {
                HINSTANCE = Windows.Win32.PInvoke.GetModuleHandle(lpModuleNameLocal);
            }

            ComWrappers = new StrategyBasedComWrappers();
            createFunctions = new Dictionary<Guid, Func<object>>()
            {
                [typeof(Demos.DemoPreviewHandler).GUID] = () => new Demos.DemoPreviewHandler(),
                [typeof(Demos.DemoExplorerCommandState).GUID] = () => new Demos.DemoExplorerCommandState(),
                [typeof(Demos.DemoExplorerCommandVerb).GUID] = () => new Demos.DemoExplorerCommandVerb(),
            };
        }

        internal const int S_OK = 0;
        internal const int S_FALSE = 1;
        internal const int E_FAIL = unchecked((int)(0x80004005));
        internal const int E_INVALIDARG = unchecked((int)(0x80070057));
        internal const int E_NOTIMPL = unchecked((int)(0x80004001));
        internal const int E_NOINTERFACE = unchecked((int)(0x80004002));
        internal const int E_PENDING = unchecked((int)(0x8000000A));
        internal const int CLASS_E_CLASSNOTAVAILABLE = unchecked((int)(0x80040111));

        private static long g_cRefModule = 0;
        private static IReadOnlyDictionary<Guid, Func<object>> createFunctions;

        internal static Windows.Win32.Foundation.HMODULE HINSTANCE { get; }
        internal static StrategyBasedComWrappers ComWrappers { get; }


        [UnmanagedCallersOnly(EntryPoint = "DllCanUnloadNow")]
        private static int DllCanUnloadNow()
        {
            return g_cRefModule >= 1 ? S_FALSE : S_OK;
        }

        public static void DllAddRef()
        {
            Interlocked.Increment(ref g_cRefModule);
        }

        public static void DllRelease()
        {
            Interlocked.Decrement(ref g_cRefModule);
        }

        [UnmanagedCallersOnly(EntryPoint = "DllGetClassObject")]
        private unsafe static int DllGetClassObject(Guid* clsid, Guid* riid, void** ppv)
        {
            foreach (var (guid, func) in createFunctions)
            {
                if (clsid->Equals(guid))
                {
                    var factory = new ClassFactory(func);
                    var pFactory = ComWrappers.GetOrCreateComInterfaceForObject(factory, CreateComInterfaceFlags.None);

                    ((Windows.Win32.System.Com.IUnknown*)pFactory)->QueryInterface(riid, ppv);
                    ((Windows.Win32.System.Com.IUnknown*)pFactory)->Release();

                    return S_OK;
                }
            }

            return CLASS_E_CLASSNOTAVAILABLE;
        }

        internal unsafe static uint SafeRelease<T>(T** ppv) where T : unmanaged
        {
            if (*ppv == (void*)0) return 0;

            var result = ((Windows.Win32.System.Com.IUnknown*)*ppv)->Release();
            *ppv = (T*)0;
            return result;
        }

        internal unsafe static int SetInterface<T>(T** ppT, Guid* iid, void* punk) where T : unmanaged
        {
            SafeRelease(ppT);

            if (punk != (void*)0)
            {
                void* ppvObj = (void*)0;
                var hr = ((Windows.Win32.System.Com.IUnknown*)punk)->QueryInterface(iid, &ppvObj).Value;
                *ppT = (T*)ppvObj;
                return hr;
            }
            return E_NOINTERFACE;
        }
    }
}
