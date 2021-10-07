using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using System;

namespace PrinceXML.Wrapper
{
    using Enums;
    using Events;
    using Util;
    using static Util.CommandLine;

    public class PrinceControl : PrinceBase
    {
        public string Version { get; private set; }

        private Process _process;
        private List<string> _inputPaths = new List<string>();
        private List<byte[]> _resources = new List<byte[]>();

        public PrinceControl(string princePath, PrinceEvents events = null)
            : base(princePath, events) {}

        public override bool Convert(string inputPath, Stream output)
        {
            _inputPaths.Add(inputPath);
            return Convert(output);
        }

        public override bool Convert(List<string> inputPaths, Stream output)
        {
            _inputPaths.AddRange(inputPaths);
            return Convert(output);
        }

        public override bool Convert(Stream input, Stream output)
        {
            if (InputType == null || InputType == InputType.Auto)
            {
                throw new ApplicationException("InputType has to be set to Xml or Html.");
            }

            using (MemoryStream ms = new MemoryStream())
            {
                input.CopyTo(ms);
                AddResource(ms.ToArray());
            }

            return Convert(output);
        }

        public override bool ConvertString(string input, Stream output)
        {
            if (InputType == null || InputType == InputType.Auto)
            {
                throw new ApplicationException("InputType has to be set to Xml or Html.");
            }

            AddResource(Encoding.UTF8.GetBytes(input));
            return Convert(output);
        }

        private bool Convert(Stream output)
        {
            if (_process == null)
            {
                throw new SystemException("Control process has not been started.");
            }

            Stream toPrince = _process.StandardInput.BaseStream;
            Stream fromPrince = _process.StandardOutput.BaseStream;

            Chunk.WriteChunk(toPrince, "job", GetJobJson());
            foreach (byte[] r in _resources)
            {
                Chunk.WriteChunk(toPrince, "dat", r);
            }
            toPrince.Flush();

            Chunk chunk = Chunk.ReadChunk(fromPrince);
            if (chunk.Tag == "pdf")
            {
                output.Write(chunk.Bytes, 0, chunk.Bytes.Length);
                chunk = Chunk.ReadChunk(fromPrince);
            }

            if (chunk.Tag == "log")
            {
                using (MemoryStream ms = new MemoryStream(chunk.Bytes))
                using (StreamReader sr = new StreamReader(ms))
                {
                    return ReadMessages(sr);
                }
            }
            else if (chunk.Tag == "err")
            {
                throw new IOException("Error: " + chunk.ToString());
            }
            else
            {
                throw new IOException("Unknown chunk: " + chunk.Tag);
            }
        }

        public void Start()
        {
            if (_process != null)
            {
                throw new SystemException("Control process has already been started.");
            }

            List<string> cmdLine = GetBaseCommandLine();
            cmdLine.Add(ToCommand("control"));

            _process = StartPrince(cmdLine);

            Stream fromPrince = _process.StandardOutput.BaseStream;
            Chunk chunk = Chunk.ReadChunk(fromPrince);

            if (chunk.Tag == "ver")
            {
                Version = chunk.ToString();
            }
            else if (chunk.Tag == "err")
            {
                throw new IOException("Error: " + chunk.ToString());
            }
            else
            {
                throw new IOException("Unknown chunk: " + chunk.Tag);
            }
        }

        public void Stop()
        {
            if (_process == null)
            {
                throw new SystemException("Control process has not been started.");
            }

            using (Stream toPrince = _process.StandardInput.BaseStream)
            {
                Chunk.WriteChunk(toPrince, "end", "");
            }
            _process.StandardOutput.Close();

            _process.Close();
        }

        private string GetJobJson()
        {
            Json json = new Json();
            json.BeginObj();

            json.BeginObj("input");
            json.BeginList("src");
            _inputPaths.ForEach(i => json.Value(i));
            json.EndList();

            if (InputType != null) { json.Field("type", InputType.ToString()); }
            if (BaseUrl != null) { json.Field("base", BaseUrl); }
            if (Media != null) { json.Field("media", Media); }

            json.BeginList("styles");
            StyleSheets.ForEach(s => json.Value(s));
            json.EndList();

            json.BeginList("scripts");
            Scripts.ForEach(s => json.Value(s));
            json.EndList();

            json.Field("default-style", !NoDefaultStyle);
            json.Field("author-style", !NoAuthorStyle);
            json.Field("javascript", JavaScript);
            if (MaxPasses > 0) { json.Field("max-passes", MaxPasses); }
            json.Field("xinclude", XInclude);
            json.Field("xml-external-entities", XmlExternalEntities);
            json.EndObj();

            json.BeginObj("pdf");
            json.Field("embed-fonts", !NoEmbedFonts);
            json.Field("subset-fonts", !NoSubsetFonts);
            json.Field("artificial-fonts", !NoArtificialFonts);
            json.Field("force-identity-encoding", ForceIdentityEncoding);
            json.Field("compress", !NoCompress);
            json.Field("object-streams", !NoObjectStreams);

            json.BeginObj("encrypt");
            if (KeyBits != null) { json.Field("key-bits", (int) KeyBits); }
            if (UserPassword != null) { json.Field("user-password", UserPassword); }
            if (OwnerPassword != null) { json.Field("owner-password", OwnerPassword); }
            json.Field("disallow-print", DisallowPrint);
            json.Field("disallow-modify", DisallowModify);
            json.Field("disallow-copy", DisallowCopy);
            json.Field("disallow-annotate", DisallowAnnotate);
            json.Field("allow-copy-for-accessibility", AllowCopyForAccessibility);
            json.Field("allow-assembly", AllowAssembly);
            json.EndObj();

            if (PdfProfile != null) { json.Field("pdf-profile", PdfProfile.ToString()); }
            if (PdfOutputIntent != null) { json.Field("pdf-output-intent", PdfOutputIntent); }
            if (FallbackCmykProfile != null) { json.Field("fallback-cmyk-profile", FallbackCmykProfile); }
            json.Field("color-conversion", ConvertColors ? "output-intent" : "none");
            if (PdfId != null) { json.Field("pdf-id", PdfId); }
            if (PdfLang != null) { json.Field("pdf-lang", PdfLang); }
            if (Xmp != null) { json.Field("pdf-xmp", Xmp); }
            json.Field("tagged-pdf", TaggedPdf);

            json.BeginList("attach");
            foreach (FileAttachment fa in FileAttachments)
            {
                json.BeginObj();
                json.Field("url", fa.Url);
                if (fa.FileName != null) { json.Field("filename", fa.FileName); }
                if (fa.Description != null) { json.Field("description", fa.Description); }
                json.EndObj();
            }
            json.EndList();
            json.EndObj();

            json.BeginObj("metadata");
            if (PdfTitle != null) { json.Field("title", PdfTitle); }
            if (PdfSubject != null) { json.Field("subject", PdfSubject); }
            if (PdfAuthor != null) { json.Field("author", PdfAuthor); }
            if (PdfKeywords != null) { json.Field("keywords", PdfKeywords); }
            if (PdfCreator != null) { json.Field("creator", PdfCreator); }
            json.EndObj();

            json.Field("job-resource-count", _resources.Count);

            json.EndObj();
            return json.ToString();
        }

        public void AddScript(byte[] script)
        {
            _resources.Add(script);
            Scripts.Add("job-resource:" + (_resources.Count - 1));
        }

        public void AddStyleSheet(byte[] styleSheet)
        {
            _resources.Add(styleSheet);
            StyleSheets.Add("job-resource:" + (_resources.Count - 1));
        }

        public void AddFileAttachment(byte[] attachment, string fileName, string description)
        {
            _resources.Add(attachment);
            FileAttachments.Add(new FileAttachment(
                "job-resource:" + (_resources.Count - 1), fileName, description));
        }

        private void AddResource(byte[] resource)
        {
            _resources.Add(resource);
            _inputPaths.Add("job-resource:" + (_resources.Count - 1));
        }
    }
}