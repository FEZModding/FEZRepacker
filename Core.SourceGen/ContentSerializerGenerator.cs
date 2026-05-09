using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace FEZRepacker.Core.SourceGen;

[Generator]
public sealed class ContentSerializerGenerator : IIncrementalGenerator
{
    private const string XnbReaderTypeAttributeName = "FEZRepacker.Core.Definitions.Game.XnbReaderTypeAttribute";
    private const string XnbPropertyAttributeName = "FEZRepacker.Core.Definitions.Game.XnbPropertyAttribute";
    
    private struct XnbTypeInfo
    {
        public string TypeName;
        public string TypeFullName;
        public string QualifierString;
        public List<XnbPropertyInfo> Properties;
    }

    private struct XnbPropertyInfo
    {
        public string Name;
        public string TypeFullName;
        public bool IsNullable;
        public bool IsReferenceType;
        public int Order;
        public bool UseConverter;
        public bool Optional;
        public bool SkipIdentifier;
    }

    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        var xnbTypeInfos = context.SyntaxProvider
            .CreateSyntaxProvider((node, _) => node is TypeDeclarationSyntax { AttributeLists.Count: > 0 }, GetXnbType)
            .Where(m => m != null);

        context.RegisterSourceOutput(xnbTypeInfos, (ctx, xnbTypeInfo) => 
            CreateSerializerSourceFile(ctx, xnbTypeInfo!.Value));
    }

    private static XnbTypeInfo? GetXnbType(GeneratorSyntaxContext ctx, CancellationToken ct)
    {
        if (ctx.SemanticModel.GetDeclaredSymbol(ctx.Node, ct) is not INamedTypeSymbol typeSymbol)
        {
            return null;
        }
        
        if (typeSymbol.IsGenericType)
        {
            return null;
        }
        
        var xnbReaderTypeAttribute = typeSymbol.GetAttributes()
            .FirstOrDefault(a => a.AttributeClass?.ToDisplayString() == XnbReaderTypeAttributeName);

        if (xnbReaderTypeAttribute is not {ConstructorArguments.Length: > 0})
        {
            return null;
        }
        
        bool useBaseClass = xnbReaderTypeAttribute.NamedArguments
            .FirstOrDefault(x => x.Key == "UseBaseClass").Value.Value is true;

        var logicalTypeSymbol = typeSymbol;
        if (useBaseClass && typeSymbol.BaseType is not null)
        {
            logicalTypeSymbol = typeSymbol.BaseType;
        }

        var qualifierString = xnbReaderTypeAttribute.ConstructorArguments[0].Value as string ?? string.Empty;
        var properties = new List<XnbPropertyInfo>();

        foreach (var member in logicalTypeSymbol.GetMembers().OfType<IPropertySymbol>())
        {
            ct.ThrowIfCancellationRequested();

            var xnbPropertyAttribute = member.GetAttributes()
                .FirstOrDefault(a => a.AttributeClass?.ToDisplayString() == XnbPropertyAttributeName);
            
            if (xnbPropertyAttribute == null)
            {
                continue;
            }

            int order = (int?)(xnbPropertyAttribute.ConstructorArguments.FirstOrDefault()).Value ?? 0;

            bool useConverter = xnbPropertyAttribute.NamedArguments
                .FirstOrDefault(x => x.Key == "UseConverter").Value.Value is true;
            bool optional = xnbPropertyAttribute.NamedArguments
                .FirstOrDefault(x => x.Key == "Optional").Value.Value is true;
            bool skipIdentifier = xnbPropertyAttribute.NamedArguments
                .FirstOrDefault(x => x.Key == "SkipIdentifier").Value.Value is true;

            ITypeSymbol underlyingPropertyType = member.Type;
            bool propertyNullable = false;

            if (member.Type is INamedTypeSymbol {ConstructedFrom.SpecialType: SpecialType.System_Nullable_T} namedType)
            {
                underlyingPropertyType = namedType.TypeArguments[0];
                propertyNullable = true;
            }

            properties.Add(new XnbPropertyInfo
            {
                Name = member.Name,
                TypeFullName = underlyingPropertyType.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat),
                IsNullable = propertyNullable,
                IsReferenceType = !propertyNullable && member.Type.IsReferenceType,
                Order = order,
                UseConverter = useConverter,
                Optional = optional,
                SkipIdentifier = skipIdentifier
            });
        }

        properties.Sort((a, b) => a.Order.CompareTo(b.Order));

        return new XnbTypeInfo
        {
            TypeName = typeSymbol.Name,
            TypeFullName = logicalTypeSymbol.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat),
            QualifierString = qualifierString,
            Properties = properties,
        };
    }

    private static void CreateSerializerSourceFile(SourceProductionContext ctx, XnbTypeInfo model)
    {
        var cb = new CodeStringBuilder();
        EmitSerializer(cb, model);
        ctx.AddSource($"{model.TypeName}ContentSerializer.g.cs", cb.ToString());
    }

    private static void EmitSerializer(CodeStringBuilder cb, XnbTypeInfo xnbTypeInfo)
    {
        cb.AppendLine("// <auto-generated/>");
        cb.AppendLine("// FEZRepacker.Core.SourceGen output");
        cb.AppendLine("#nullable enable");
        cb.AppendLine();
        cb.AppendLine("using FEZRepacker.Core;");
        cb.AppendLine("using FEZRepacker.Core.Helpers;");
        cb.AppendLine("using FEZRepacker.Core.XNB;");
        cb.AppendLine();
        cb.AppendLine("namespace FEZRepacker.Core.XNB.ContentSerialization;");
        cb.AppendLine();
        cb.Append($"internal sealed class {xnbTypeInfo.TypeName}ContentSerializer");
        cb.AppendLine($" : XnbContentSerializer<{xnbTypeInfo.TypeFullName}>");
        cb.BeginCodeBlock();
        {
            cb.AppendLine($"public override XnbAssemblyQualifier Name => \"{xnbTypeInfo.QualifierString}\";");
            cb.AppendLine();
            EmitDeserialize(cb, xnbTypeInfo);
            cb.AppendLine();
            EmitSerialize(cb, xnbTypeInfo);
        }
        cb.EndCodeBlock();
    }

    private static void EmitDeserialize(CodeStringBuilder cb, XnbTypeInfo model)
    {
        cb.AppendLine("public override object Deserialize(XnbContentReader reader)");
        cb.BeginCodeBlock();
        {
            cb.AppendLine($"var content = new {model.TypeFullName}();");

            foreach (var prop in model.Properties)
            {
                EmitPropertyDeserialize(cb, prop);
            }

            cb.AppendLine("return content;");
        }
        cb.EndCodeBlock();
    }

    private static void EmitPropertyDeserialize(CodeStringBuilder cb, XnbPropertyInfo prop)
    {
        if (prop.Optional)
        {
            cb.AppendLine($"if (reader.ReadBoolean())");
            cb.BeginCodeBlock();
        }
        
        cb.Append($"content.{prop.Name} = ");

        if (prop.UseConverter)
        {
            var castType = $"{prop.TypeFullName}{(prop.IsNullable ? "?" : "")}";
            cb.Append($"({castType})reader.ReadContent(typeof({prop.TypeFullName}), ");
            cb.Append($"{(prop.SkipIdentifier ? "true" : "false")}){(prop.IsNullable ? "" : "!")}");
        }
        else
        {
            cb.Append(prop.TypeFullName switch
            {
                "bool" => "reader.ReadBoolean()",
                "int" => "reader.ReadInt32()",
                "byte" => "reader.ReadByte()",
                "short" => "reader.ReadInt16()",
                "float" => "reader.ReadSingle()",
                "char" => "reader.ReadChar()",
                "string" => "reader.ReadString()",
                "global::FEZRepacker.Core.Definitions.Game.XNA.Vector2" => "reader.ReadVector2()",
                "global::FEZRepacker.Core.Definitions.Game.XNA.Vector3" => "reader.ReadVector3()",
                "global::FEZRepacker.Core.Definitions.Game.XNA.Quaternion" => "reader.ReadQuaternion()",
                "global::FEZRepacker.Core.Definitions.Game.XNA.Color" => "reader.ReadColor()",
                "global::System.TimeSpan" => "new global::System.TimeSpan(reader.ReadInt64())",
                _ => $"default! /* unsupported type: {prop.TypeFullName} */"
            });
        }
        
        cb.AppendLine(";");
        
        if (prop.Optional)
        {
            cb.EndCodeBlock();
        }
    }

    private static void EmitSerialize(CodeStringBuilder cb, XnbTypeInfo model)
    {
        cb.AppendLine("public override void Serialize(object data, XnbContentWriter writer)");
        cb.BeginCodeBlock();
        {
            cb.AppendLine($"var content = ({model.TypeFullName})data;");

            foreach (var prop in model.Properties)
            {
                EmitPropertySerialize(cb, prop);
            }
        }
        cb.EndCodeBlock();
    }

    private static void EmitPropertySerialize(CodeStringBuilder cb, XnbPropertyInfo prop)
    {
        var valueExpression = $"content.{prop.Name}";
        var propertyType = $"typeof({prop.TypeFullName})";
        
        if (prop.Optional)
        {
            if (prop.IsNullable)
            {
                cb.AppendLine($"if ({valueExpression}.HasValue)");
                valueExpression += ".Value";
            }
            else if (prop.IsReferenceType)
            {
                cb.AppendLine($"if ({valueExpression} != null)");
            }
            else
            {
                cb.AppendLine($"// {prop.Name}");
            }

            cb.BeginCodeBlock();
            cb.AppendLine("writer.Write(true);");
        }

        if (prop.UseConverter)
        {
            cb.Append($"writer.WriteContent({propertyType}, {valueExpression}, ");
            cb.Append($"{(prop.SkipIdentifier ? "true" : "false")})");
        }
        else
        {
            cb.Append(prop.TypeFullName switch
            {
                "bool" => $"writer.Write({valueExpression})",
                "int" => $"writer.Write({valueExpression})",
                "byte" => $"writer.Write({valueExpression})",
                "short" => $"writer.Write({valueExpression})",
                "float" => $"writer.Write({valueExpression})",
                "char" => $"writer.Write({valueExpression})",
                "string" => $"writer.Write({valueExpression})",
                "global::FEZRepacker.Core.Definitions.Game.XNA.Vector2" => $"writer.Write({valueExpression})",
                "global::FEZRepacker.Core.Definitions.Game.XNA.Vector3" => $"writer.Write({valueExpression})",
                "global::FEZRepacker.Core.Definitions.Game.XNA.Quaternion" => $"writer.Write({valueExpression})",
                "global::FEZRepacker.Core.Definitions.Game.XNA.Color" => $"writer.Write({valueExpression})",
                "global::System.TimeSpan" => $"writer.Write({valueExpression}.Ticks)",
                _ => $"_ = {valueExpression} /* unsupported type: {prop.TypeFullName} */"
            });
        }
        cb.AppendLine(";");

        if (prop.Optional)
        {
            cb.EndCodeBlock();

            if (prop.IsNullable || prop.IsReferenceType)
            {
                cb.AppendLine("else");
                cb.BeginCodeBlock();
                cb.AppendLine("writer.Write(false);");
                cb.EndCodeBlock();
            }
        }
    }
}
    
