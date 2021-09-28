using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace PrinceXML.Wrapper.Enums
{
    public class AuthMethod : Enumeration
    {
        public static AuthMethod Basic = new AuthMethod(0, "basic");
        public static AuthMethod Digest = new AuthMethod(1, "digest");
        public static AuthMethod Ntlm = new AuthMethod(2, "ntlm");
        public static AuthMethod Negotiate = new AuthMethod(3, "negotiate");

        private AuthMethod(int id, string name) : base(id, name) {}
    }

    public class AuthScheme : Enumeration
    {
        public static AuthScheme Http = new AuthScheme(0, "http");
        public static AuthScheme Https = new AuthScheme(1, "https");

        private AuthScheme(int id, string name) : base(id, name) {}
    }

    public class InputType : Enumeration
    {
        public static InputType Auto = new InputType(0, "auto");
        public static InputType Html = new InputType(1, "html");
        public static InputType Xml = new InputType(2, "xml");

        private InputType(int id, string name) : base(id, name) {}
    }

    public enum KeyBits
    {
        Bits40 = 40,
        Bits128 = 128
    }

    public class PdfProfile : Enumeration
    {
        public static PdfProfile PdfA_1A = new PdfProfile(0, "PDF/A-1a");
        public static PdfProfile PdfA_1A_And_PdfUA_1 = new PdfProfile(1, "PDF/A-1a+PDF/UA-1");
        public static PdfProfile PdfA_1B = new PdfProfile(2, "PDF/A-1b");
        public static PdfProfile PdfA_2A = new PdfProfile(3, "PDF/A-2a");
        public static PdfProfile PdfA_2A_And_PdfUA_1 = new PdfProfile(4, "PDF/A-2a+PDF/UA-1");
        public static PdfProfile PdfA_2B = new PdfProfile(5, "PDF/A-2b");
        public static PdfProfile PdfA_3A = new PdfProfile(6, "PDF/A-3a");
        public static PdfProfile PdfA_3A_And_PdfUA_1 = new PdfProfile(7, "PDF/A-3a+PDF/UA-1");
        public static PdfProfile PdfA_3B = new PdfProfile(8, "PDF/A-3b");
        public static PdfProfile PdfUA_1 = new PdfProfile(9, "PDF/UA-1");
        public static PdfProfile PdfX_1A_2001 = new PdfProfile(10, "PDF/X-1a:2001");
        public static PdfProfile PdfX_1A_2003 = new PdfProfile(11, "PDF/X-1a:2003");
        public static PdfProfile PdfX_3_2002 = new PdfProfile(12, "PDF/X-3:2002");
        public static PdfProfile PdfX_3_2003 = new PdfProfile(13, "PDF/X-3:2003");
        public static PdfProfile PdfX_4 = new PdfProfile(14, "PDF/X-4");

        private PdfProfile(int id, string name) : base(id, name) {}
    }

    public class RasterBackground : Enumeration
    {
        public static RasterBackground White = new RasterBackground(0, "white");
        public static RasterBackground Transparent = new RasterBackground(1, "transparent");

        private RasterBackground(int id, string name) : base(id, name) {}
    }

    public class RasterFormat : Enumeration
    {
        public static RasterFormat Auto = new RasterFormat(0, "auto");
        public static RasterFormat Png = new RasterFormat(1, "png");
        public static RasterFormat Jpeg = new RasterFormat(2, "jpeg");

        private RasterFormat(int id, string name) : base(id, name) {}
    }

    public class SslType : Enumeration
    {
        public static SslType Pem = new SslType(0, "PEM");
        public static SslType Der = new SslType(1, "DER");

        private SslType(int id, string name) : base(id, name) {}
    }

    public class SslVersion : Enumeration
    {
        public static SslVersion Default = new SslVersion(0, "default");
        public static SslVersion TlsV1 = new SslVersion(1, "tlsv1");
        public static SslVersion TlsV1_0 = new SslVersion(2, "tlsv1.0");
        public static SslVersion TlsV1_1 = new SslVersion(3, "tlsv1.1");
        public static SslVersion TlsV1_2 = new SslVersion(4, "tlsv1.2");
        public static SslVersion TlsV1_3 = new SslVersion(5, "tlsv1.3");

        private SslVersion(int id, string name) : base(id, name) {}
    }

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
}
