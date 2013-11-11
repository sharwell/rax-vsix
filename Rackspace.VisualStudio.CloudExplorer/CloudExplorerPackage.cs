namespace Rackspace.VisualStudio.CloudExplorer
{
    using System;
    using System.Runtime.InteropServices;
    using Microsoft.VisualStudio.Shell;

    [ProvideServerExplorerNode(typeof(RackspaceProductsNode))]
    [ProvideBindingPath]
    [PackageRegistration(UseManagedResourcesOnly = true)]
    [InstalledProductRegistration("#110", "#112", "1.0")]
    [Guid("E51AAEFC-E050-40F2-91D8-5055285A03C6")]
    public class CloudExplorerPackage : Package
    {
    }
}
