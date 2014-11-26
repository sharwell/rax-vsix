using System.Reflection;
using System.Runtime.InteropServices;
using Microsoft.VisualStudio.Shell;

// General Information about an assembly is controlled through the following 
// set of attributes. Change these attribute values to modify the information
// associated with an assembly.
[assembly: AssemblyTitle("Rackspace.VisualStudio.CloudExplorer")]
[assembly: AssemblyDescription("Server Explorer support for Rackspace cloud products")]
[assembly: AssemblyConfiguration("")]
[assembly: AssemblyCompany("Rackspace, Inc.")]
[assembly: AssemblyProduct("Rackspace.VisualStudio.CloudExplorer")]
[assembly: AssemblyCopyright("Copyright © Sam Harwell 2013")]
[assembly: AssemblyTrademark("")]
[assembly: AssemblyCulture("")]

[assembly: ProvideBindingRedirection(
    AssemblyName = "Newtonsoft.Json",
    OldVersionLowerBound = "4.5.0.0",
    OldVersionUpperBound = "6.0.0.0",
    NewVersion = "6.0.0.0",
    Culture = "neutral",
    PublicKeyToken = "30ad4fe6b2a6aeed")]

// Setting ComVisible to false makes the types in this assembly not visible 
// to COM components.  If you need to access a type in this assembly from 
// COM, set the ComVisible attribute to true on that type.
[assembly: ComVisible(false)]

[assembly: AssemblyVersion("1.0.0.0")]
[assembly: AssemblyFileVersion("1.0.0.0")]
[assembly: AssemblyInformationalVersion("1.0.0.0-dev")]
