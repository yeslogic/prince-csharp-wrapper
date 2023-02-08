using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace PrinceXML.Wrapper
{
    using Enums;
    using Events;
    using static Util.Arguments;
    using static Util.CommandLine;

    /// <summary>Base class for <c>Prince</c> and <c>PrinceControl</c>.</summary>
    public abstract class PrinceBase
    {
        private readonly string _princePath;
        private readonly PrinceEvents _events;

        // Logging options.
        /// <summary>
        /// Enable logging of informative messages.
        /// </summary>
        /// <value>true if verbose logging is enabled. Default value is false.</value>
        public bool Verbose { get; set; }
        /// <summary>
        /// Enable logging of debug messages.
        /// </summary>
        /// <value>true if debug logging is enabled. Default value is false.</value>
        public bool Debug { get; set; }
        /// <summary>
        /// Specify a file that Prince should use to log messages. If this property
        /// is not set, then Prince will not write to any log. This property does
        /// not affect the operation of <c>PrinceEvents</c>, which will also receive
        /// messages from Prince.
        /// </summary>
        /// <value>The filename that Prince uses to log messages.</value>
        public string Log { get; set; }
        /// <summary>
        /// Disable warnings about unknown CSS features.
        /// </summary>
        /// <value>true if warnings are disabled. Default value is false.</value>
        public bool NoWarnCssUnknown { get; set; }
        /// <summary>
        /// Disable warnings about unsupported CSS features.
        /// </summary>
        /// <value>true if warnings are disabled. Default value is false.</value>
        public bool NoWarnCssUnsupported { get; set; }

        // Input options.
        /// <summary>
        /// Specify the input type of the document. Setting this to <c>InputType.Xml</c>
        /// or <c>InputType.Html</c> is required if a document is provided via a
        /// <c>Stream</c> or <c>string</c>, as the types of these documents cannot
        /// be determined.
        /// </summary>
        /// <value>The document's input type. Default value is <c>InputType.Auto</c>.</value>
        public InputType InputType { get; set; }
        /// <summary>
        /// Specify the base URL of the input document. This can be used to override
        /// the path of the input document, which is convenient when processing local
        /// copies of a document from a website.
        /// </summary>
        /// <value>The base URL or path of the input document.</value>
        public string BaseUrl { get; set; }
        /// <summary>
        /// Enable HTML iframes.
        /// </summary>
        /// <value>true if HTML iframes is enabled. Default value is false.</value>
        public bool Iframes { get; set; }
        /// <summary>
        /// Enable XInclude and XML external entities (XXE). Note that XInclude only
        /// applies to XML files. To apply it to HTML files, the input format needs
        /// to be specified by setting <c>InputType</c>.
        /// </summary>
        /// <value>true if XInclude and XXE are enabled. Default value is false.</value>
        public bool XInclude { get; set; }
        /// <summary>
        /// Enable XML external entities (XXE).
        /// </summary>
        /// <value>true if XXE is enabled. Default value is false.</value>
        public bool XmlExternalEntities { get; set; }

        // Network options.
        /// <summary>
        /// Disable network access (prevents HTTP downloads).
        /// </summary>
        /// <value>true if network access is disabled. Default value is false.</value>
        public bool NoNetwork { get; set; }
        /// <summary>
        /// Disable all HTTP and HTTPS redirects.
        /// </summary>
        /// <value>true if redirects are disabled. Default value is false.</value>
        public bool NoRedirects { get; set; }
        /// <summary>
        /// Specify the username for HTTP authentication.
        /// </summary>
        /// <value>The username for authentication.</value>
        public string AuthUser { get; set; }
        /// <summary>
        /// Specify the password for HTTP authentication.
        /// </summary>
        /// <value>The password for authentication.</value>
        public string AuthPassword { get; set; }
        /// <summary>
        /// Send username and password credentials to the specified server only.
        /// </summary>
        /// <value>
        /// The server to send credentials to (e.g. "localhost:8001"). The default
        /// is to send them to any server which challenges for authentication.
        /// </value>
        public string AuthServer { get; set; }
        /// <summary>
        /// Send username and password credentials only for requests with the
        /// given scheme.
        /// </summary>
        /// <value>The authentication scheme.</value>
        public AuthScheme AuthScheme { get; set; }
        /// <summary>
        /// HTTP authentication methods to enable.
        /// </summary>
        /// <value>The authentication method to enable.</value>
        public List<AuthMethod> AuthMethods { get; } = new List<AuthMethod>();
        /// <summary>
        /// Do not authenticate with named servers until asked.
        /// </summary>
        /// <value>true if authentication preemptive is disabled. Default value is false.</value>
        public bool NoAuthPreemptive { get; set; }
        /// <summary>
        /// Specify the URL for the HTTP proxy server, if needed.
        /// </summary>
        /// <value>The URL for the HTTP proxy server.</value>
        public string HttpProxy { get; set; }
        /// <summary>
        /// Specify the timeout for HTTP requests.
        /// </summary>
        /// <value>
        /// The HTTP timeout in seconds. Value must be greater than 0.
        /// Default value is false.
        /// </value>
        public int HttpTimeout { get; set; }
        /// <summary>
        /// Add a value for the <c>Set-Cookie</c> HTTP header value.
        /// </summary>
        /// <value>The cookie to be added.</value>
        public List<string> Cookies { get; } = new List<string>();
        /// <summary>
        /// Specify a file containing HTTP cookies.
        /// </summary>
        /// <value>The filename of the file containing HTTP cookies.</value>
        public string CookieJar { get; set; }
        /// <summary>
        /// Specify an SSL certificate file.
        /// </summary>
        /// <value>The filename of the SSL certificate file.</value>
        public string SslCaCert { get; set; }
        /// <summary>
        /// Specify an SSL certificate directory.
        /// </summary>
        /// <value>The SSL certificate directory.</value>
        public string SslCaPath { get; set; }
        /// <summary>
        /// Specify an SSL client certificate file. On MacOS, specify a PKCS#12
        /// file containing a client certificate and private key. Client authentication
        /// is not supported on Windows.
        /// </summary>
        /// <value>The filename of the SSL client certificate file.</value>
        public string SslCert { get; set; }
        /// <summary>
        /// Specify the SSL client certificate file type. This option is not
        /// supported on MacOS or Windows.
        /// </summary>
        /// <value>The SSL client certificate file type.</value>
        public SslType SslCertType { get; set; }
        /// <summary>
        ///  Specify an SSL private key file. This option is not supported on MacOS
        /// or Windows.
        /// </summary>
        /// <value>The filename of the SSL private key file.</value>
        public string SslKey { get; set; }
        /// <summary>
        /// Specify the SSL private key file type. This option is not supported on
        /// MacOS or Windows.
        /// </summary>
        /// <value>The SSL private key file type.</value>
        public SslType SslKeyType { get; set; }
        /// <summary>
        /// Specify a password for the SSL private key.
        /// </summary>
        /// <value>The password for the SSL private key.</value>
        public string SslKeyPassword { get; set; }
        /// <summary>
        /// Set the minimum version of SSL to allow.
        /// </summary>
        /// <value>
        /// The minimum version to allow. Default value is <c>SslVersion.Default</c>.
        /// </value>
        public SslVersion SslVersion { get; set; }
        /// <summary>
        /// Specify whether to disable SSL verification.
        /// </summary>
        /// <value>true if SSL verification is disabled. Default value is false.</value>
        public bool Insecure { get; set; }
        /// <summary>
        /// Disable downloading multiple HTTP resources at once.
        /// </summary>
        /// <value>true if parallel downloads are disabled. Default value is false.</value>
        public bool NoParallelDownloads { get; set; }

        // JavaScript options.
        /// <summary>
        /// Specify whether JavaScript found in documents should be run.
        /// </summary>
        /// <value>true if document scripts should run. Default value is false.</value>
        public bool JavaScript { get; set; }
        /// <summary>
        /// JavaScript scripts that will run before conversion.
        /// </summary>
        /// <value>The filename of the script to run.</value>
        public List<string> Scripts { get; } = new List<string>();
        /// <summary>
        /// Defines the maximum number of consequent layout passes.
        /// </summary>
        /// <value>
        /// The number of maximum passes. Value must be greater than 0.
        /// Default value is unlimited passes.
        /// </value>
        public int MaxPasses { get; set; }

        // CSS options.
        /// <summary>
        /// CSS style sheets that will be applied to each input document.
        /// </summary>
        /// <value>The filename of the CSS style sheet to apply.</value>
        public List<string> StyleSheets { get; } = new List<string>();
        /// <summary>
        /// Specify the media type.
        /// </summary>
        /// <value>The media type (e.g. "print", "screen"). Default value is "print".</value>
        public string Media { get; set; }
        /// <summary>
        /// Ignore author style sheets.
        /// </summary>
        /// <value>true if author style sheets are ignored. Default value is false.</value>
        public bool NoAuthorStyle { get; set; }
        /// <summary>
        /// Ignore default style sheets.
        /// </summary>
        /// <value>true if default style sheets are ignored. Default value is false.</value>
        public bool NoDefaultStyle { get; set; }

        // PDF output options.
        /// <summary>
        /// Specify the PDF ID to use.
        /// </summary>
        /// <value>The PDF ID.</value>
        public string PdfId { get; set; }
        /// <summary>
        /// Specify the PDF document's Lang entry in the document catalog.
        /// </summary>
        /// <value>The PDF document's Lang entry.</value>
        public string PdfLang { get; set; }
        /// <summary>
        /// Specify the PDF profile to use.
        /// </summary>
        /// <value>The PDF profile.</value>
        public PdfProfile PdfProfile { get; set; }
        /// <summary>
        /// Specify the ICC profile to use.
        /// </summary>
        /// <value>The ICC profile.</value>
        public string PdfOutputIntent { get; set; }
        /// <summary>
        /// Include an AcroJS script to run when the PDF is opened.
        /// </summary>
        /// <value>The filename or URL of the AcroJS script.</value>
        public string PdfScript { get; set; }
        /// <summary>
        /// Include an AcrosJS script to run on a specific event.
        /// </summary>
        /// <value>A mapping of PDF events to the filenames or URLs of the AcroJS scripts.</value>
        public Dictionary<PdfEvent, string> PdfEventScripts { get; } = new Dictionary<PdfEvent, string>();
        /// <summary>
        /// File attachments that will be attached to the PDF file.
        /// </summary>
        /// <value>The file attachment.</value>
        public List<FileAttachment> FileAttachments { get; } = new List<FileAttachment>();
        /// <summary>
        /// Specify whether artificial bold/italic fonts should be generated if
        /// necessary.
        /// </summary>
        /// <value>
        /// true if artificial bold/italic fonts are disabled. Default value is false.
        /// </value>
        public bool NoArtificialFonts { get; set; }
        /// <summary>
        /// Specify whether fonts should be embedded in the output PDF file.
        /// </summary>
        /// <value>true if PDF font embedding is disabled. Default value is false.</value>
        public bool NoEmbedFonts { get; set; }
        /// <summary>
        /// Specify whether embedded fonts should be subset in the output PDF file.
        /// </summary>
        /// <value>true if PDF font subsetting is disabled. Default value is false.</value>
        public bool NoSubsetFonts { get; set; }
        /// <summary>
        /// Ensure that all fonts are encoded in the PDF using their identity encoding
        /// (directly mapping to glyph indices), even if they could have used MacRoman
        /// or some other encoding.
        /// </summary>
        /// <value>true if identity encoding is forced. Default value is false.</value>
        public bool ForceIdentityEncoding { get; set; }
        /// <summary>
        /// Specify whether compression should be applied to the output PDF file.
        /// </summary>
        /// <value>true if PDF compression is disabled. Default value is false.</value>
        public bool NoCompress { get; set; }
        /// <summary>
        /// Disable PDF object streams.
        /// </summary>
        /// <value>true if PDF object streams are disabled. Default value is false.</value>
        public bool NoObjectStreams { get; set; }
        /// <summary>
        /// Convert colors to output intent color space.
        /// </summary>
        /// <value>
        /// true if colors are converted to output intent color space.
        ///  Default value is false.
        /// </value>
        public bool ConvertColors { get; set; }
        /// <summary>
        /// Set fallback ICC profile for uncalibrated CMYK.
        /// </summary>
        /// <value>The fallback ICC profile.</value>
        public string FallbackCmykProfile { get; set; }
        /// <summary>
        /// Enable tagged PDF.
        /// </summary>
        /// <value>true if tagged PDF is enabled. Default value is false.</value>
        public bool TaggedPdf { get; set; }
        /// <summary>
        /// Enable PDF forms by default.
        /// </summary>
        /// <value>true if PDF forms is enabled by default. Default value is false.</value>
        public bool PdfForms { get; set; }

        // PDF metadata options.
        /// <summary>
        /// Specify the document title for PDF metadata.
        /// </summary>
        /// <value>The document title.</value>
        public string PdfTitle { get; set; }
        /// <summary>
        /// Specify the document subject for PDF metadata.
        /// </summary>
        /// <value>The document subject.</value>
        public string PdfSubject { get; set; }
        /// <summary>
        /// Specify the document author for PDF metadata.
        /// </summary>
        /// <value>The document author.</value>
        public string PdfAuthor { get; set; }
        /// <summary>
        /// Specify the document keywords for PDF metadata.
        /// </summary>
        /// <value>The document keywords.</value>
        public string PdfKeywords { get; set; }
        /// <summary>
        /// Specify the document creator for PDF metadata.
        /// </summary>
        /// <value>The document creator.</value>
        public string PdfCreator { get; set; }
        /// <summary>
        /// Specify an XMP file that contains XMP metadata to be included in the
        /// output PDF file.
        /// </summary>
        /// <value>The filename of the XMP file.</value>
        public string Xmp { get; set; }

        // PDF encryption options.
        /// <summary>
        /// Specify whether encryption should be applied to the output file.
        /// </summary>
        /// <value>true if PDF encryption is enabled. Default value is false.</value>
        public bool Encrypt { get; set; }
        /// <summary>
        /// Specify the size of the encryption key.
        /// </summary>
        /// <value>
        /// The size of the encryption key. Default value is <c>KeyBits.Bits128</c>.
        /// </value>
        public KeyBits? KeyBits { get; set; }
        /// <summary>
        /// Specify the user password for the PDF file.
        /// </summary>
        /// <value>The user password.</value>
        public string UserPassword { get; set; }
        /// <summary>
        /// Specify the owner password for the PDF file.
        /// </summary>
        /// <value>The owner password.</value>
        public string OwnerPassword { get; set; }
        /// <summary>
        /// Disallow printing of the PDF file.
        /// </summary>
        /// <value>true if printing is disallowed. Default value is false.</value>
        public bool DisallowPrint { get; set; }
        /// <summary>
        /// Disallow modification of the PDF file.
        /// </summary>
        /// <value>true if modification is disallowed. Default value is false.</value>
        public bool DisallowCopy { get; set; }
        /// <summary>
        /// Used together with <c>DisallowCopy</c>, which creates an exception by
        /// enabling text access for screen reader devices for the visually impaired.
        /// </summary>
        /// <value>true if text access is allowed. Default value is false.</value>
        public bool AllowCopyForAccessibility { get; set; }
        /// <summary>
        /// Disallow annotation of the PDF file.
        /// </summary>
        /// <value>true if annotation is disallowed. Default value is false.</value>
        public bool DisallowAnnotate { get; set; }
        /// <summary>
        /// Disallow modification of the PDF file.
        /// </summary>
        /// <value>true if modification is disallowed. Default value is false.</value>
        public bool DisallowModify { get; set; }
        /// <summary>
        /// Used together with <c>DisallowModify</c>, which creates an exception.
        /// It allows the document to be inserted into another document or other
        /// pages to be added, but the content of the document cannot be modified.
        /// </summary>
        /// <value>true if assembly is allowed. Default value is false.</value>
        public bool AllowAssembly { get; set; }

        // License options.
        /// <summary>
        /// Specify the license file.
        /// </summary>
        /// <value>The filename of the license file.</value>
        public string LicenseFile { get; set; }
        /// <summary>
        /// Specify the license key. This is the <c>&lt;signature&gt;</c> field in the
        /// license file.
        /// </summary>
        /// <value>The license key.</value>
        public string LicenseKey { get; set; }

        // Advanced options.
        /// <summary>
        /// Fail-safe option that aborts the creation of a PDF if any content is
        /// dropped.
        /// </summary>
        /// <value>true if fail-safe option is enabled. Default value is false.</value>
        public bool FailDroppedContent { get; set; }
        /// <summary>
        /// Fail-safe option that aborts the creation of a PDF if any resources
        /// cannot be loaded.
        /// </summary>
        /// <value>true if fail-safe option is enabled. Default value is false.</value>
        public bool FailMissingResources { get; set; }
        /// <summary>
        /// Fail-safe option that aborts the creation of a PDF if transparent
        /// images are used with a PDF profile that does not support opacity.
        /// </summary>
        /// <value>true if fail-safe option is enabled. Default value is false.</value>
        public bool FailStrippedTransparency { get; set; }
        /// <summary>
        /// Fail-safe option that aborts the creation of a PDF if glyphs cannot
        /// be found for any characters.
        /// </summary>
        /// <value>true if fail-safe option is enabled. Default value is false.</value>
        public bool FailMissingGlyphs { get; set; }
        /// <summary>
        /// Fail-safe option that aborts the creation of a PDF if there are
        /// problems complying with the specified PDF profile.
        /// </summary>
        /// <value>true if fail-safe option is enabled. Default value is false.</value>
        public bool FailPdfProfileError { get; set; }
        /// <summary>
        /// Fail-safe option that aborts the creation of a PDF if there are
        /// problems tagging the PDF for accessibility.
        /// </summary>
        /// <value>true if fail-safe option is enabled. Default value is false.</value>
        public bool FailPdfTagError { get; set; }
        /// <summary>
        /// Fail-safe option that aborts the creation of a PDF if the Prince
        /// license is invalid or not readable.
        /// </summary>
        /// <value>true if fail-safe option is enabled. Default value is false.</value>
        public bool FailInvalidLicense { get; set; }

        /// <summary>The <c>PrinceBase</c> constructor.</summary>
        protected PrinceBase(string princePath, PrinceEvents events = null) =>
            (_princePath, _events) = (princePath, events);

        /// <summary>
        /// Convert an XML or HTML file to a PDF file.
        /// </summary>
        /// <param name="inputPath">The filename of the input XML or HTML document.</param>
        /// <param name="output">
        /// The output Stream to which Prince will write the PDF output.
        /// </param>
        /// <returns>true if a PDF file was generated successfully.</returns>
        public abstract bool Convert(string inputPath, Stream output);

        /// <summary>
        /// Convert multiple XML or HTML files to a PDF file.
        /// </summary>
        /// <param name="inputPaths">The filenames of the input XML or HTML documents.</param>
        /// <param name="output">
        /// The output Stream to which Prince will write the PDF output.
        /// </param>
        /// <returns>true if a PDF file was generated successfully.</returns>
        public abstract bool Convert(List<string> inputPaths, Stream output);

        /// <summary>
        /// Convert an XML or HTML stream to a PDF file. Note that it may be helpful to
        /// specify a base URL or path from the input document using <c>BaseUrl</c>.
        /// This allows relative URLs and paths in the document (e.g. for images) to be
        /// resolved correctly.
        /// </summary>
        /// <param name="input">
        /// The input Stream from which Prince will read the XML or HTML document.
        /// </param>
        /// <param name="output">
        /// The output Stream to which Prince will write the PDF output.
        /// </param>
        /// <returns>true if a PDF file was generated successfully.</returns>
        public abstract bool Convert(Stream input, Stream output);

        /// <summary>
        /// Convert an XML or HTML string to a PDF file.
        /// </summary>
        /// <param name="input">The XML or HTML document in the form of a string.</param>
        /// <param name="output">
        /// The output Stream to which Prince will write the PDF output.
        /// </param>
        /// <returns>true if a PDF file was generated successfully.</returns>
        public abstract bool ConvertString(string input, Stream output);

        /// <summary>
        /// Add a file attachment that will be attached to the PDF file.
        /// </summary>
        /// <param name="url">The URL of the file attachment.</param>
        public void AddFileAttachment(string url)
        {
            FileAttachments.Add(new FileAttachment(url));
        }

        /// <summary>
        /// Enables/disables all fail-safe options.
        /// </summary>
        /// <param name="failSafe">The value to set all fail-safe options to.</param>
        public void FailSafe(bool failSafe)
        {
            FailDroppedContent = failSafe;
            FailMissingResources = failSafe;
            FailStrippedTransparency = failSafe;
            FailMissingGlyphs = failSafe;
            FailPdfProfileError = failSafe;
            FailPdfTagError = failSafe;
            FailInvalidLicense = failSafe;
        }

        /// <summary>
        /// Starts a Prince process.
        /// </summary>
        /// <param name="args">The command line arguments for Prince.</param>
        /// <returns>The Prince process.</returns>
        protected Process StartPrince(List<string> args)
        {
            ProcessStartInfo psi = new ProcessStartInfo()
            {
                Arguments = BuildArguments(args),
                CreateNoWindow = true,
                FileName = _princePath,
                RedirectStandardError = true,
                RedirectStandardInput = true,
                RedirectStandardOutput = true,
                UseShellExecute = false,
            };

            try
            {
                Process process = Process.Start(psi);
                if (process != null && !process.HasExited)
                {
                    return process;
                }
                throw new ApplicationException("Error starting Prince: " + _princePath);
            }
            catch (Win32Exception ex)
            {
                const int ERROR_FILE_NOT_FOUND = 2;
                const int ERROR_PATH_NOT_FOUND = 3;
                const int ERROR_ACCESS_DENIED = 5;

                string msg = ex.Message;
                switch (ex.NativeErrorCode)
                {
                    case ERROR_FILE_NOT_FOUND:
                        msg += " -- Please verify that Prince.exe is in the directory.";
                        break;
                    case ERROR_PATH_NOT_FOUND:
                        msg += " -- Please check Prince path.";
                        break;
                    case ERROR_ACCESS_DENIED:
                        msg += " -- Please check system permissions to run Prince.";
                        break;
                }

                throw new ApplicationException(msg);
            }
        }

        /// <summary>
        /// The common base command lines used by Prince and the Prince control interface.
        /// </summary>
        /// <returns>A list of base command lines.</returns>
        protected List<string> GetBaseCommandLine()
        {
            List<string> cmdLine = new List<string>();

            if (Verbose) { cmdLine.Add(ToCommand("verbose")); }
            if (Debug) { cmdLine.Add(ToCommand("debug")); }
            if (Log != null) { cmdLine.Add(ToCommand("log", Log)); }
            if (NoWarnCssUnknown) { cmdLine.Add(ToCommand("no-warn-css-unknown")); }
            if (NoWarnCssUnsupported) { cmdLine.Add(ToCommand("no-warn-css-unsupported")); }

            if (NoNetwork) { cmdLine.Add(ToCommand("no-network")); }
            if (NoRedirects) { cmdLine.Add(ToCommand("no-redirects")); }
            if (AuthUser != null) { cmdLine.Add(ToCommand("auth-user", AuthUser)); }
            if (AuthPassword != null) { cmdLine.Add(ToCommand("auth-password", AuthPassword)); }
            if (AuthServer != null) { cmdLine.Add(ToCommand("auth-server", AuthServer)); }
            if (AuthScheme != null) { cmdLine.Add(ToCommand("auth-scheme", AuthScheme)); }
            if (AuthMethods.Any()) { cmdLine.Add(ToCommandCsvs("auth-method", AuthMethods)); }
            if (NoAuthPreemptive) { cmdLine.Add(ToCommand("no-auth-preemptive")); }
            if (HttpProxy != null) { cmdLine.Add(ToCommand("http-proxy", HttpProxy)); }
            if (HttpTimeout > 0) { cmdLine.Add(ToCommand("http-timeout", HttpTimeout)); }
            if (Cookies.Any()) { cmdLine.AddRange(ToCommands("cookie", Cookies)); }
            if (CookieJar != null) { cmdLine.Add(ToCommand("cookiejar", CookieJar)); }
            if (SslCaCert != null) { cmdLine.Add(ToCommand("ssl-cacert", SslCaCert)); }
            if (SslCaPath != null) { cmdLine.Add(ToCommand("ssl-capath", SslCaPath)); }
            if (SslCert != null) { cmdLine.Add(ToCommand("ssl-cert", SslCert)); }
            if (SslCertType != null) { cmdLine.Add(ToCommand("ssl-cert-type", SslCertType)); }
            if (SslKey != null) { cmdLine.Add(ToCommand("ssl-key", SslKey)); }
            if (SslKeyType != null) { cmdLine.Add(ToCommand("ssl-key-type", SslKeyType)); }
            if (SslKeyPassword != null) { cmdLine.Add(ToCommand("ssl-key-password", SslKeyPassword)); }
            if (SslVersion != null) { cmdLine.Add(ToCommand("ssl-version", SslVersion)); }
            if (Insecure) { cmdLine.Add(ToCommand("insecure")); }
            if (NoParallelDownloads) { cmdLine.Add(ToCommand("no-parallel-downloads")); }

            if (LicenseFile != null) { cmdLine.Add(ToCommand("license-file", LicenseFile)); }
            if (LicenseKey != null) { cmdLine.Add(ToCommand("license-key", LicenseKey)); }

            if(FailDroppedContent) { cmdLine.Add(ToCommand("fail-dropped-content")); };
            if(FailMissingResources) { cmdLine.Add(ToCommand("fail-missing-resources")); };
            if(FailStrippedTransparency) { cmdLine.Add(ToCommand("fail-stripped-transparency")); };
            if(FailMissingGlyphs) { cmdLine.Add(ToCommand("fail-missing-glyphs")); };
            if(FailPdfProfileError) { cmdLine.Add(ToCommand("fail-pdf-profile-error")); };
            if(FailPdfTagError) { cmdLine.Add(ToCommand("fail-pdf-tag-error")); };
            if(FailInvalidLicense) { cmdLine.Add(ToCommand("fail-invalid-license")); };

            return cmdLine;
        }

        /// <summary>
        /// Handle (structured) messages received from Prince.
        /// </summary>
        /// <param name="reader">The reader for messages received from Prince.</param>
        /// <returns>true if Prince returns a "success" message.</returns>
        protected bool ReadMessages(StreamReader reader)
        {
            string result = "";
            string line = reader.ReadLine();

            while (line != null)
            {
                string[] tokens = line.Split(new char[] { '|' }, 2);
                if (tokens.Length == 2)
                {
                    string msgTag = tokens[0];
                    string msgBody = tokens[1];

                    switch (msgTag)
                    {
                        case "msg":
                            HandleMessage(msgBody);
                            break;
                        case "dat":
                            HandleDataMessage(msgBody);
                            break;
                        case "fin":
                            result = msgBody;
                            break;
                    }
                }
                else
                {
                    HandleNonStructuredMessage(line);
                }
                line = reader.ReadLine();
            }

            return result == "success";
        }

        private void HandleMessage(string msgBody)
        {
            if (_events == null) { return; }

            string[] tokens = msgBody.Split(new char[] { '|' }, 3);
            if (tokens.Length == 3)
            {
                MessageType msgType;
                if (Enum.TryParse(tokens[0].ToUpper(), out msgType))
                {
                    string msgLocation = tokens[1];
                    string msgText = tokens[2];

                    _events.OnMessage(msgType, msgLocation, msgText);
                }
                else
                {
                    // Ignore unknown message types.
                }
            }
            else
            {
                // Ignore incorrectly-formatted messages.
            }
        }

        private void HandleDataMessage(string msgBody)
        {
            if (_events == null) { return; }

            string[] tokens = msgBody.Split(new char[] { '|' }, 2);
            if (tokens.Length == 2)
            {
                string name = tokens[0];
                string value = tokens[1];

                _events.OnDataMessage(name, value);
            }
            else
            {
                // Ignore incorrectly-formatted messages.
            }
        }

        private void HandleNonStructuredMessage(string msg)
        {
            const string princeWrn = "prince: warning: ";
            const string princeErr = "prince: error: ";

            if (_events == null) { return; }

            if (msg.StartsWith(princeWrn))
            {
                string msgText = msg.Substring(princeWrn.Length);
                _events.OnMessage(MessageType.WRN, "", msgText);
            }
            else if (msg.StartsWith(princeErr))
            {
                string msgText = msg.Substring(princeErr.Length);
                _events.OnMessage(MessageType.ERR, "", msgText);
            }
            else
            {
                // Just treat everything else as debug messages.
                _events.OnMessage(MessageType.DBG, "", msg);
            }
        }
    }

    /// <summary>
    /// Data class for file attachments.
    /// </summary>
    public class FileAttachment
    {
        /// <value>The URL of the file attachment.</value>
        public string Url { get; }
        /// <value>The filename of the file attachment.</value>
        public string FileName { get; }
        /// <value>The description of the file attachment.</value>
        public string Description { get; }

        /// <summary>
        /// The <c>FileAttachment</c> constructor.
        /// </summary>
        /// <param name="url">The URL of the file attachment.</param>
        /// <param name="fileName">The optional filename of the file attachment.</param>
        /// <param name="description">The optional description of the file attachment.</param>
        /// <returns></returns>
        public FileAttachment(string url, string fileName = null, string description = null) =>
            (Url, FileName, Description) = (url, fileName, description);
    }
}
