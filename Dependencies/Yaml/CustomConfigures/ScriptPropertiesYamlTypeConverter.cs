using FEZEngine.Structure.Scripting;
using YamlDotNet.Core;
using YamlDotNet.Core.Events;
using YamlDotNet.Serialization;

namespace FEZRepacker.Dependencies.Yaml.CustomConfigures
{
    class ScriptPropertiesYamlTypeConverter : IYamlTypeConverter
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

        private static string ToPropertyIdentifier(Entity entity, string property)
        {
            string output = $"{entity.Type}";
            if (entity.Identifier.HasValue)
            {
                output += $"[{entity.Identifier.Value}]";
            }
            output += $".{property}";
            return output;
        }

        private static (Entity,string) FromPropertyIdentifier(string identifier)
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


        public bool Accepts(Type type)
        {
            return
                type == typeof(ScriptTrigger) ||
                type == typeof(ScriptCondition) ||
                type == typeof(ScriptAction);
        }
        public object? ReadYaml(IParser parser, Type type)
        {
            var value = parser.Consume<Scalar>().Value;
            if (value == null) return null;

            if (type == typeof(ScriptTrigger))
            {
                ScriptTrigger trigger = new ScriptTrigger();
                (trigger.Object, trigger.Event) = FromPropertyIdentifier(value);
                return trigger;
            }
            else if (type == typeof(ScriptCondition))
            {
                ScriptCondition cond = new ScriptCondition();

                cond.Operator = OperatorLookup.FirstOrDefault(o => value.Contains(o.Value)).Key;

                if (cond.Operator != ComparisonOperator.None)
                {
                    string[] split = value.Split(OperatorLookup[cond.Operator]);
                    (cond.Object, cond.Property) = FromPropertyIdentifier(split[0].Trim());
                    if(split.Length > 1) cond.Value = split[1].Trim();
                }

                return cond;
            }
            else if (type == typeof(ScriptAction))
            {
                ScriptAction action = new ScriptAction();

                string[] afterFuncSplit = value.Split(')');

                string[] beforeFuncSplit = afterFuncSplit[0].Split('(');
                (action.Object, action.Operation) = FromPropertyIdentifier(beforeFuncSplit[0]);
                if (beforeFuncSplit.Length > 1 && beforeFuncSplit[1].Length > 0)
                {
                    action.Arguments = beforeFuncSplit[1].Split(',');
                    for(int i = 0; i < action.Arguments.Length; i++)
                    {
                        action.Arguments[i] = action.Arguments[i].Trim();
                    }
                }
                if(afterFuncSplit.Length > 1 && afterFuncSplit[1].Length > 0)
                {
                    string[] flags = afterFuncSplit[1].Split(',');
                    foreach(string flag in flags){
                        string f = flag.Trim().ToLower();
                        if (f == "killswitch") action.Killswitch = true;
                        if (f == "blocking") action.Blocking = true;
                    }
                }

                return action;
            }
            else return null;
        }


        public void WriteYaml(IEmitter emitter, object? value, Type type)
        {
            if (value == null) return;

            string output = "";
            if (value is ScriptTrigger trigger)
            {
                output = $"{ToPropertyIdentifier(trigger.Object, trigger.Event)}";
            }
            if (value is ScriptCondition cond)
            {
                output = $"{ToPropertyIdentifier(cond.Object, cond.Property)}";
                output += $" {OperatorLookup[cond.Operator]} {cond.Value}";
            }
            if (value is ScriptAction action)
            {
                output = $"{ToPropertyIdentifier(action.Object, action.Operation)}";
                output += "(";
                for(int i = 0; i < action.Arguments.Count(); i++)
                {
                    if (i > 0) output += ", ";
                    output += action.Arguments[i];
                }
                output += ")";
                if (action.Killswitch) output += ", Killswitch";
                if (action.Blocking) output += ", Blocking";
            }

            emitter.Emit(new Scalar(TagName.Empty, output));
        }
    }
}
