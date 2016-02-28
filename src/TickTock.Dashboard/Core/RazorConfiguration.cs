using Nancy.ViewEngines.Razor;
using System.Collections.Generic;

namespace TickTock.Dashboard.Core
{
    public class RazorConfiguration : IRazorConfiguration
    {
        public bool AutoIncludeModelNamespace
        {
            get { return true; }
        }

        public IEnumerable<string> GetAssemblyNames()
        {
            yield break;
        }

        public IEnumerable<string> GetDefaultNamespaces()
        {
            yield return "Nancy.ViewEngines.Razor";
        }
    }
}