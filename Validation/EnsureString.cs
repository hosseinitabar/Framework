using Holism.Framework;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;

namespace Holism.Validation
{
    public class EnsureString
    {
        string @string;

        public EnsureString(string @string)
        {
            this.@string = @string;
        }

        public EnsureString IsGuid(string message = null)
        {
            if (@string.IsGuid())
            {
                return this;
            }
            throw new ClientException(message ?? "String is not a GUID");
        }

        public EnsureString IsNotEmptyGuid(string message = null)
        {
            IsGuid(message);
            var guid = Guid.Parse(@string);
            if (guid == Guid.Empty)
            {
                throw new ClientException(message ?? "GUID is empty");
            }
            return this;
        }

        public EnsureString HasLengthGreaterThan(int length, string message = null)
        {
            if (@string.Length <= length)
            {
                throw new ClientException(message ?? $"Ensure validation: '{@string}' length is {@string.Length} characters, but it should be more than {length} characters.");
            }
            return this;
        }

        public EnsureString HasLengthEquoalTo(int length, string message = null)
        {
            if (@string.Length != length)
            {
                throw new ClientException(message ?? $"Ensure validation: '{@string}' length is {@string.Length} characters, but it should be {length} characters.");
            }
            return this;
        }

        public EnsureString HasLengthGreaterThanOrEqualTo(int length, string message = null)
        {
            if (@string.Length < length)
            {
                throw new ClientException(message ?? $"Ensure validation: '{@string}' length is {@string.Length} characters, but it should be more than or equal to {length} characters.");
            }
            return this;
        }

        public EnsureString And()
        {
            return this;
        }

        public EnsureString IsSomething(string message = null)
        {
            if (@string.IsNothing())
            {
                throw new ClientException(message ?? "Ensure validation: string is either null, or whitespace.");
            }
            return this;
        }

        public EnsureString IsEmail(string message = null)
        {
            if (EmailValidator.IsEmail(@string))
            {
                return this;
            }
            throw new ClientException(message ?? $"Ensure validation: '{@string}' is not a valid email address.");
        }

        public EnsureString IsEqualTo(string target, string message = null)
        {
            if (@string.ToLower() != target.ToLower())
            {
                throw new ClientException(message ?? $"Ensure validation: '{@string}' is not equal to '{target}'. Comparison is made without considerint case sensitivity.");
            }
            return this;
        }

        public EnsureString IsUrl(string message = null)
        {
            new Ensure(@string).AsString().IsSomething();
            Uri uri;
            if (!Uri.TryCreate(@string, UriKind.Absolute, out uri))
            {
                throw new ClientException(message ?? $"Ensure validation: '{@string}' is not a valid URL");
            }
            return this;
        }

        public EnsureString IsIp(string message = null)
        {
            IPAddress ip;
            if (!IPAddress.TryParse(@string, out ip))
            {
                throw new ClientException(message ?? $"{@string} is not a valid IP address");
            }
            return this;
        }

        public EnsureString IsHexadecimalColor(string message = null)
        {
            @string.Ensure().IsSomething();
            @string = @string.Trim();
            if (!@string.StartsWith("#"))
            {
                throw new ClientException(message ?? $"Hexadecimal colors start with #");
            }
            if (@string.Replace("#", "").Length == 3)
            {
                if (!Regex.IsMatch(@string, @"[0-9a-fA-F]{3}"))
                {
                    throw new ClientException(message ?? $"{@string} is not a valid 3-character hexadecimal color.");
                }
            }
            else if (@string.Replace("#", "").Length == 6)
            {
                if (!Regex.IsMatch(@string, @"[0-9a-fA-F]{6}"))
                {
                    throw new ClientException(message ?? $"{@string} is not a valid 6-character hexadecimal color.");
                }
            }
            else
            {
                throw new ClientException(message ?? $"{ @string} has invalid length for a hexadecimal color.");
            }
            return this;
        }

        public EnsureString IsJson(string message = null)
        {
            if (@string.IsJson())
            {
                return this;
            }
            throw new ClientException(message ?? "String is not a valid JSON");
        }

        public EnsureString EqualsTo(string target, string message = null)
        {
            if (@string == target)
            {
                return this;
            }
            throw new ClientException(message ?? "Values are not equal");
        }
    }
}