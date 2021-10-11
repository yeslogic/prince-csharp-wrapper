using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace PrinceXML.Wrapper.Enums
{
    /// <summary>HTTP authentication methods.</summary>
    public class AuthMethod : Enumeration
    {
        /// <summary>Equates to the string <c>"basic"</c>.</summary>
        public static AuthMethod Basic = new AuthMethod(0, "basic");
        /// <summary>Equates to the string <c>"digest"</c>.</summary>
        public static AuthMethod Digest = new AuthMethod(1, "digest");
        /// <summary>Equates to the string <c>"ntlm"</c>.</summary>
        public static AuthMethod Ntlm = new AuthMethod(2, "ntlm");
        /// <summary>Equates to the string <c>"negotiate"</c>.</summary>
        public static AuthMethod Negotiate = new AuthMethod(3, "negotiate");

        private AuthMethod(int id, string name) : base(id, name) {}
    }

    /// <summary>HTTP authentication schemes.</summary>
    public class AuthScheme : Enumeration
    {
        /// <summary>Equates to the string <c>"http"</c>.</summary>
        public static AuthScheme Http = new AuthScheme(0, "http");
        /// <summary>Equates to the string <c>"https"</c>.</summary>
        public static AuthScheme Https = new AuthScheme(1, "https");

        private AuthScheme(int id, string name) : base(id, name) {}
    }

    /// <summary>Input types.</summary>
    public class InputType : Enumeration
    {
        /// <summary>Equates to the string <c>"auto"</c>.</summary>
        public static InputType Auto = new InputType(0, "auto");
        /// <summary>Equates to the string <c>"html"</c>.</summary>
        public static InputType Html = new InputType(1, "html");
        /// <summary>Equates to the string <c>"xml"</c>.</summary>
        public static InputType Xml = new InputType(2, "xml");

        private InputType(int id, string name) : base(id, name) {}
    }

    /// <summary>Encryption key sizes.</summary>
    public enum KeyBits
    {
        /// <summary>Equates to the integer <c>40</c>.</summary>
        Bits40 = 40,
        /// <summary>Equates to the integer <c>128</c>.</summary>
        Bits128 = 128
    }

    /// <summary>PDF profiles.</summary>
    public class PdfProfile : Enumeration
    {
        /// <summary>Equates to the string <c>"PDF/A-1a"</c>.</summary>
        public static PdfProfile PdfA_1A = new PdfProfile(0, "PDF/A-1a");
        /// <summary>Equates to the string <c>"PDF/A-1a+PDF/UA-1"</c>.</summary>
        public static PdfProfile PdfA_1A_And_PdfUA_1 = new PdfProfile(1, "PDF/A-1a+PDF/UA-1");
        /// <summary>Equates to the string <c>"PDF/A-1b"</c>.</summary>
        public static PdfProfile PdfA_1B = new PdfProfile(2, "PDF/A-1b");
        /// <summary>Equates to the string <c>"PDF/A-2a"</c>.</summary>
        public static PdfProfile PdfA_2A = new PdfProfile(3, "PDF/A-2a");
        /// <summary>Equates to the string <c>"PDF/A-2a+PDF/UA-1"</c>.</summary>
        public static PdfProfile PdfA_2A_And_PdfUA_1 = new PdfProfile(4, "PDF/A-2a+PDF/UA-1");
        /// <summary>Equates to the string <c>"PDF/A-2b"</c>.</summary>
        public static PdfProfile PdfA_2B = new PdfProfile(5, "PDF/A-2b");
        /// <summary>Equates to the string <c>"PDF/A-3a"</c>.</summary>
        public static PdfProfile PdfA_3A = new PdfProfile(6, "PDF/A-3a");
        /// <summary>Equates to the string <c>"PDF/A-3a+PDF/UA-1"</c>.</summary>
        public static PdfProfile PdfA_3A_And_PdfUA_1 = new PdfProfile(7, "PDF/A-3a+PDF/UA-1");
        /// <summary>Equates to the string <c>"PDF/A-3b"</c>.</summary>
        public static PdfProfile PdfA_3B = new PdfProfile(8, "PDF/A-3b");
        /// <summary>Equates to the string <c>"PDF/UA-1"</c>.</summary>
        public static PdfProfile PdfUA_1 = new PdfProfile(9, "PDF/UA-1");
        /// <summary>Equates to the string <c>"PDF/X-1a:2001"</c>.</summary>
        public static PdfProfile PdfX_1A_2001 = new PdfProfile(10, "PDF/X-1a:2001");
        /// <summary>Equates to the string <c>"PDF/X-1a:2003"</c>.</summary>
        public static PdfProfile PdfX_1A_2003 = new PdfProfile(11, "PDF/X-1a:2003");
        /// <summary>Equates to the string <c>"PDF/X-3:2002"</c>.</summary>
        public static PdfProfile PdfX_3_2002 = new PdfProfile(12, "PDF/X-3:2002");
        /// <summary>Equates to the string <c>"PDF/X-3:2003"</c>.</summary>
        public static PdfProfile PdfX_3_2003 = new PdfProfile(13, "PDF/X-3:2003");
        /// <summary>Equates to the string <c>"PDF/X-4"</c>.</summary>
        public static PdfProfile PdfX_4 = new PdfProfile(14, "PDF/X-4");

        private PdfProfile(int id, string name) : base(id, name) {}
    }

    /// <summary>Raster backgrounds.</summary>
    public class RasterBackground : Enumeration
    {
        /// <summary>Equates to the string <c>"white"</c>.</summary>
        public static RasterBackground White = new RasterBackground(0, "white");
        /// <summary>Equates to the string <c>"transparent"</c>.</summary>
        public static RasterBackground Transparent = new RasterBackground(1, "transparent");

        private RasterBackground(int id, string name) : base(id, name) {}
    }

    /// <summary>Raster formats.</summary>
    public class RasterFormat : Enumeration
    {
        /// <summary>Equates to the string <c>"auto"</c>.</summary>
        public static RasterFormat Auto = new RasterFormat(0, "auto");
        /// <summary>Equates to the string <c>"png"</c>.</summary>
        public static RasterFormat Png = new RasterFormat(1, "png");
        /// <summary>Equates to the string <c>"jpeg"</c>.</summary>
        public static RasterFormat Jpeg = new RasterFormat(2, "jpeg");

        private RasterFormat(int id, string name) : base(id, name) {}
    }

    /// <summary>SSL file type.</summary>
    public class SslType : Enumeration
    {
        /// <summary>Equates to the string <c>"PEM"</c>.</summary>
        public static SslType Pem = new SslType(0, "PEM");
        /// <summary>Equates to the string <c>"DER"</c>.</summary>
        public static SslType Der = new SslType(1, "DER");

        private SslType(int id, string name) : base(id, name) {}
    }

    /// <summary>Minimum-allowed SSL version.</summary>
    public class SslVersion : Enumeration
    {
        /// <summary>Equates to the string <c>"default"</c>.</summary>
        public static SslVersion Default = new SslVersion(0, "default");
        /// <summary>Equates to the string <c>"tlsv1"</c>.</summary>
        public static SslVersion TlsV1 = new SslVersion(1, "tlsv1");
        /// <summary>Equates to the string <c>"tlsv1.0"</c>.</summary>
        public static SslVersion TlsV1_0 = new SslVersion(2, "tlsv1.0");
        /// <summary>Equates to the string <c>"tlsv1.1"</c>.</summary>
        public static SslVersion TlsV1_1 = new SslVersion(3, "tlsv1.1");
        /// <summary>Equates to the string <c>"tlsv1.2"</c>.</summary>
        public static SslVersion TlsV1_2 = new SslVersion(4, "tlsv1.2");
        /// <summary>Equates to the string <c>"tlsv1.3"</c>.</summary>
        public static SslVersion TlsV1_3 = new SslVersion(5, "tlsv1.3");

        private SslVersion(int id, string name) : base(id, name) {}
    }

#pragma warning disable 1591
    // https://docs.microsoft.com/en-us/dotnet/architecture/microservices/microservice-ddd-cqrs-patterns/enumeration-classes-over-enum-types
    public abstract class Enumeration : IComparable
    {
        public int Id { get; private set; }
        public string Name { get; private set; }

        protected Enumeration(int id, string name) => (Id, Name) = (id, name);

        public static IEnumerable<T> GetAll<T>() where T : Enumeration =>
            typeof(T)
                .GetFields(BindingFlags.Public |
                           BindingFlags.Static |
                           BindingFlags.DeclaredOnly)
                .Select(f => f.GetValue(null))
                .Cast<T>();

        public override bool Equals(object obj)
        {
            Enumeration otherEnum = obj as Enumeration;
            if (otherEnum == null)
            {
                return false;
            }
            bool typeMatches = GetType().Equals(obj.GetType());
            bool idMatches = Id.Equals(otherEnum.Id);
            return typeMatches && idMatches;
        }

        public override int GetHashCode() => Id.GetHashCode();

        public override string ToString() => Name;

        public int CompareTo(object obj)
        {
            Enumeration otherEnum = obj as Enumeration;
            if (otherEnum == null)
            {
                return 1;
            }
            return Id.CompareTo(otherEnum.Id);
        }
    }
#pragma warning restore 1591
}
