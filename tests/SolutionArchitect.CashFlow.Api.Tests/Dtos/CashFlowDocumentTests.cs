using System;
using System.Linq;
using System.Reflection;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using SolutionArchitect.CashFlow.Api.Shareable.Dtos;

namespace SolutionArchitect.CashFlow.Api.Tests.Dtos
{
    public class CashFlowDocumentTests
    {
        [Fact]
        public void Initialization_SetsProperties()
        {
            var id = ObjectId.GenerateNewId();
            var date = DateTime.UtcNow;

            var doc = new CashFlowDocument
            {
                Id = id,
                Balance = 123.45m,
                Date = date,
                Version = 2
            };

            Assert.Equal(id, doc.Id);
            Assert.Equal(123.45m, doc.Balance);
            Assert.Equal(date, doc.Date);
            Assert.Equal(2, doc.Version);
        }

        [Fact]
        public void Id_HasBsonAttributes()
        {
            var prop = typeof(CashFlowDocument).GetProperty(nameof(CashFlowDocument.Id));
            Assert.NotNull(prop);

            var bsonIdAttr = prop.GetCustomAttribute<BsonIdAttribute>();
            Assert.NotNull(bsonIdAttr);

            var repAttr = prop.GetCustomAttribute<BsonRepresentationAttribute>();
            Assert.NotNull(repAttr);
            Assert.Equal(BsonType.String, repAttr.Representation);
        }

        [Fact]
        public void Properties_AreInitOnly()
        {
            var type = typeof(CashFlowDocument);
            var propertyNames = new[] { "Id", "Balance", "Date", "Version" };

            foreach (var name in propertyNames)
            {
                var prop = type.GetProperty(name);
                Assert.NotNull(prop);

                var setMethod = prop.SetMethod;
                Assert.NotNull(setMethod);

                Assert.True(setMethod.ReturnParameter.GetRequiredCustomModifiers()
                    .Any(m => m.FullName == "System.Runtime.CompilerServices.IsExternalInit"),
                    $"Property '{name}' does not appear to be init-only.");
            }
        }
    }
}