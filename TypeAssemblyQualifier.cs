using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FEZRepacker
{
    struct TypeAssemblyQualifier
    {
        public string Namespace;
        public string Name;
        public TypeAssemblyQualifier[] Templates;

        public TypeAssemblyQualifier(string fullName)
        {
            // there's a bunch of info about version and such
            // let's hope we don't need that in the future lmfao
            var noInfoFullName = fullName;
            var endOfTemplatesIndex = fullName.LastIndexOf(']');

            var additionalInfoSplit = fullName.IndexOf(',', Math.Max(0,endOfTemplatesIndex));
            if(additionalInfoSplit != -1)
            {
                noInfoFullName = fullName.Substring(0, additionalInfoSplit);
            }
            

            // splitting name into actual name and templates
            var noTemplateFullName = noInfoFullName;
            var templatesStr = "";

            var templateSeparatorIndex = noInfoFullName.IndexOf('`');
            if(templateSeparatorIndex != -1)
            {
                noTemplateFullName = noInfoFullName.Substring(0, templateSeparatorIndex);
                templatesStr = noInfoFullName.Substring(templateSeparatorIndex + 1);
            }


            // separating namespace from the name
            var namespaceSplit = noTemplateFullName.Split('.');
            if(namespaceSplit.Length > 1)
            {
                Namespace = String.Join('.', namespaceSplit.SkipLast(1));
                Name = namespaceSplit[namespaceSplit.Length - 1];
            }
            else
            {
                Namespace = "";
                Name = noTemplateFullName;
            }

            // type uses templates
            if(templatesStr.Length > 0)
            {
                int templateCount = Int32.Parse(templatesStr.Substring(0, 1));
                string templatesStrInner = templatesStr.Substring(2, templatesStr.Length - 3);

                List<string> templates = new List<string>();

                int depth = 0;
                int beginIndex = 0;
                for(int i = 0; i < templatesStrInner.Length; i++)
                {
                    if(templatesStrInner[i] == '[')
                    {
                        if (depth == 0) beginIndex = i;
                        depth++;
                    }
                    if(templatesStrInner[i] == ']')
                    {
                        depth--;
                        if(depth == 0)
                        {
                            templates.Add(templatesStrInner.Substring(beginIndex+1, i - beginIndex - 1));
                        }
                    }
                }

                Templates = new TypeAssemblyQualifier[templateCount];
                for(var i = 0; i < templateCount; i++)
                {
                    Templates[i] = new TypeAssemblyQualifier(templates[i]);
                }
            }
            else
            {
                Templates = new TypeAssemblyQualifier[0];
            }
        }

        public string GetDisplayName(bool simplified=false)
        {
            string displayedName = (simplified ? "" : (Namespace + ".")) + Name;
            if(Templates.Length > 0)
            {
                displayedName += (simplified) ? "[" : $"`{Templates.Length}[";
                for (var i=0;i<Templates.Length;i++)
                {
                    if (i > 0) displayedName += ",";
                    var templateName = Templates[i].GetDisplayName(simplified);
                    displayedName += (simplified) ? templateName : $"[{templateName}]";
                }
                displayedName += "]";
            }

            return displayedName;
        }

        public override string ToString()
        {
            return GetDisplayName(false);
        }

        public override bool Equals(object? obj)
        {
            if (obj is TypeAssemblyQualifier)
            {
                TypeAssemblyQualifier taq = (TypeAssemblyQualifier)obj;
                return (Namespace == taq.Namespace) && (Name == taq.Name) && Templates.SequenceEqual(taq.Templates);
            }
            else
            {
                return base.Equals(obj);
            }
        }
    }
}
