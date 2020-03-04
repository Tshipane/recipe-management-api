using System;
using System.Linq;
using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace RecipeManagement.Infrastructure.Database.Configurations
{
    public class EncryptedConverter : ValueConverter<string, string>
    {
        public EncryptedConverter(ConverterMappingHints mappingHints = default)
            : base(EncryptExpr, DecryptExpr, mappingHints)
        {
        }

        private static readonly Expression<Func<string, string>> DecryptExpr = x => new string(x.Reverse().ToArray());
        private static readonly Expression<Func<string, string>> EncryptExpr = x => new string(x.Reverse().ToArray());
    }
}