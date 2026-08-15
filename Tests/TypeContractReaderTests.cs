using System.Reflection;

using FEZRepacker.Core.Definitions.Game.Level;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using Mono.Cecil;
using Mono.Cecil.Cil;

namespace FEZRepacker.Tests
{
    // Verifies model nullability against FEZ defaults first, then preserved FEZ reader IL
    [TestClass]
    public class TypeContractReaderTests
    {
        private const string FixturePath = "Dependencies/FezEngine.dll";

        private const string ReadObjectMethodName = "ReadObject";

        private const string ReadStringMethodName = "ReadString";

        private const string FezReaderNamespacePrefix = "FezEngine.Readers.";

        private const string ReaderMethodName = "Read";

        private const string XnbPropertyAttributeName = "FEZRepacker.Core.Definitions.Game.XnbPropertyAttribute";

        private const string XnbReaderTypeAttributeName = "FEZRepacker.Core.Definitions.Game.XnbReaderTypeAttribute";

        private enum ReaderValueContract
        {
            Required,
            NullableObject
        }

        // These readers assign through a closure or under a name FezEngine does not share with the model
        private static readonly IReadOnlyDictionary<string, ReaderValueContract> ReaderOverrides =
            new Dictionary<string, ReaderValueContract>(StringComparer.Ordinal)
            {
                ["FezEngine.Readers.TrileSetReader.TextureAtlas"] = ReaderValueContract.NullableObject,
                ["FezEngine.Readers.ArtObjectReader.Cubemap"] = ReaderValueContract.NullableObject,
                ["FezEngine.Readers.LevelReader.StartingFace"] = ReaderValueContract.NullableObject
            };

        // Maps every reader-backed reference property and checks its nullable write metadata
        [TestMethod]
        public void ModelDefaultsAndReaderAssignmentsMatchModelNullability()
        {
            var fixturePath = Path.Combine(AppContext.BaseDirectory, FixturePath);
            var failures = new List<string>();

            using var module = ModuleDefinition.ReadModule(fixturePath);

            foreach (var repackerType in typeof(Level).Assembly.GetTypes())
            {
                var readerName = GetReaderName(repackerType);
                if (readerName is null)
                {
                    continue;
                }

                if (!readerName.StartsWith(FezReaderNamespacePrefix, StringComparison.Ordinal))
                {
                    continue;
                }

                var reader = GetReader(module, readerName, repackerType);
                if (reader is null)
                {
                    failures.Add($"Reader {readerName} does not exist.");
                    continue;
                }

                var readMethod = reader.Methods.SingleOrDefault(method => method.Name == ReaderMethodName && method.HasBody);
                if (readMethod is null)
                {
                    failures.Add($"Reader {readerName} does not have a readable Read method.");
                    continue;
                }

                foreach (var property in GetReferenceXnbProperties(repackerType))
                {
                    var key = $"{reader.FullName}.{property.Name}";

                    // We say a default value of a property means it's highly unlikely to be null,
                    // so assume it's non-nullable.
                    if (HasNonNullDefault(repackerType, property))
                    {
                        AssertNullability(property, ReaderValueContract.Required, key, failures);
                        continue;
                    }

                    var expected = GetDirectContract(readMethod, property.Name);

                    if (expected is not null)
                    {
                        AssertNullability(property, expected.Value, key, failures);
                        continue;
                    }

                    if (!ReaderOverrides.TryGetValue(key, out var overrideContract))
                    {
                        failures.Add($"{key} has no direct reader contract or explicit override.");
                        continue;
                    }

                    AssertNullability(property, overrideContract, key, failures);
                }
            }

            if (failures.Any())
            {
                Assert.Fail("Reader-derived nullability contract failures:\n" + string.Join(Environment.NewLine, failures));
            }
        }

        // Keeps every model property on one side of the rule: a default means non-nullable, and nothing else does
        [TestMethod]
        public void NullabilityMatchesDefaultPresence()
        {
            var failures = new List<string>();

            foreach (var repackerType in typeof(Level).Assembly.GetTypes())
            {
                foreach (var property in GetReferenceXnbProperties(repackerType))
                {
                    if (CreateDefaultInstance(repackerType) is null)
                    {
                        continue;
                    }

                    var hasDefault = HasNonNullDefault(repackerType, property);
                    var isNullable = new NullabilityInfoContext().Create(property).WriteState == NullabilityState.Nullable;

                    if (hasDefault == isNullable)
                    {
                        failures.Add(hasDefault
                            ? $"{repackerType.Name}.{property.Name} has a default but is declared nullable."
                            : $"{repackerType.Name}.{property.Name} is declared non-nullable but has no default.");
                    }
                }
            }

            if (failures.Any())
            {
                Assert.Fail("Default and nullability mismatches:\n" + string.Join(Environment.NewLine, failures));
            }
        }

        private static void AssertNullability(
            PropertyInfo property,
            ReaderValueContract contract,
            string key,
            ICollection<string> failures)
        {
            var actual = new NullabilityInfoContext().Create(property).WriteState;
            var expectedNullability = contract == ReaderValueContract.NullableObject
                        ? NullabilityState.Nullable
                        : NullabilityState.NotNull;

            if (actual != expectedNullability)
            {
                failures.Add($"{key} must be {expectedNullability} but is {actual}.");
            }
        }

        private static bool HasNonNullDefault(Type declaringType, PropertyInfo property)
        {
            var instance = CreateDefaultInstance(declaringType);

            return instance is not null &&
                instance.GetType().GetProperty(property.Name)?.GetValue(instance) is not null;
        }

        private static object? CreateDefaultInstance(Type type)
        {
            if (type.IsGenericTypeDefinition)
            {
                var placeholders = type.GetGenericArguments().Select(_ => typeof(object)).ToArray();
                type = type.MakeGenericType(placeholders);
            }

            return type.GetConstructor(Type.EmptyTypes) is null ? null : Activator.CreateInstance(type);
        }

        private static IEnumerable<PropertyInfo> GetReferenceXnbProperties(Type type)
        {
            return type.GetProperties(BindingFlags.Public | BindingFlags.Instance)
                .Where(property => !property.PropertyType.IsValueType)
                .Where(property => property.CustomAttributes.Any(attribute =>
                    attribute.AttributeType.FullName == XnbPropertyAttributeName));
        }

        private static string? GetReaderName(Type repackerType)
        {
            var readerAttribute = repackerType.CustomAttributes.SingleOrDefault(attribute =>
                attribute.AttributeType.FullName == XnbReaderTypeAttributeName);

            return readerAttribute?.ConstructorArguments.Single().Value is string qualifier
                ? qualifier.Split(',')[0].Replace('+', '/')
                : null;
        }

        private static TypeDefinition? GetReader(ModuleDefinition module, string readerName, Type repackerType)
        {
            var reader = module.GetType(readerName);
            if (reader is not null || !repackerType.IsGenericType)
            {
                return reader;
            }

            return module.GetType($"{readerName}{'`'}{repackerType.GetGenericArguments().Length}");
        }

        private static ReaderValueContract? GetDirectContract(MethodDefinition readMethod, string propertyName)
        {
            var instructions = readMethod.Body.Instructions;

            for (var index = 1; index < instructions.Count; index++)
            {
                if (!IsPropertyAssignment(instructions[index], propertyName))
                {
                    continue;
                }

                return GetReadContract(instructions[index - 1]);
            }

            return null;
        }

        private static bool IsPropertyAssignment(Instruction instruction, string propertyName)
        {
            return instruction.Operand switch
            {
                MethodReference { Name: var name } when (instruction.OpCode.Code is Code.Call or Code.Callvirt) &&
                    name == $"set_{propertyName}" => true,
                FieldReference { Name: var name } when instruction.OpCode.Code == Code.Stfld &&
                    name == propertyName => true,
                _ => false
            };
        }

        private static ReaderValueContract? GetReadContract(Instruction instruction)
        {
            if (instruction.Operand is not MethodReference method)
            {
                return null;
            }

            if (method.Name == ReadStringMethodName)
            {
                return ReaderValueContract.Required;
            }

            if (method.Name != ReadObjectMethodName || method is not GenericInstanceMethod genericMethod)
            {
                return null;
            }

            return genericMethod.GenericArguments[0].IsValueType
                ? null
                : ReaderValueContract.NullableObject;
        }
    }
}
