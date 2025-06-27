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
    public class PrinceControlTests
    {
        // Hack to disable tests. Comment to re-enable.
        private class FactAttribute : Attribute {}

        public class Events : PrinceEvents
        {
            public string Msg { get; private set; } = "";

            public void OnMessage(MessageType msgType, string msgLocation, string msgText)
            {
                Msg += msgType.ToString() + " " + msgLocation + " " + msgText + "\n";
            }

            public void OnDataMessage(string name, string value)
            {
                Msg += "DAT " + name + " " + value + "\n";
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

        public class PrinceControlInitialisedTests : IDisposable
        {
            public string InputPath = ResourcesDir + "convert-1.html";

            public PrinceControl P;
            public Events E;

            // Setup for each test.
            public PrinceControlInitialisedTests()
            {
                E = new Events();
                P = new PrinceControl(PrincePath, E);

                P.StyleSheets.Add(ResourcesDir + "convert-1.css");
                P.JavaScript = true;
                P.Start();
            }

            // Teardown for each test.
            public void Dispose()
            {
                P.Stop();
            }

            [Fact]
            public void Convert1()
            {
                using (Stream fs = File.Create(ResourcesDir + "control-convert-1.pdf"))
                {
                    Assert.True(P.Convert(InputPath, fs), E.Msg);
                }
            }

            [Fact]
            public void Convert2()
            {
                using (Stream fs = File.Create(ResourcesDir + "control-convert-2.pdf"))
                {
                    List<string> inputPaths = new List<string>() { InputPath, InputPath };
                    Assert.True(P.Convert(inputPaths, fs), E.Msg);
                }
            }

            [Fact]
            public void Convert3()
            {
                using (FileStream fis = new FileStream(InputPath, FileMode.Open, FileAccess.Read))
                using (FileStream fos = File.Create(ResourcesDir + "control-convert-3.pdf"))
                {
                    P.InputType = InputType.Html;
                    Assert.True(P.Convert(fis, fos), E.Msg);
                }
            }

            [Fact]
            public void ConvertString()
            {
                using (Stream fs = File.Create(ResourcesDir + "control-convertstring.pdf"))
                {
                    string input = File.ReadAllText(InputPath, Encoding.UTF8);
                    P.InputType = InputType.Html;
                    Assert.True(P.ConvertString(input, fs), E.Msg);
                }
            }

            [Fact]
            public void JobOptionKeys()
            {
                P.InputType = InputType.Html;
                P.BaseUrl = "x";
                P.Media = "x";
                P.StyleSheets.Add("x");
                P.StyleSheets.Add("y");
                P.Scripts.Add("x");
                P.Scripts.Add("y");
                P.NoDefaultStyle = true;
                P.NoAuthorStyle = true;
                P.JavaScript = true;
                P.MaxPasses = 5;
                P.Iframes = true;
                P.XInclude = true;
                P.XmlExternalEntities = true;

                P.NoEmbedFonts = true;
                P.NoSubsetFonts = true;
                P.NoArtificialFonts = true;
                P.ForceIdentityEncoding = true;
                P.NoCompress = true;
                P.NoObjectStreams = true;

                P.Encrypt = true;
                P.KeyBits = KeyBits.Bits40;
                P.UserPassword = "x";
                P.OwnerPassword = "x";
                P.DisallowPrint = true;
                P.DisallowModify = true;
                P.DisallowCopy = true;
                P.DisallowAnnotate = true;
                P.AllowCopyForAccessibility = true;
                P.AllowAssembly = true;

                P.PdfProfile = PdfProfile.PdfA_1A_And_PdfUA_1;
                P.PdfOutputIntent = "x";
                P.PdfScript = "x";
                P.PdfEventScripts[PdfEvent.WillPrint] = "w";
                P.PdfEventScripts[PdfEvent.WillClose] = "x";
                P.PdfEventScripts[PdfEvent.WillClose] = "y";
                P.PdfEventScripts[PdfEvent.DidPrint] = "z";
                P.FallbackCmykProfile = "x";
                P.ConvertColors = true;
                P.PdfId = "x";
                P.PdfLang = "x";
                P.Xmp = "x";
                P.TaggedPdf = true;
                P.PdfForms = true;

                P.AddFileAttachment("x");
                P.AddFileAttachment(new byte[] { 0 }, "x", "y");

                P.PdfTitle = "x";
                P.PdfSubject = "x";
                P.PdfAuthor = "x";
                P.PdfKeywords = "x";
                P.PdfCreator = "x";

                // Discard output. Just tests that all option keys are correct.
                using (MemoryStream ms = new MemoryStream())
                {
                    Assert.True(P.Convert(InputPath, ms), E.Msg);
                }
            }
        }

        public class PrinceControlStandaloneTests
        {
            public class NoLocalEvents : PrinceEvents
            {
                public void OnMessage(MessageType msgType, string msgLocation, string msgText)
                {
                    Assert.Equal(MessageType.WRN, msgType);
                    Assert.Contains("convert-1.css", msgLocation);
                    Assert.Equal("not loading local file", msgText);
                }

                public void OnDataMessage(string name, string value) {}
            }

            [Fact]
            public void ConvertNoLocal()
            {
                PrinceEvents E = new NoLocalEvents();
                PrinceControl P = new PrinceControl(PrincePath, E);
                P.NoLocalFiles = true;

                P.Start();
                using (Stream fs = File.Create(ResourcesDir + "control-nolocal.pdf"))
                {
                    Assert.True(P.Convert(ResourcesDir + "convert-nolocal.html", fs));
                }
                P.Stop();
            }
        }
    }
}
