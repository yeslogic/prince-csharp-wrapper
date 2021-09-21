using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

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

        public override bool Equals(object? obj)
        {
            if (obj is not Enumeration otherEnum)
            {
                return false;
            }
            var typeMatches = GetType().Equals(obj.GetType());
            var idMatches = Id.Equals(otherEnum.Id);
            return typeMatches && idMatches;
        }

        public override int GetHashCode() => Id.GetHashCode();

        public override string ToString() => Name;

        public int CompareTo(object? obj)
        {
            if (obj is not Enumeration otherEnum)
            {
                return 1;
            }
            return Id.CompareTo(otherEnum.Id);
        }
    }
}
