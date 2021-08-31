namespace PrinceXML.Wrapper.Enums
{
    public static class AuthMethod
    {
        public const string Basic = "basic";
        public const string Digest = "digest";
        public const string Ntlm = "ntlm";
        public const string Negotiate = "negotiate";
    }

    public static class AuthScheme
    {
        public const string Http = "http";
        public const string Https = "https";
    }

    public static class InputType
    {
        public const string Auto = "auto";
        public const string Html = "html";
        public const string Xml = "xml";
    }

    public enum KeyBits
    {
        Bits40 = 40,
        Bits128 = 128
    }

    public static class PdfProfile
    {
        public const string PdfA_1A = "PDF/A-1a";
        public const string PdfA_1A_And_PdfUA_1 = "PDF/A-1a+PDF/UA-1";
        public const string PdfA_1B = "PDF/A-1b";
        public const string PdfA_2A = "PDF/A-2a";
        public const string PdfA_2A_And_PdfUA_1 = "PDF/A-2a+PDF/UA-1";
        public const string PdfA_2B = "PDF/A-2b";
        public const string PdfA_3A = "PDF/A-3a";
        public const string PdfA_3A_And_PdfUA_1 = "PDF/A-3a+PDF/UA-1";
        public const string PdfA_3B = "PDF/A-3b";
        public const string PdfUA_1 = "PDF/UA-1";
        public const string PdfX_1A_2001 = "PDF/X-1a:2001";
        public const string PdfX_1A_2003 = "PDF/X-1a:2003";
        public const string PdfX_3_2002 = "PDF/X-3:2002";
        public const string PdfX_3_2003 = "PDF/X-3:2003";
        public const string PdfX_4 = "PDF/X-4";
    }

    public static class RasterBackground
    {
        public const string White = "white";
        public const string Transparent = "transparent";
    }

    public static class RasterFormat
    {
        public const string Auto = "auto";
        public const string Png = "png";
        public const string Jpeg = "jpeg";
    }

    public static class SslType
    {
        public const string Pem = "PEM";
        public const string Der = "DER";
    }

    public static class SslVersion
    {
        public const string Default = "default";
        public const string TlsV1 = "tlsv1";
        public const string TlsV1_0 = "tlsv1.0";
        public const string TlsV1_1 = "tlsv1.1";
        public const string TlsV1_2 = "tlsv1.2";
        public const string TlsV1_3 = "tlsv1.3";
    }
}
