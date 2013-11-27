namespace Rackspace.VisualStudio.CloudExplorer
{
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.VSDesigner.ServerExplorer;
    using Image = System.Drawing.Image;

    public class NotImplementedPlaceholderNode : AsyncNode
    {
        protected override string DisplayText
        {
            get
            {
                return "Not yet implemented";
            }
        }

        public override bool IsAlwaysLeaf()
        {
            return true;
        }

        protected override Task<Node[]> CreateChildrenAsync(CancellationToken cancellationToken)
        {
            return Task.FromResult(RackspaceProductsNode.EmptyChildren);
        }

        public override int CompareUnique(Node node)
        {
            return Label.CompareTo(node.Label);
        }

        public override Image Icon
        {
            get
            {
                return RackspaceProductsNode.EmptyIcon;
            }
        }
    }
}
