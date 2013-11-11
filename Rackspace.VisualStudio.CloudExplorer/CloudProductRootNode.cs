namespace Rackspace.VisualStudio.CloudExplorer
{
    using System;
    using Microsoft.VSDesigner.ServerExplorer;
    using Newtonsoft.Json.Linq;
    using File = System.IO.File;

    public abstract class CloudProductRootNode : AsyncNode
    {
        private static Lazy<JObject> _developerSettings = new Lazy<JObject>(
            () =>
            {
                if (File.Exists(@"C:\.openstack_net"))
                    return JObject.Parse(File.ReadAllText(@"C:\.openstack_net"));

                return null;
            }, true);

        protected CloudProductRootNode()
        {
        }

        public static JObject DeveloperSettings
        {
            get
            {
                return _developerSettings.Value;
            }
        }

        public override bool CanDeleteNode()
        {
            return false;
        }

        public override sealed int CompareUnique(Node node)
        {
            return Label.CompareTo(node.Label);
        }
    }
}
