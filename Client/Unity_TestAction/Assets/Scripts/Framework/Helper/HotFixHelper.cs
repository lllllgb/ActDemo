using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Reflection;
using ILRuntime.CLR.TypeSystem;
using ILRuntime.CLR.Method;
using System.Linq;

namespace AosBaseFramework
{
    public static class HotFixHelper
    {
        private static ILRuntime.Runtime.Enviorment.AppDomain smAppdomain;
        public static ILRuntime.Runtime.Enviorment.AppDomain Appdomain
        {
            get { return smAppdomain; }
        }

        private static Assembly smHotFixAssembly;
        public static Assembly HotFixAssembly
        {
            get { return smHotFixAssembly; }
        }

        public static void LoadHotfixAssembly(byte[] data)
        {
#if TEST_HOTFIX
            byte[] tmpDecompressData = data;//DecompressFileLZMA(data);
#else
            byte[] tmpDecompressData = DecompressFileLZMA(data);
#endif

#if ILRuntime
            smAppdomain = new ILRuntime.Runtime.Enviorment.AppDomain();

            using (System.IO.MemoryStream fs = new System.IO.MemoryStream(tmpDecompressData))
            {
                smAppdomain.LoadAssembly(fs, null, new Mono.Cecil.Pdb.PdbReaderProvider());
            }

            ILHelper.InitILRuntime(smAppdomain);
#else
            smHotFixAssembly = null == tmpDecompressData ? Assembly.GetExecutingAssembly() : Assembly.Load(tmpDecompressData);
#endif
        }

        public static Type[] GetHotfixTypes()
        {
#if ILRuntime
            if (null == smAppdomain)
            {
                return new Type[0];
            }

            return smAppdomain.LoadedTypes.Values.Select(x => x.ReflectionType).ToArray();
#else
            if (null == smHotFixAssembly)
            {
                return new Type[0];
            }
            return smHotFixAssembly.GetTypes();
#endif
        }

        static byte[] DecompressFileLZMA(byte[] data)
        {
            if (null == data)
                return null;

            var coder = new SevenZip.Compression.LZMA.Decoder();
            var input = new System.IO.MemoryStream(data);
            var output = new System.IO.MemoryStream();

            // Read the decoder properties
            byte[] properties = new byte[5];
            input.Read(properties, 0, 5);

            // Read in the decompress file size.
            byte[] fileLengthBytes = new byte[8];
            input.Read(fileLengthBytes, 0, 8);

            long fileLength = System.BitConverter.ToInt64(fileLengthBytes, 0);

            // Decompress the file.
            coder.SetDecoderProperties(properties);
            coder.Code(input, output, input.Length, fileLength, null);
            output.Flush();
            byte[] returnData = output.ToArray();
            output.Close();
            input.Close();
            return returnData;
        }
    }
}
