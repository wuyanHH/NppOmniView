using System;
using System.IO;
using dnlib.DotNet;
using dnlib.DotNet.MD;
using dnlib.DotNet.Writer;
using dnlib.PE;

class Program
{
    static int Main(string[] args)
    {
        if (args.Length == 0 || !File.Exists(args[0])) return 1;
        string dllPath = args[0];
        byte[] rawBytes = File.ReadAllBytes(dllPath);
        using (ModuleDefMD mod = ModuleDefMD.Load(rawBytes))
        {
            TypeDef mainType = mod.Find("NppPlugin.OmniView.Main", false);
            if (mainType == null) return 2;
            var callConvCdecl = mod.CorLibTypes.GetTypeRef("System.Runtime.CompilerServices", "CallConvCdecl");
            string[] exports = { "isUnicode", "setInfo", "getName", "getFuncsArray", "beNotified", "messageProc" };
            foreach (string name in exports)
            {
                MethodDef method = mainType.FindMethod(name);
                if (method != null)
                {
                    method.ExportInfo = new MethodExportInfo();
                    method.ExportInfo.Options = MethodExportInfoOptions.FromUnmanaged;
                    // Idempotent check: prevent multi-wrapping CModOptSig on repeated runs
                    if (!(method.MethodSig.RetType is CModOptSig cmod && cmod.Modifier.Name == "CallConvCdecl"))
                    {
                        method.MethodSig.RetType = new CModOptSig(callConvCdecl, method.MethodSig.RetType);
                    }
                }
            }
            var opts = new ModuleWriterOptions(mod);
            opts.Cor20HeaderOptions.Flags &= ~ComImageFlags.ILOnly;
            opts.PEHeadersOptions.Machine = Machine.AMD64;
            mod.Write(dllPath, opts);
        }
        return 0;
    }
}
