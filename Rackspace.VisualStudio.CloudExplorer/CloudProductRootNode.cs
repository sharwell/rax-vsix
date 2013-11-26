namespace Rackspace.VisualStudio.CloudExplorer
{
    using Microsoft.VSDesigner.ServerExplorer;

    public abstract class CloudProductRootNode : AsyncNode
    {
        protected CloudProductRootNode()
        {
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
