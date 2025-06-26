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

    /// <summary>
    /// Class that provides the default interface to Prince, where each document
    /// conversion invokes a new Prince process.
    /// </summary>
    public class Prince : PrinceBase
    {
        // Input options.
        /// <summary>
        /// Rather than retrieving documents using a URL, remap them to a local directory.
        /// </summary>
        /// <value>
        /// A tuple, where the first value is a URL, and the second is the directory
        /// to remap that URL to.
        /// </value>
        public List<(string, string)> Remaps { get; } = new List<(string, string)>();

        // CSS options.
        /// <summary>
        /// Specify the page size.
        /// </summary>
        /// <value>The page size to use (e.g. "A4").</value>
        public string PageSize { get; set; }
        /// <summary>
        /// Specify the page margin.
        /// </summary>
        /// <value>The page margin to use (e.g. "20mm").</value>
        public string PageMargin { get; set; }

        // PDF output options.
        /// <summary>
        /// Disable the use of system fonts. Only fonts defined with <c>font-face</c>
        /// rules in CSS will be available.
        /// </summary>
        /// <value>true if system fonts are disabled. Default value is false.</value>
        public bool NoSystemFonts { get; set; }
        /// <summary>
        /// Changes the DPI of the "px" units in CSS.
        /// </summary>
        /// <value>
        /// The DPI of the "px" units. Value must be greater than 0. Default value
        /// is 96 dpi.
        /// </value>
        public int CssDpi { get; set; }

        // Raster output options.
        /// <summary>
        /// Specify the format for the raster output.
        /// </summary>
        /// <value>
        /// The format for the raster output. Default value is <c>RasterFormat.Auto</c>.
        /// </value>
        public RasterFormat RasterFormat { get; set; }
        /// <summary>
        /// Specify the level of JPEG compression when generating raster output in
        /// JPEG format.
        /// </summary>
        /// <value>
        /// The level of JPEG compression. Valid range is between 0 and 100 inclusive.
        /// Default value is 92.
        /// </value>
        public int RasterJpegQuality { get; set; } = -1;
        /// <summary>
        /// Set the page number to be rasterized.
        /// </summary>
        /// <value>
        /// The page number to be rasterized. Value must be greater than 0. Defaults
        /// to rasterizing all pages.
        /// </value>
        public int RasterPage { get; set; }
        /// <summary>
        /// Specify the resolution of raster output.
        /// </summary>
        /// <value>
        /// The raster output resolution. Vlaue must be greater than 0. Default
        /// value is 96 dpi.
        /// </value>
        public int RasterDpi { get; set; }
        /// <summary>
        /// Set the number of threads to use for multi-threaded rasterization.
        /// </summary>
        /// <value>
        /// The number of threads to use. Default value is the number of cores and
        /// hyperthreads the system provides.
        /// </value>
        public int RasterThreads { get; set; } = -1;
        /// <summary>
        /// Set the background. Can be used when rasterizing to an image format that
        /// supports transparency.
        /// </summary>
        /// <value>The raster background. Default value is <c>RasterBackground.White</c>.</value>
        public RasterBackground RasterBackground { get; set; }

        // Additional options.
        /// <summary>
        /// Specify additional Prince command-line options.
        /// </summary>
        /// <value>
        /// The command line option, comprised of a key and an optional value.
        /// Pass in a <c>null</c> if the value is not required.
        /// </value>
        public List<(string, string)> Options { get; } = new List<(string, string)>();

        /// <summary>
        /// Constructor for <c>Prince</c>
        /// </summary>
        /// <param name="princePath">
        /// The path of the Prince executable. For example, this may be
        /// <c>C:\Program&#xA0;Files\Prince\engine\bin\prince.exe</c> on Windows.
        /// </param>
        /// <param name="events">
        /// An implementation of <c>PrinceEvents</c> that will receive messages
        /// returned from Prince.
        /// </param>
        public Prince(string princePath, PrinceEvents events = null)
            : base(princePath, events) {}

        /// <summary>
        /// Convert an XML or HTML file to a PDF file.
        /// </summary>
        /// <param name="inputPath">The filename of the input XML or HTML document.</param>
        /// <param name="outputPath">
        /// The filename of the output PDF file. If not provided, the name of the output
        /// file will be the same as the name of the input file, but with the <c>.pdf</c>
        /// extension.
        /// </param>
        /// <returns>true if a PDF file was generated successfully.</returns>
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

        /// <summary>
        /// Convert multiple XML or HTML files to a PDF file.
        /// </summary>
        /// <param name="inputPaths">The filenames of the input XML or HTML documents.</param>
        /// <param name="outputPath">The filename of the output PDF file.</param>
        /// <returns>true if a PDF file was generated successfully.</returns>
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

        /// <inheritdoc/>
        public override bool Convert(string inputPath, Stream output)
        {
            return Convert(new List<string> { inputPath }, output);
        }

        /// <inheritdoc/>
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

        /// <inheritdoc/>
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

        /// <summary>
        /// Convert multiple XML or HTML files to a PDF file by reading an input list from a
        /// specified file. (An input list is a newline-separated sequence of file paths / URLs.)
        /// </summary>
        /// <param name="inputListPath">The path to the input list file.</param>
        /// <param name="outputPath">The filename of the output PDF file.</param>
        /// <returns>true if a PDF file was generated successfully.</returns>
        public bool ConvertInputList(string inputListPath, string outputPath)
        {
            List<string> cmdLine = GetJobCommandLine("normal");
            cmdLine.Add(ToCommand("input-list", inputListPath));
            cmdLine.Add(ToCommand("output", outputPath));

            using (Process process = StartPrince(cmdLine))
            {
                return ReadMessages(process.StandardError);
            }
        }

        /// <summary>
        /// Convert multiple XML or HTML files to a PDF file by reading an input list from a
        /// specified file. (An input list is a newline-separated sequence of file paths / URLs.)
        /// </summary>
        /// <param name="inputListPath">The path to the input list file.</param>
        /// <param name="output">
        /// The output Stream to which Prince will write the PDF output.
        /// </param>
        /// <returns>true if a PDF file was generated successfully.</returns>
        public bool ConvertInputList(string inputListPath, Stream output)
        {
            List<string> cmdLine = GetJobCommandLine("buffered");
            cmdLine.Add(ToCommand("input-list", inputListPath));
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

        /// <summary>
        /// Convert an XML or HTML string to a PDF file.
        /// </summary>
        /// <param name="input">The XML or HTML document in the form of a string.</param>
        /// <param name="outputPath">The filename of the output PDF file.</param>
        /// <returns>true if a PDF file was generated successfully.</returns>
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

        /// <inheritdoc/>
        public override bool ConvertString(string input, Stream output)
        {
            using (MemoryStream inputStream =
                new MemoryStream(Encoding.UTF8.GetBytes(input)))
            {
                return Convert(inputStream, output);
            }
        }

        /// <summary>
        /// Rasterize an XML or HTML file.
        /// </summary>
        /// <param name="inputPath">The filename of the input XML or HTML document.</param>
        /// <param name="outputPath">
        /// A template string from which the raster files will be named (e.g. "page_%02d.png"
        /// will cause Prince to generate page_01.png, page_02.png, ..., page_10.png etc.).
        /// </param>
        /// <returns>true if the input was successfully rasterized.</returns>
        public bool Rasterize(string inputPath, string outputPath)
        {
            return Rasterize(new List<string> { inputPath }, outputPath);
        }

        /// <summary>
        /// Rasterize multiple XML or HTML files.
        /// </summary>
        /// <param name="inputPaths">The filenames of the input XML or HTML documents.</param>
        /// <param name="outputPath">
        /// A template string from which the raster files will be named (e.g. "page_%02d.png"
        /// will cause Prince to generate page_01.png, page_02.png, ..., page_10.png etc.).
        /// </param>
        /// <returns>true if the input was successfully rasterized.</returns>
        public bool Rasterize(List<string> inputPaths, string outputPath)
        {
            List<string> cmdLine = GetJobCommandLine("normal");
            cmdLine.AddRange(inputPaths);
            cmdLine.Add(ToCommand("raster-output", outputPath));

            using (Process process = StartPrince(cmdLine))
            {
                return ReadMessages(process.StandardError);
            }
        }

        /// <summary>
        /// Rasterize an XML or HTML file.
        /// </summary>
        /// <param name="inputPath">The filename of the input XML or HTML document.</param>
        /// <param name="output">
        /// The output Stream to which Prince will write the raster output.
        /// </param>
        /// <returns>true if the input was successfully rasterized.</returns>
        public bool Rasterize(string inputPath, Stream output)
        {
            return Rasterize(new List<string> { inputPath }, output);
        }

        /// <summary>
        /// Rasterize multiple XML or HTML files.
        /// </summary>
        /// <param name="inputPaths">The filenames of the input XML or HTML documents.</param>
        /// <param name="output">
        /// The output Stream to which Prince will write the raster output.
        /// </param>
        /// <returns>true if the input was successfully rasterized.</returns>
        public bool Rasterize(List<string> inputPaths, Stream output)
        {
            if (RasterPage < 1)
            {
                throw new ApplicationException("RasterPage has to be > 0.");
            }
            if (RasterFormat == null || RasterFormat == RasterFormat.Auto)
            {
                throw new ApplicationException("RasterFormat has to be set to Jpeg or Png.");
            }

            List<string> cmdLine = GetJobCommandLine("buffered");
            cmdLine.AddRange(inputPaths);
            cmdLine.Add(ToCommand("raster-output", "-"));

            using (Process process = StartPrince(cmdLine))
            {
                using (Stream fromPrince = process.StandardOutput.BaseStream)
                {
                    fromPrince.CopyTo(output);
                }
                return ReadMessages(process.StandardError);
            }
        }

        /// <summary>
        /// Rasterize an XML or HTML stream.
        /// </summary>
        /// <param name="input">
        /// The input Stream from which Prince will read the XML or HTML document.
        /// </param>
        /// <param name="output">
        /// The output Stream to which Prince will write the raster output.
        /// </param>
        /// <returns>true if the input was successfully rasterized.</returns>
        public bool Rasterize(Stream input, Stream output)
        {
            if (InputType == null || InputType == InputType.Auto)
            {
                throw new ApplicationException("InputType has to be set to Xml or Html.");
            }
            if (RasterPage < 1)
            {
                throw new ApplicationException("RasterPage has to be > 0.");
            }
            if (RasterFormat == null || RasterFormat == RasterFormat.Auto)
            {
                throw new ApplicationException("RasterFormat has to be set to Jpeg or Png.");
            }

            List<string> cmdLine = GetJobCommandLine("buffered");
            cmdLine.Add(ToCommand("raster-output", "-"));
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

        /// <summary>
        /// Rasterize multiple XML or HTML files by reading an input list from a specified file.
        /// (An input list is a newline-separated sequence of file paths / URLs.)
        /// </summary>
        /// <param name="inputListPath">The path to the input list file.</param>
        /// <param name="outputPath">
        /// A template string from which the raster files will be named (e.g. "page_%02d.png"
        /// will cause Prince to generate page_01.png, page_02.png, ..., page_10.png etc.).
        /// </param>
        /// <returns>true if the input was successfully rasterized.</returns>
        public bool RasterizeInputList(string inputListPath, string outputPath)
        {
            List<string> cmdLine = GetJobCommandLine("normal");
            cmdLine.Add(ToCommand("input-list", inputListPath));
            cmdLine.Add(ToCommand("raster-output", outputPath));

            using (Process process = StartPrince(cmdLine))
            {
                return ReadMessages(process.StandardError);
            }
        }

        /// <summary>
        /// Rasterize multiple XML or HTML files by reading an input list from a specified file.
        /// (An input list is a newline-separated sequence of file paths / URLs.)
        /// </summary>
        /// <param name="inputListPath">The path to the input list file.</param>
        /// <param name="output">
        /// The output Stream to which Prince will write the raster output.
        /// </param>
        /// <returns>true if the input was successfully rasterized.</returns>
        public bool RasterizeInputList(string inputListPath, Stream output)
        {
            if (RasterPage < 1)
            {
                throw new ApplicationException("RasterPage has to be > 0.");
            }
            if (RasterFormat == null || RasterFormat == RasterFormat.Auto)
            {
                throw new ApplicationException("RasterFormat has to be set to Jpeg or Png.");
            }

            List<string> cmdLine = GetJobCommandLine("buffered");
            cmdLine.Add(ToCommand("input-list", inputListPath));
            cmdLine.Add(ToCommand("raster-output", "-"));

            using (Process process = StartPrince(cmdLine))
            {
                using (Stream fromPrince = process.StandardOutput.BaseStream)
                {
                    fromPrince.CopyTo(output);
                }
                return ReadMessages(process.StandardError);
            }
        }

        /// <summary>
        /// Rasterize an XML or HTML string.
        /// </summary>
        /// <param name="input">The XML or HTML document in the form of a string.</param>
        /// <param name="outputPath">
        /// A template string from which the raster files will be named (e.g. "page_%02d.png"
        /// will cause Prince to generate page_01.png, page_02.png, ..., page_10.png etc.).
        /// </param>
        /// <returns>true if the input was successfully rasterized.</returns>
        public bool RasterizeString(string input, string outputPath)
        {
            if (InputType == null || InputType == InputType.Auto)
            {
                throw new ApplicationException("InputType has to be set to Xml or Html.");
            }

            List<string> cmdLine = GetJobCommandLine("buffered");
            cmdLine.Add(ToCommand("raster-output", outputPath));
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

        /// <summary>
        /// Rasterize an XML or HTML string.
        /// </summary>
        /// <param name="input">The XML or HTML document in the form of a string.</param>
        /// <param name="output">
        /// The output Stream to which Prince will write the raster output.
        /// </param>
        /// <returns>true if the input was successfully rasterized.</returns>
        public bool RasterizeString(string input, Stream output)
        {
            using (MemoryStream inputStream =
                new MemoryStream(Encoding.UTF8.GetBytes(input)))
            {
                return Rasterize(inputStream, output);
            }
        }

        private List<string> GetJobCommandLine(string logType)
        {
            List<string> cmdLine = GetBaseCommandLine();

            cmdLine.Add(ToCommand("structured-log", logType));

            if (InputType != null) { cmdLine.Add(ToCommand("input", InputType)); }
            if (BaseUrl != null) { cmdLine.Add(ToCommand("baseurl", BaseUrl)); }
            if (Remaps.Any()) { cmdLine.AddRange(ToCommands("remap", Remaps.Select((u, d) => $"{u}={d}"))); }
            if (Iframes) { cmdLine.Add(ToCommand("iframes")); }
            if (XInclude) { cmdLine.Add(ToCommand("xinclude")); }
            if (XmlExternalEntities) { cmdLine.Add(ToCommand("xml-external-entities")); }

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
            if (PdfScript != null) { cmdLine.Add(ToCommand("pdf-script", PdfScript)); }
            foreach (KeyValuePair<PdfEvent, string> p in PdfEventScripts)
            {
                cmdLine.Add(ToCommand("pdf-event-script", p.Key + ":" + p.Value));
            }
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
            if (PdfForms) { cmdLine.Add(ToCommand("pdf-forms")); }
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
