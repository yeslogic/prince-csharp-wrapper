using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;

namespace PrinceXML.Wrapper
{
    using Enums;
    using Events;
    using static Util.CommandLine;

    public class Prince : PrinceBase
    {
        // Input options.
        public List<(string, string)> Remaps { get; } = new List<(string, string)>();
        public bool NoLocalFiles { get; set; }

        // CSS options.
        public string PageSize { get; set; }
        public string PageMargin { get; set; }

        // PDF output options.
        public bool NoSystemFonts { get; set; }
        public int CssDpi { get; set; }

        // Raster output options.
        public RasterFormat RasterFormat { get; set; }
        public int RasterJpegQuality { get; set; } = -1;
        public int RasterPage { get; set; }
        public int RasterDpi { get; set; }
        public int RasterThreads { get; set; } = -1;
        public RasterBackground RasterBackground { get; set; }

        // Additional options.
        public List<(string, string)> Options { get; } = new List<(string, string)>();

        public Prince(string princePath, PrinceEvents events = null)
            : base(princePath, events) {}

        public bool Convert(string inputPath, string outputPath = null)
        {
            List<string> cmdLine = GetJobCommandLine("normal");
            cmdLine.Add(inputPath);
            if (outputPath != null)
            {
                cmdLine.Add(ToCommand("output", outputPath));
            }

            using (Process process = StartPrince(cmdLine))
            {
                return ReadMessages(process.StandardError);
            }
        }

        public bool Convert(List<string> inputPaths, string outputPath)
        {
            List<string> cmdLine = GetJobCommandLine("normal");
            cmdLine.AddRange(inputPaths);
            cmdLine.Add(ToCommand("output", outputPath));

            using (Process process = StartPrince(cmdLine))
            {
                return ReadMessages(process.StandardError);
            }
        }

        public override bool Convert(string inputPath, Stream output)
        {
            return Convert(new List<string> { inputPath }, output);
        }

        public override bool Convert(List<string> inputPaths, Stream output)
        {
            List<string> cmdLine = GetJobCommandLine("buffered");
            cmdLine.AddRange(inputPaths);
            cmdLine.Add(ToCommand("output", "-"));

            using (Process process = StartPrince(cmdLine))
            {
                using (Stream fromPrince = process.StandardOutput.BaseStream)
                {
                    fromPrince.CopyTo(output);
                }
                return ReadMessages(process.StandardError);
            }
        }

        public override bool Convert(Stream input, Stream output)
        {
            if (InputType == null || InputType == InputType.Auto)
            {
                throw new ApplicationException("InputType has to be set to Xml or Html.");
            }

            List<string> cmdLine = GetJobCommandLine("buffered");
            cmdLine.Add("-");

            using (Process process = StartPrince(cmdLine))
            {
                using (Stream toPrince = process.StandardInput.BaseStream)
                {
                    input.CopyTo(toPrince);
                }
                using (Stream fromPrince = process.StandardOutput.BaseStream)
                {
                    fromPrince.CopyTo(output);
                }
                return ReadMessages(process.StandardError);
            }
        }

        public bool ConvertString(string input, string outputPath)
        {
            if (InputType == null || InputType == InputType.Auto)
            {
                throw new ApplicationException("InputType has to be set to Xml or Html.");
            }

            List<string> cmdLine = GetJobCommandLine("buffered");
            cmdLine.Add(ToCommand("output", outputPath));
            cmdLine.Add("-");

            using (Process process = StartPrince(cmdLine))
            {
                using (Stream toPrince = process.StandardInput.BaseStream)
                {
                    byte[] inputBytes = Encoding.UTF8.GetBytes(input);
                    toPrince.Write(inputBytes, 0, inputBytes.Length);
                }
                return ReadMessages(process.StandardError);
            }
        }

        public override bool ConvertString(string input, Stream output)
        {
            using (MemoryStream inputStream =
                new MemoryStream(Encoding.UTF8.GetBytes(input)))
            {
                return Convert(inputStream, output);
            }
        }

        private List<string> GetJobCommandLine(string logType)
        {
            List<string> cmdLine = GetBaseCommandLine();

            cmdLine.Add(ToCommand("structured-log", logType));

            if (InputType != null) { cmdLine.Add(ToCommand("input", InputType)); }
            if (BaseUrl != null) { cmdLine.Add(ToCommand("baseurl", BaseUrl)); }
            if (Remaps.Any()) { cmdLine.AddRange(ToCommands("remap", Remaps.Select((u, d) => $"{u}={d}"))); }
            if (XInclude) { cmdLine.Add(ToCommand("xinclude")); }
            if (XmlExternalEntities) { cmdLine.Add(ToCommand("xml-external-entities")); }
            if (NoLocalFiles) { cmdLine.Add(ToCommand("no-local-files")); }

            if (JavaScript) { cmdLine.Add(ToCommand("javascript")); }
            if (Scripts.Any()) { cmdLine.AddRange(ToCommands("script", Scripts)); }
            if (MaxPasses > 0) { cmdLine.Add(ToCommand("max-passes", MaxPasses)); }

            if (StyleSheets.Any()) { cmdLine.AddRange(ToCommands("style", StyleSheets)); }
            if (Media != null) { cmdLine.Add(ToCommand("media", Media)); }
            if (PageSize != null) { cmdLine.Add(ToCommand("page-size", PageSize)); }
            if (PageMargin != null) { cmdLine.Add(ToCommand("page-margin", PageMargin)); }
            if (NoAuthorStyle) { cmdLine.Add(ToCommand("no-author-style")); }
            if (NoDefaultStyle) { cmdLine.Add(ToCommand("no-default-style")); }

            if (PdfId != null) { cmdLine.Add(ToCommand("pdf-id", PdfId)); }
            if (PdfLang != null) { cmdLine.Add(ToCommand("pdf-lang", PdfLang)); }
            if (PdfProfile != null) { cmdLine.Add(ToCommand("pdf-profile", PdfProfile)); }
            if (PdfOutputIntent != null) { cmdLine.Add(ToCommand("pdf-output-intent", PdfOutputIntent)); }
            if (FileAttachments.Any()) { cmdLine.AddRange(ToCommands("attach", FileAttachments.Select(a => a.Url))); }
            if (NoArtificialFonts) { cmdLine.Add(ToCommand("no-artificial-fonts")); }
            if (NoEmbedFonts) { cmdLine.Add(ToCommand("no-embed-fonts")); }
            if (NoSubsetFonts) { cmdLine.Add(ToCommand("no-subset-fonts")); }
            if (NoSystemFonts) { cmdLine.Add(ToCommand("no-system-fonts")); }
            if (ForceIdentityEncoding) { cmdLine.Add(ToCommand("force-identity-encoding")); }
            if (NoCompress) { cmdLine.Add(ToCommand("no-compress")); }
            if (NoObjectStreams) { cmdLine.Add(ToCommand("no-object-streams")); }
            if (ConvertColors) { cmdLine.Add(ToCommand("convert-colors")); }
            if (FallbackCmykProfile != null) { cmdLine.Add(ToCommand("fallback-cmyk-profile", FallbackCmykProfile)); }
            if (TaggedPdf) { cmdLine.Add(ToCommand("tagged-pdf")); }
            if (CssDpi > 0) {cmdLine.Add(ToCommand("css-dpi", CssDpi)); }

            if (PdfTitle != null) { cmdLine.Add(ToCommand("pdf-title", PdfTitle)); }
            if (PdfSubject != null) { cmdLine.Add(ToCommand("pdf-subject", PdfSubject)); }
            if (PdfAuthor != null) { cmdLine.Add(ToCommand("pdf-author", PdfAuthor)); }
            if (PdfKeywords != null) { cmdLine.Add(ToCommand("pdf-keywords", PdfKeywords)); }
            if (PdfCreator != null) { cmdLine.Add(ToCommand("pdf-creator", PdfCreator)); }
            if (Xmp != null) { cmdLine.Add(ToCommand("pdf-xmp", Xmp)); }

            if (Encrypt) { cmdLine.Add(ToCommand("encrypt")); }
            if (KeyBits != null) { cmdLine.Add(ToCommand("key-bits", (int) KeyBits)); }
            if (UserPassword != null) { cmdLine.Add(ToCommand("user-password", UserPassword)); }
            if (OwnerPassword != null) { cmdLine.Add(ToCommand("owner-password", OwnerPassword)); }
            if (DisallowPrint) { cmdLine.Add(ToCommand("disallow-print")); }
            if (DisallowCopy) { cmdLine.Add(ToCommand("disallow-copy")); }
            if (AllowCopyForAccessibility) { cmdLine.Add(ToCommand("allow-copy-for-accessibility")); }
            if (DisallowAnnotate) { cmdLine.Add(ToCommand("disallow-annotate")); }
            if (DisallowModify) { cmdLine.Add(ToCommand("disallow-modify")); }
            if (AllowAssembly) { cmdLine.Add(ToCommand("allow-assembly")); }

            if (RasterFormat != null) { cmdLine.Add(ToCommand("raster-format", RasterFormat)); }
            if (RasterJpegQuality > -1) { cmdLine.Add(ToCommand("raster-jpeg-quality", RasterJpegQuality)); }
            if (RasterPage > 0) { cmdLine.Add(ToCommand("raster-pages", RasterPage)); }
            if (RasterDpi > 0) { cmdLine.Add(ToCommand("raster-dpi", RasterDpi)); }
            if (RasterThreads > -1) { cmdLine.Add(ToCommand("raster-threads", RasterThreads)); }
            if (RasterBackground != null) { cmdLine.Add(ToCommand("raster-background", RasterBackground)); }

            foreach ((string k, string v) in Options)
            {
                cmdLine.Add(ToCommand(k, v));
            }

            return cmdLine;
        }
    }
}
