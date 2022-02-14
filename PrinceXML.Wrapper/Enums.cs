using System;

namespace PrinceXML.Wrapper.Enums
{
    /// <summary>HTTP authentication methods.</summary>
    public class AuthMethod
    {
        /// <summary>Equates to the string <c>"basic"</c>.</summary>
        public static readonly AuthMethod Basic = new AuthMethod("basic");
        /// <summary>Equates to the string <c>"digest"</c>.</summary>
        public static readonly AuthMethod Digest = new AuthMethod("digest");
        /// <summary>Equates to the string <c>"ntlm"</c>.</summary>
        public static readonly AuthMethod Ntlm = new AuthMethod("ntlm");
        /// <summary>Equates to the string <c>"negotiate"</c>.</summary>
        public static readonly AuthMethod Negotiate = new AuthMethod("negotiate");

        private readonly string _name;
        private AuthMethod(string name) => _name = name;
        /// <summary>Returns a string representation of the object.</summary>
        public override string ToString() => _name;
    }

    /// <summary>HTTP authentication schemes.</summary>
    public class AuthScheme
    {
        /// <summary>Equates to the string <c>"http"</c>.</summary>
        public static readonly AuthScheme Http = new AuthScheme("http");
        /// <summary>Equates to the string <c>"https"</c>.</summary>
        public static readonly AuthScheme Https = new AuthScheme("https");

        private readonly string _name;
        private AuthScheme(string name) => _name = name;
        /// <summary>Returns a string representation of the object.</summary>
        public override string ToString() => _name;
    }

    /// <summary>Input types.</summary>
    public class InputType
    {
        /// <summary>Equates to the string <c>"auto"</c>.</summary>
        public static readonly InputType Auto = new InputType("auto");
        /// <summary>Equates to the string <c>"html"</c>.</summary>
        public static readonly InputType Html = new InputType("html");
        /// <summary>Equates to the string <c>"xml"</c>.</summary>
        public static readonly InputType Xml = new InputType("xml");

        private readonly string _name;
        private InputType(string name) => _name = name;
        /// <summary>Returns a string representation of the object.</summary>
        public override string ToString() => _name;
    }

    /// <summary>Encryption key sizes.</summary>
    public enum KeyBits
    {
        /// <summary>Equates to the integer <c>40</c>.</summary>
        Bits40 = 40,
        /// <summary>Equates to the integer <c>128</c>.</summary>
        Bits128 = 128
    }

    /// <summary>PDF events.</summary>
    public class PdfEvent
    {
        /// <summary>Equates to the string <c>"will-close"</c>.</summary>
        public static readonly PdfEvent WillClose = new PdfEvent("will-close");
        /// <summary>Equates to the string <c>"will-save"</c>.</summary>
        public static readonly PdfEvent WillSave = new PdfEvent("will-save");
        /// <summary>Equates to the string <c>"did-save"</c>.</summary>
        public static readonly PdfEvent DidSave = new PdfEvent("did-save");
        /// <summary>Equates to the string <c>"will-print"</c>.</summary>
        public static readonly PdfEvent WillPrint = new PdfEvent("will-print");
        /// <summary>Equates to the string <c>"did-print"</c>.</summary>
        public static readonly PdfEvent DidPrint = new PdfEvent("did-print");

        private readonly string _name;
        private PdfEvent(string name) => _name = name;
        /// <summary>Returns a string representation of the object.</summary>
        public override string ToString() => _name;
    }

    /// <summary>PDF profiles.</summary>
    public class PdfProfile
    {
        /// <summary>Equates to the string <c>"PDF/A-1a"</c>.</summary>
        public static readonly PdfProfile PdfA_1A = new PdfProfile("PDF/A-1a");
        /// <summary>Equates to the string <c>"PDF/A-1a+PDF/UA-1"</c>.</summary>
        public static readonly PdfProfile PdfA_1A_And_PdfUA_1 = new PdfProfile("PDF/A-1a+PDF/UA-1");
        /// <summary>Equates to the string <c>"PDF/A-1b"</c>.</summary>
        public static readonly PdfProfile PdfA_1B = new PdfProfile("PDF/A-1b");
        /// <summary>Equates to the string <c>"PDF/A-2a"</c>.</summary>
        public static readonly PdfProfile PdfA_2A = new PdfProfile("PDF/A-2a");
        /// <summary>Equates to the string <c>"PDF/A-2a+PDF/UA-1"</c>.</summary>
        public static readonly PdfProfile PdfA_2A_And_PdfUA_1 = new PdfProfile("PDF/A-2a+PDF/UA-1");
        /// <summary>Equates to the string <c>"PDF/A-2b"</c>.</summary>
        public static readonly PdfProfile PdfA_2B = new PdfProfile("PDF/A-2b");
        /// <summary>Equates to the string <c>"PDF/A-3a"</c>.</summary>
        public static readonly PdfProfile PdfA_3A = new PdfProfile("PDF/A-3a");
        /// <summary>Equates to the string <c>"PDF/A-3a+PDF/UA-1"</c>.</summary>
        public static readonly PdfProfile PdfA_3A_And_PdfUA_1 = new PdfProfile("PDF/A-3a+PDF/UA-1");
        /// <summary>Equates to the string <c>"PDF/A-3b"</c>.</summary>
        public static readonly PdfProfile PdfA_3B = new PdfProfile("PDF/A-3b");
        /// <summary>Equates to the string <c>"PDF/UA-1"</c>.</summary>
        public static readonly PdfProfile PdfUA_1 = new PdfProfile("PDF/UA-1");
        /// <summary>Equates to the string <c>"PDF/X-1a:2001"</c>.</summary>
        public static readonly PdfProfile PdfX_1A_2001 = new PdfProfile("PDF/X-1a:2001");
        /// <summary>Equates to the string <c>"PDF/X-1a:2003"</c>.</summary>
        public static readonly PdfProfile PdfX_1A_2003 = new PdfProfile("PDF/X-1a:2003");
        /// <summary>Equates to the string <c>"PDF/X-3:2002"</c>.</summary>
        public static readonly PdfProfile PdfX_3_2002 = new PdfProfile("PDF/X-3:2002");
        /// <summary>Equates to the string <c>"PDF/X-3:2003"</c>.</summary>
        public static readonly PdfProfile PdfX_3_2003 = new PdfProfile("PDF/X-3:2003");
        /// <summary>Equates to the string <c>"PDF/X-4"</c>.</summary>
        public static readonly PdfProfile PdfX_4 = new PdfProfile("PDF/X-4");

        private readonly string _name;
        private PdfProfile(string name) => _name = name;
        /// <summary>Returns a string representation of the object.</summary>
        public override string ToString() => _name;
    }

    /// <summary>Raster backgrounds.</summary>
    public class RasterBackground
    {
        /// <summary>Equates to the string <c>"white"</c>.</summary>
        public static readonly RasterBackground White = new RasterBackground("white");
        /// <summary>Equates to the string <c>"transparent"</c>.</summary>
        public static readonly RasterBackground Transparent = new RasterBackground("transparent");

        private readonly string _name;
        private RasterBackground(string name) => _name = name;
        /// <summary>Returns a string representation of the object.</summary>
        public override string ToString() => _name;
    }

    /// <summary>Raster formats.</summary>
    public class RasterFormat
    {
        /// <summary>Equates to the string <c>"auto"</c>.</summary>
        public static readonly RasterFormat Auto = new RasterFormat("auto");
        /// <summary>Equates to the string <c>"png"</c>.</summary>
        public static readonly RasterFormat Png = new RasterFormat("png");
        /// <summary>Equates to the string <c>"jpeg"</c>.</summary>
        public static readonly RasterFormat Jpeg = new RasterFormat("jpeg");

        private readonly string _name;
        private RasterFormat(string name) => _name = name;
        /// <summary>Returns a string representation of the object.</summary>
        public override string ToString() => _name;
    }

    /// <summary>SSL file type.</summary>
    public class SslType
    {
        /// <summary>Equates to the string <c>"PEM"</c>.</summary>
        public static readonly SslType Pem = new SslType("PEM");
        /// <summary>Equates to the string <c>"DER"</c>.</summary>
        public static readonly SslType Der = new SslType("DER");

        private readonly string _name;
        private SslType(string name) => _name = name;
        /// <summary>Returns a string representation of the object.</summary>
        public override string ToString() => _name;
    }

    /// <summary>Minimum-allowed SSL version.</summary>
    public class SslVersion
    {
        /// <summary>Equates to the string <c>"default"</c>.</summary>
        public static readonly SslVersion Default = new SslVersion("default");
        /// <summary>Equates to the string <c>"tlsv1"</c>.</summary>
        public static readonly SslVersion TlsV1 = new SslVersion("tlsv1");
        /// <summary>Equates to the string <c>"tlsv1.0"</c>.</summary>
        public static readonly SslVersion TlsV1_0 = new SslVersion("tlsv1.0");
        /// <summary>Equates to the string <c>"tlsv1.1"</c>.</summary>
        public static readonly SslVersion TlsV1_1 = new SslVersion("tlsv1.1");
        /// <summary>Equates to the string <c>"tlsv1.2"</c>.</summary>
        public static readonly SslVersion TlsV1_2 = new SslVersion("tlsv1.2");
        /// <summary>Equates to the string <c>"tlsv1.3"</c>.</summary>
        public static readonly SslVersion TlsV1_3 = new SslVersion("tlsv1.3");

        private readonly string _name;
        private SslVersion(string name) => _name = name;
        /// <summary>Returns a string representation of the object.</summary>
        public override string ToString() => _name;
    }
}
