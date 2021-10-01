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

    public abstract class PrinceBase
    {
        private string _princePath;
        private PrinceEvents _events;

        // Logging options.
        public bool Verbose { get; set; }
        public bool Debug { get; set; }
        public string Log { get; set; }
        public bool NoWarnCssUnknown { get; set; }
        public bool NoWarnCssUnsupported { get; set; }

        // Input options.
        public InputType InputType { get; set; }
        public string BaseUrl { get; set; }
        public bool XInclude { get; set; }
        public bool XmlExternalEntities { get; set; }

        // Network options.
        public bool NoNetwork { get; set; }
        public bool NoRedirects { get; set; }
        public string AuthUser { get; set; }
        public string AuthPassword { get; set; }
        public string AuthServer { get; set; }
        public AuthScheme AuthScheme { get; set; }
        public List<AuthMethod> AuthMethods { get; } = new List<AuthMethod>();
        public bool NoAuthPreemptive { get; set; }
        public string HttpProxy { get; set; }
        public int HttpTimeout { get; set; }
        public List<string> Cookies { get; } = new List<string>();
        public string CookieJar { get; set; }
        public string SslCaCert { get; set; }
        public string SslCaPath { get; set; }
        public string SslCert { get; set; }
        public SslType SslCertType { get; set; }
        public string SslKey { get; set; }
        public SslType SslKeyType { get; set; }
        public string SslKeyPassword { get; set; }
        public SslVersion SslVersion { get; set; }
        public bool Insecure { get; set; }
        public bool NoParallelDownloads { get; set; }

        // JavaScript options.
        public bool JavaScript { get; set; }
        public List<string> Scripts { get; } = new List<string>();
        public int MaxPasses { get; set; }

        // CSS options.
        public List<string> StyleSheets { get; } = new List<string>();
        public string Media { get; set; }
        public bool NoAuthorStyle { get; set; }
        public bool NoDefaultStyle { get; set; }

        // PDF output options.
        public string PdfId { get; set; }
        public string PdfLang { get; set; }
        public PdfProfile PdfProfile { get; set; }
        public string PdfOutputIntent { get; set; }
        public List<FileAttachment> FileAttachments { get; } = new List<FileAttachment>();
        public bool NoArtificialFonts { get; set; }
        public bool NoEmbedFonts { get; set; }
        public bool NoSubsetFonts { get; set; }
        public bool ForceIdentityEncoding { get; set; }
        public bool NoCompress { get; set; }
        public bool NoObjectStreams { get; set; }
        public bool ConvertColors { get; set; }
        public string FallbackCmykProfile { get; set; }
        public bool TaggedPdf { get; set; }

        // PDF metadata options.
        public string PdfTitle { get; set; }
        public string PdfSubject { get; set; }
        public string PdfAuthor { get; set; }
        public string PdfKeywords { get; set; }
        public string PdfCreator { get; set; }
        public string Xmp { get; set; }

        // PDF encryption options.
        public bool Encrypt { get; set; }
        public KeyBits? KeyBits { get; set; }
        public string UserPassword { get; set; }
        public string OwnerPassword { get; set; }
        public bool DisallowPrint { get; set; }
        public bool DisallowCopy { get; set; }
        public bool AllowCopyForAccessibility { get; set; }
        public bool DisallowAnnotate { get; set; }
        public bool DisallowModify { get; set; }
        public bool AllowAssembly { get; set; }

        // License options.
        public string LicenseFile { get; set; }
        public string LicenseKey { get; set; }

        protected PrinceBase(string princePath, PrinceEvents events = null) =>
            (_princePath, _events) = (princePath, events);

        public abstract bool Convert(string inputPath, Stream output);

        public abstract bool Convert(List<string> inputPaths, Stream output);

        public abstract bool Convert(Stream input, Stream output);

        public abstract bool ConvertString(string input, Stream output);

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

            return cmdLine;
        }

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
                    // Ignore too-short messages.
                }
                line = reader.ReadLine();
            }

            return result.Equals("success");
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
    }

    public class FileAttachment
    {
        public string Url { get; }
        public string FileName { get; }
        public string Description { get; }

        public FileAttachment(string url, string fileName = null, string description = null) =>
            (Url, FileName, Description) = (url, fileName, description);
    }
}
