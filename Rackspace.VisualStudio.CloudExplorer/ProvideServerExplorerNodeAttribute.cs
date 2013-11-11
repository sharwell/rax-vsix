namespace Rackspace.VisualStudio.CloudExplorer
{
    using System;
    using System.Globalization;
    using Microsoft.VisualStudio.Shell;

    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = true)]
    internal sealed class ProvideServerExplorerNodeAttribute : RegistrationAttribute
    {
        private readonly Type _nodeType;

        public ProvideServerExplorerNodeAttribute(Type nodeType)
        {
            if (nodeType == null)
                throw new ArgumentNullException("nodeType");

            _nodeType = nodeType;
        }

        public override void Register(RegistrationContext context)
        {
            if (context == null)
                throw new ArgumentNullException("context");

            using (Key key = context.CreateKey(GetKeyName(_nodeType)))
            {
                key.SetValue("Assembly", _nodeType.Assembly.GetName().FullName);
                key.SetValue("ClassName", _nodeType.FullName);
            }
        }

        public override void Unregister(RegistrationContext context)
        {
            if (context != null)
                context.RemoveKey(GetKeyName(_nodeType));
        }

        private static string GetKeyName(Type nodeType)
        {
            if (nodeType == null)
                throw new ArgumentNullException("nodeType");

            return string.Format(CultureInfo.InvariantCulture, @"ServerExplorer\Nodes\{0}", nodeType.GUID.ToString("B"));
        }
    }
}
