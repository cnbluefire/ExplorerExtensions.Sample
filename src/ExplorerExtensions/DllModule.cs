using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace ExplorerExtensions
{
    public static class DllModule
    {
        private static nint hInstance;
        private static string? dllFilePath;
        private static char[] pathSeparatorChars = ['/', '\\'];
        private static string? currentPackagePath;
        private static bool isPackagedApp;

        public static unsafe nint HINSTANCE
        {
            get
            {
                if (hInstance == 0)
                {
                    lock (pathSeparatorChars)
                    {
                        if (hInstance == 0)
                        {
                            const uint GET_MODULE_HANDLE_EX_FLAG_FROM_ADDRESS = 0x00000004;
                            const uint GET_MODULE_HANDLE_EX_FLAG_UNCHANGED_REFCOUNT = 0x00000002;

                            void* funcPtr = (delegate* unmanaged[Stdcall]<void>)&STUB;

                            fixed (nint* pHInstance = &hInstance)
                            {
                                Windows.Win32.PInvoke.GetModuleHandleEx(
                                    GET_MODULE_HANDLE_EX_FLAG_FROM_ADDRESS | GET_MODULE_HANDLE_EX_FLAG_UNCHANGED_REFCOUNT,
                                    new Windows.Win32.Foundation.PCWSTR((char*)funcPtr),
                                    (Windows.Win32.Foundation.HMODULE*)pHInstance);
                            }
                        }
                    }
                }

                return hInstance;

                [UnmanagedCallersOnly(CallConvs = [typeof(CallConvStdcall)])]
                static void STUB()
                { }
            }
        }

        public static string Location
        {
            get
            {
                if (string.IsNullOrEmpty(dllFilePath))
                {
                    lock (pathSeparatorChars)
                    {
                        if (string.IsNullOrEmpty(dllFilePath))
                        {
                            dllFilePath = GetModuleFileName(HINSTANCE);
                        }
                    }
                }
                return dllFilePath;
            }
        }

        public static bool IsPackagedApp
        {
            get
            {
                _ = CurrentPackagePath;
                return isPackagedApp;
            }
        }

        public unsafe static string CurrentPackagePath
        {
            get
            {
                if (currentPackagePath == null)
                {
                    lock (pathSeparatorChars)
                    {
                        if (currentPackagePath == null)
                        {
                            uint length = 0;
                            var result = Windows.Win32.PInvoke.GetCurrentPackagePath(&length, default);
                            if (result == Windows.Win32.Foundation.WIN32_ERROR.ERROR_INSUFFICIENT_BUFFER)
                            {
                                isPackagedApp = true;

                                var buffer = new char[length];
                                fixed (char* pBuffer = buffer)
                                {
                                    result = Windows.Win32.PInvoke.GetCurrentPackagePath(&length, pBuffer);
                                    if (result == Windows.Win32.Foundation.WIN32_ERROR.ERROR_SUCCESS)
                                    {
                                        if (length >= 1) currentPackagePath = new string(pBuffer, 0, (int)length - 1);
                                    }
                                }
                            }
                            else if (result == Windows.Win32.Foundation.WIN32_ERROR.APPMODEL_ERROR_NO_PACKAGE)
                            {
                                isPackagedApp = false;
                            }

                            if (currentPackagePath == null) currentPackagePath = string.Empty;
                        }
                    }
                }
                return currentPackagePath;
            }
        }

        public static string BaseDirectory => GetDirectory(Location);

        private static unsafe string GetModuleFileName(nint hInstance)
        {
            var buffer = new char[65536];
            fixed (char* pBuffer = buffer)
            {
                var length = Windows.Win32.PInvoke.GetModuleFileName((Windows.Win32.Foundation.HMODULE)hInstance, pBuffer, 65536);
                return new string(pBuffer, 0, (int)length);
            }
        }

        private static string GetDirectory(string filePath)
        {
            if (string.IsNullOrWhiteSpace(filePath)) return string.Empty;
            filePath = filePath.Trim();

            if (pathSeparatorChars.Contains(filePath[^1])) return filePath;

            var idx = filePath.LastIndexOfAny(pathSeparatorChars);
            if (idx == -1) return string.Empty;

            idx++;
            return filePath[..idx];
        }
    }
}
