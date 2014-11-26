namespace Rackspace.VisualStudio.CloudExplorer.Files
{
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.VSDesigner.ServerExplorer;
    using Image = System.Drawing.Image;

    public class DeletedContainerPlaceholderNode : AsyncNode
    {
        protected override string DisplayText
        {
            get
            {
                return "Container has been removed";
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

        public override Image Icon
        {
            get
            {
                return RackspaceProductsNode.EmptyIcon;
            }
        }
    }
}
