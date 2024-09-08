using System;
using System.Collections.Generic;
using System.Text;
using CustomToolGenerator;
using System.IO;
using Canna.Prolog.Runtime.Compiler;
using System.Runtime.InteropServices;
using System.Diagnostics;

namespace CustomTool
{
    [Guid("4489c1ff-a893-4140-bbdd-5edaad102fcd")]
    public class PrologCodeGenerator : BaseCodeGeneratorWithSite
    {

        public PrologCodeGenerator()
        {

        }

        protected override byte[] GenerateCode(string inputFileName, string inputFileContent)
        {
            //string tempFile = Path.GetTempFileName();
            string output=string.Empty;
            try
            {

                output = PrologCompiler.PrologToCSharp(inputFileName, this.FileNameSpace);
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.ToString());
            }
            finally
            {
              //  File.Delete(tempFile);
            }
            return System.Text.Encoding.ASCII.GetBytes(output);
        }
    }
}
