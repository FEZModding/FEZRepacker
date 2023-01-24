using FEZRepacker.Converter.Definitions.FezEngine.Structure.Scripting;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace FEZRepacker.Converter.XNB.Formats.Json.CustomConverters
{
    internal static class EntityJsonConverter
    {
        public static string ToPropertyIdentifier(Entity entity, string property)
        {
            string output = $"{entity.Type}";
            if (entity.Identifier.HasValue)
            {
                output += $"[{entity.Identifier.Value}]";
            }
            output += $".{property}";
            return output;
        }
        public static (Entity, string) FromPropertyIdentifier(string identifier)
        {
            string[] split = identifier.Split('.');
            string entityStr = split[0];
            string property = split.Length > 1 ? split[1] : "";

            string[] entitySplit = entityStr.Split('[');
            Entity ent = new Entity();
            ent.Type = entitySplit[0];
            if (entitySplit.Length > 1)
            {
                string id = entitySplit[1];
                ent.Identifier = Int32.Parse(id.Substring(0, id.Length - 1));
            }
            return (ent, property);
        }
    }



    internal class ScriptTriggerJsonConverter : JsonConverter<ScriptTrigger>
    {
        public bool Accepts(Type type) => type == typeof(ScriptTrigger);
        
        public override ScriptTrigger Read(
            ref Utf8JsonReader reader,
            Type type,
            JsonSerializerOptions options)
        {
            var value = reader.GetString();
            if (value == null) throw new JsonException();

            if (type == typeof(ScriptTrigger))
            {
                ScriptTrigger trigger = new ScriptTrigger();
                (trigger.Object, trigger.Event) = EntityJsonConverter.FromPropertyIdentifier(value);
                return trigger;
            }
            else throw new JsonException();
        }


        public override void Write(
            Utf8JsonWriter writer,
            ScriptTrigger value,
            JsonSerializerOptions options)
        {
            string output = $"{EntityJsonConverter.ToPropertyIdentifier(value.Object, value.Event)}";
            writer.WriteStringValue(output);
        }
    }

    internal class ScriptConditionJsonConverter : JsonConverter<ScriptCondition>
    {
        private static readonly Dictionary<ComparisonOperator, string> OperatorLookup = new Dictionary<ComparisonOperator, string>()
        {
            { ComparisonOperator.None, "?" },
            { ComparisonOperator.Equal, "==" },
            { ComparisonOperator.GreaterEqual, ">=" },
            { ComparisonOperator.LessEqual, "<=" },
            { ComparisonOperator.Greater, ">" },
            { ComparisonOperator.Less, "<" },
            { ComparisonOperator.NotEqual, "!=" },
        };


        public bool Accepts(Type type) => type == typeof(ScriptCondition);
        
        public override ScriptCondition Read(
            ref Utf8JsonReader reader,
            Type type,
            JsonSerializerOptions options)
        {
            var value = reader.GetString();
            if (value == null) throw new JsonException();

            ScriptCondition cond = new ScriptCondition();

            cond.Operator = OperatorLookup.FirstOrDefault(o => value.Contains(o.Value)).Key;

            if (cond.Operator != ComparisonOperator.None)
            {
                string[] split = value.Split(OperatorLookup[cond.Operator]);
                (cond.Object, cond.Property) = EntityJsonConverter.FromPropertyIdentifier(split[0].Trim());
                if (split.Length > 1) cond.Value = split[1].Trim();
            }

            return cond;
        }


        public override void Write(
            Utf8JsonWriter writer,
            ScriptCondition value,
            JsonSerializerOptions options)
        {
            string output = $"{EntityJsonConverter.ToPropertyIdentifier(value.Object, value.Property)}";
            output += $" {OperatorLookup[value.Operator]} {value.Value}";

            writer.WriteStringValue(output);
        }
    }

    internal class ScriptActionJsonConverter : JsonConverter<ScriptAction>
    {
        public bool Accepts(Type type) => type == typeof(ScriptAction);

        public override ScriptAction Read(
            ref Utf8JsonReader reader,
            Type type,
            JsonSerializerOptions options)
        {
            var value = reader.GetString();
            if (value == null) throw new JsonException();

            ScriptAction action = new ScriptAction();

            while (value[0] == '!' || value[0] == '#'){
                if (value[0] == '!') action.Killswitch = true;
                if (value[0] == '#') action.Blocking = true;
                value = value.Substring(1);
            }

            string[] afterFuncSplit = value.Split(')');

            string[] beforeFuncSplit = afterFuncSplit[0].Split('(');
            (action.Object, action.Operation) = EntityJsonConverter.FromPropertyIdentifier(beforeFuncSplit[0]);
            if (beforeFuncSplit.Length > 1 && beforeFuncSplit[1].Length > 0)
            {
                action.Arguments = beforeFuncSplit[1].Split(',');
                for (int i = 0; i < action.Arguments.Length; i++)
                {
                    action.Arguments[i] = action.Arguments[i].Trim();
                }
            }

            return action;
        }


        public override void Write(
            Utf8JsonWriter writer,
            ScriptAction value,
            JsonSerializerOptions options)
        {

            string output = $"{EntityJsonConverter.ToPropertyIdentifier(value.Object, value.Operation)}";
            output += "(";
            for (int i = 0; i < value.Arguments.Count(); i++)
            {
                if (i > 0) output += ", ";
                output += value.Arguments[i];
            }
            output += ")";
            if (value.Blocking) output = "#" + output;
            if (value.Killswitch) output = "!" + output;

            writer.WriteStringValue(output);
        }
    }
}
