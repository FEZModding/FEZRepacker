using System.Reflection;

using FEZRepacker.Core.Definitions.Game.Level;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using Mono.Cecil;

namespace FEZRepacker.Tests
{
    // Verifies XNB reference properties match the nullable contract of FEZ metadata
    [TestClass]
    public class TypeContractMetadataTests
    {
        private const string FezAssemblyName = "FezEngine";

        private const string XnbPropertyAttributeName = "FEZRepacker.Core.Definitions.Game.XnbPropertyAttribute";

        private const string XnbTypeAttributeName = "FEZRepacker.Core.Definitions.Game.XnbTypeAttribute";

        private const string FixturePath = "Dependencies/FezEngine.dll";

        // Reads stripped FEZ metadata and requires every XNB reference property to be nullable
        [TestMethod]
        public void FezReferencePropertiesAreNullable()
        {
            var fixturePath = Path.Combine(AppContext.BaseDirectory, FixturePath);
            var failures = new List<string>();

            using var module = ModuleDefinition.ReadModule(fixturePath);

            foreach (var repackerType in typeof(Level).Assembly.GetTypes())
            {
                var originalTypeName = GetFezTypeName(repackerType);
                if (originalTypeName == null)
                {
                    continue;
                }

                var originalType = module.GetType(originalTypeName) ?? module.GetTypes()
                    .FirstOrDefault(type => type.FullName.StartsWith($"{originalTypeName}{'`'}"));
                if (originalType == null)
                {
                    failures.Add($"FEZ type {originalTypeName} does not exist.");
                    continue;
                }

                foreach (var repackerProperty in
                         repackerType.GetProperties(BindingFlags.Public | BindingFlags.Instance))
                {
                    if (repackerProperty.CustomAttributes.All(attribute => attribute.AttributeType.FullName != XnbPropertyAttributeName))
                    {
                        continue;
                    }

                    var originalPropertyType = GetOriginalPropertyType(originalType, repackerProperty.Name);
                    if (originalPropertyType == null)
                    {
                        // Some repacker properties flatten or rename the original FEZ model.
                        continue;
                    }

                    if (originalPropertyType.IsValueType)
                    {
                        continue;
                    }

                    if (repackerProperty.PropertyType.IsValueType)
                    {
                        failures.Add($"{repackerType.Name}.{repackerProperty.Name} must remain a reference type.");
                        continue;
                    }

                    var nullability = new NullabilityInfoContext().Create(repackerProperty);
                    if (nullability.WriteState != NullabilityState.Nullable)
                    {
                        failures.Add(
                            $"{originalTypeName}.{repackerProperty.Name} | {repackerType.Name}.{repackerProperty.Name}");
                    }
                }
            }

            if (failures.Any())
            {
                var message = string.Join(Environment.NewLine, failures);
                Assert.Fail("Missing FEZ nullable reference properties:\n" + message);
            }
        }

        private static string? GetFezTypeName(Type repackerType)
        {
            var xnbType = repackerType.CustomAttributes.SingleOrDefault(attribute =>
                attribute.AttributeType.FullName == XnbTypeAttributeName);

            return xnbType?.ConstructorArguments.Single().Value is not string qualifier ||
                   !qualifier.Contains($"{','} {FezAssemblyName}{','}")
                ? null
                : qualifier.Split(',')[0].Replace('+', '/');
        }

        private static TypeReference? GetOriginalPropertyType(TypeDefinition originalType, string propertyName)
        {
            var property = originalType.Properties.SingleOrDefault(property => property.Name == propertyName);
            return property != null
                ? property.PropertyType
                : originalType.Fields.SingleOrDefault(field => field.Name == propertyName)?.FieldType;
        }
    }
}