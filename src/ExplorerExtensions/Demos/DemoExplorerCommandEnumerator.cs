using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.Marshalling;
using System.Text;
using System.Threading.Tasks;

namespace ExplorerExtensions.Demos
{
    [GeneratedComClass]
    [Guid("D68561FD-27E2-48F5-AF1F-EA451FC0B6CA")]
    internal partial class DemoExplorerCommandEnumerator : IEnumExplorerCommand
    {
        private readonly IExplorerCommand[]? commands;
        private int index = 0;

        internal DemoExplorerCommandEnumerator() { }

        internal DemoExplorerCommandEnumerator(IExplorerCommand[]? commands)
        {
            this.commands = commands;
        }

        public int Clone(out IEnumExplorerCommand? ppenum)
        {
            ppenum = new DemoExplorerCommandEnumerator(commands);
            return DllMain.S_OK;
        }

        public unsafe int Next(uint celt, void** pUICommand, uint* pceltFetched)
        {
            if (pceltFetched != null)
            {
                *pceltFetched = 0;
            }

            if (commands == null || commands.Length == 0) return DllMain.E_NOTIMPL;

            var start = index;
            for (int i = 0; i < celt && start + i < commands.Length; i++)
            {
                pUICommand[i] = (void*)DllMain.ComWrappers.GetOrCreateComInterfaceForObject(commands[index], CreateComInterfaceFlags.None);
                index++;
            }
            if (pceltFetched != null)
            {
                *pceltFetched = (uint)(index - start);
            }

            return (index - start == celt) ? DllMain.S_OK : DllMain.S_FALSE;
        }

        public int Reset()
        {
            index = 0;
            return DllMain.S_OK;
        }

        public int Skip(uint celt)
        {
            return DllMain.E_NOTIMPL;
        }
    }
}
