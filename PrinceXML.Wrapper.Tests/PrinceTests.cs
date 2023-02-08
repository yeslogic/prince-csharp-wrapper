using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Xunit;

using PrinceXML.Wrapper.Enums;
using PrinceXML.Wrapper.Events;

namespace PrinceXML.Wrapper.Tests
{
    // End-to-end tests. Disabled by default.
    public class PrinceTests
    {
        // Hack to disable tests. Comment to re-enable.
        private class FactAttribute : Attribute {}

        public class Events : PrinceEvents
        {
            public string Msg { get; private set; } = "";

            public void OnMessage(MessageType msgType, string msgLocation, string msgText)
            {
                Msg = msgType.ToString() + " " + msgLocation + " " + msgText;
            }

            public void OnDataMessage(string name, string value)
            {
                Msg = "DAT " + name + " " + value;
            }
        }

        // Edit path accordingly.
        public static string PrincePath = "";
        // Probably flaky?
        public static string ResourcesDir =
            Directory.GetParent(Environment.CurrentDirectory)!.Parent!.Parent!.ToString()
                + Path.DirectorySeparatorChar
                + "resources"
                + Path.DirectorySeparatorChar;
        public static string InputPath = ResourcesDir + "convert-1.html";

        public Prince P;
        public Events E;

        // Setup for each test.
        public PrinceTests()
        {
            E = new Events();
            P = new Prince(PrincePath, E);

            P.StyleSheets.Add(ResourcesDir + "convert-1.css");
            P.JavaScript = true;
        }

        [Fact]
        public void Convert1()
        {
            Assert.True(P.Convert(InputPath), E.Msg);
        }

        [Fact]
        public void Convert2()
        {
            Assert.True(P.Convert(InputPath, ResourcesDir + "convert-2.pdf"), E.Msg);
        }

        [Fact]
        public void Convert3()
        {
            List<string> inputPaths = new List<string>() { InputPath, InputPath };
            Assert.True(P.Convert(inputPaths, ResourcesDir + "convert-3.pdf"), E.Msg);
        }

        [Fact]
        public void Convert4()
        {
            using (FileStream fs = File.Create(ResourcesDir + "convert-4.pdf"))
            {
                Assert.True(P.Convert(InputPath, fs), E.Msg);
            }
        }

        [Fact]
        public void Convert5()
        {
            using (FileStream fs = File.Create(ResourcesDir + "convert-5.pdf"))
            {
                List<string> inputPaths = new List<string>() { InputPath, InputPath };
                Assert.True(P.Convert(inputPaths, fs), E.Msg);
            }
        }

        [Fact]
        public void Convert6()
        {
            using (FileStream fis = new FileStream(InputPath, FileMode.Open, FileAccess.Read))
            using (FileStream fos = File.Create(ResourcesDir + "convert-6.pdf"))
            {
                P.InputType = InputType.Html;
                Assert.True(P.Convert(fis, fos), E.Msg);
            }
        }

        [Fact]
        public void ConvertString1()
        {
            string input = File.ReadAllText(InputPath, Encoding.UTF8);
            P.InputType = InputType.Html;
            Assert.True(P.ConvertString(input, ResourcesDir + "convertstring-1.pdf"), E.Msg);
        }

        [Fact]
        public void ConvertString2()
        {
            using (FileStream fs = File.Create(ResourcesDir + "convertstring-2.pdf"))
            {
                string input = File.ReadAllText(InputPath, Encoding.UTF8);
                P.InputType = InputType.Html;
                Assert.True(P.ConvertString(input, fs), E.Msg);
            }
        }

        [Fact]
        public void Rasterize1()
        {
            Assert.True(P.Rasterize(InputPath, ResourcesDir + "rasterize-1-page%d.png"), E.Msg);
        }

        [Fact]
        public void Rasterize2()
        {
            List<string> inputPaths = new List<string>() { InputPath, InputPath };
            Assert.True(P.Rasterize(inputPaths, ResourcesDir + "rasterize-2-page%d.png"), E.Msg);
        }

        [Fact]
        public void Rasterize3()
        {
            using (FileStream fs = File.Create(ResourcesDir + "rasterize-3.png"))
            {
                P.RasterPage = 1;
                P.RasterFormat = RasterFormat.Png;
                Assert.True(P.Rasterize(InputPath, fs), E.Msg);
            }
        }

        [Fact]
        public void Rasterize4()
        {
            using (FileStream fs = File.Create(ResourcesDir + "rasterize-4.png"))
            {
                List<string> inputPaths = new List<string>() { InputPath, InputPath };
                P.RasterPage = 2;
                P.RasterFormat = RasterFormat.Png;
                Assert.True(P.Rasterize(inputPaths, fs), E.Msg);
            }
        }

        [Fact]
        public void Rasterize5()
        {
            using (FileStream fis = new FileStream(InputPath, FileMode.Open, FileAccess.Read))
            using (FileStream fos = File.Create(ResourcesDir + "rasterize-5.png"))
            {
                P.InputType = InputType.Html;
                P.RasterPage = 1;
                P.RasterFormat = RasterFormat.Png;
                Assert.True(P.Rasterize(fis, fos), E.Msg);
            }
        }

        [Fact]
        public void RasterizeString1()
        {
            string input = File.ReadAllText(InputPath, Encoding.UTF8);
            P.InputType = InputType.Html;
            Assert.True(P.RasterizeString(input, ResourcesDir + "rasterizestring-1-page%d.png"), E.Msg);
        }

        [Fact]
        public void RasterizeString2()
        {
            using (FileStream fs = File.Create(ResourcesDir + "rasterizestring-2.png"))
            {
                string input = File.ReadAllText(InputPath, Encoding.UTF8);
                P.InputType = InputType.Html;
                P.RasterPage = 1;
                P.RasterFormat = RasterFormat.Png;
                Assert.True(P.RasterizeString(input, fs), E.Msg);
            }
        }

        [Fact]
        public void BaseOptionKeys()
        {
            P.Verbose = true;
            P.Debug = true;
            // Creates a log file in project root.
            P.Log = "x";
            P.NoWarnCssUnknown = true;
            P.NoWarnCssUnsupported = true;

            P.NoNetwork = true;
            P.NoRedirects = true;
            P.AuthUser = "x";
            P.AuthPassword = "x";
            P.AuthServer = "x";
            P.AuthScheme = AuthScheme.Https;
            P.AuthMethods.Add(AuthMethod.Basic);
            P.AuthMethods.Add(AuthMethod.Digest);
            P.NoAuthPreemptive = true;
            P.HttpProxy = "x";
            P.HttpTimeout = 100;
            P.Cookies.Add("x");
            P.Cookies.Add("y");
            P.CookieJar = "x";
            P.SslCaCert = "x";
            P.SslCaPath = "x";
            P.SslCert = "x";
            P.SslCertType = SslType.Der;
            P.SslKey = "x";
            P.SslKeyType = SslType.Pem;
            P.SslKeyPassword = "x";
            P.SslVersion = SslVersion.TlsV1_3;
            P.Insecure = true;
            P.NoParallelDownloads = true;

            P.LicenseFile = "x";
            P.LicenseKey = "x";

            // Discard output. Just tests that all option keys are correct.
            using (MemoryStream ms = new MemoryStream())
            {
                Assert.True(P.Convert(InputPath, ms), E.Msg);
            }
        }

        [Fact]
        public void FailSafe()
        {
            Assert.False(P.FailDroppedContent);
            Assert.False(P.FailMissingResources);
            Assert.False(P.FailStrippedTransparency);
            Assert.False(P.FailMissingGlyphs);
            Assert.False(P.FailPdfProfileError);
            Assert.False(P.FailPdfTagError);
            Assert.False(P.FailInvalidLicense);

            P.FailSafe(true);

            Assert.True(P.FailDroppedContent);
            Assert.True(P.FailMissingResources);
            Assert.True(P.FailStrippedTransparency);
            Assert.True(P.FailMissingGlyphs);
            Assert.True(P.FailPdfProfileError);
            Assert.True(P.FailPdfTagError);
            Assert.True(P.FailInvalidLicense);

            // Discard output.
            using (MemoryStream ms = new MemoryStream())
            {
                Assert.True(P.Convert(InputPath, ms), E.Msg);
            }
        }

        [Fact]
        public void JobOptionKeys()
        {
            P.InputType = InputType.Html;
            P.BaseUrl = ".";
            P.Remaps.Add(("x", "y"));
            P.Remaps.Add(("i", "j"));
            P.Iframes = true;
            P.XInclude = true;
            P.XmlExternalEntities = true;
            P.NoLocalFiles = true;

            P.JavaScript = true;
            P.Scripts.Add("x");
            P.Scripts.Add("y");
            P.MaxPasses = 5;

            P.StyleSheets.Add("x");
            P.StyleSheets.Add("y");
            P.Media = "x";
            P.PageSize = "x";
            P.PageMargin = "x";
            P.NoAuthorStyle = true;
            P.NoDefaultStyle = true;

            P.PdfId = "x";
            P.PdfLang = "x";
            P.PdfProfile = PdfProfile.PdfA_1A;
            P.PdfOutputIntent = "x";
            P.PdfScript = "x";
            P.PdfEventScripts[PdfEvent.WillPrint] = "w";
            P.PdfEventScripts[PdfEvent.WillClose] = "x";
            P.PdfEventScripts[PdfEvent.WillClose] = "y";
            P.PdfEventScripts[PdfEvent.DidPrint] = "z";
            P.AddFileAttachment("x");
            P.AddFileAttachment("y");
            P.NoArtificialFonts = true;
            P.NoEmbedFonts = true;
            P.NoSubsetFonts = true;
            // Fails conversion if enabled, due to being unable to find any fonts.
            // P.NoSystemFonts = true;
            P.ForceIdentityEncoding = true;
            P.NoCompress = true;
            P.NoObjectStreams = true;
            P.ConvertColors = true;
            P.FallbackCmykProfile = "x";
            P.TaggedPdf = true;
            P.PdfForms = true;
            P.CssDpi = 100;

            P.PdfTitle = "x";
            P.PdfSubject = "x";
            P.PdfAuthor = "x";
            P.PdfKeywords = "x";
            P.PdfCreator = "x";
            P.Xmp = "x";

            P.Encrypt = true;
            P.KeyBits = KeyBits.Bits40;
            P.UserPassword = "x";
            P.OwnerPassword = "x";
            P.DisallowPrint = true;
            P.DisallowCopy = true;
            P.AllowCopyForAccessibility = true;
            P.DisallowAnnotate = true;
            P.DisallowModify = true;
            P.AllowAssembly = true;

            P.RasterFormat = RasterFormat.Jpeg;
            P.RasterJpegQuality = 100;
            P.RasterPage = 100;
            P.RasterDpi = 100;
            P.RasterThreads = 100;
            P.RasterBackground = RasterBackground.White;

            // Discard output. Just tests that all option keys are correct.
            using (MemoryStream ms = new MemoryStream())
            {
                Assert.True(P.Convert(InputPath, ms), E.Msg);
            }
        }
    }
}
