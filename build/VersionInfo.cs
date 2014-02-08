using System.Diagnostics.CodeAnalysis;
using System.Reflection;

// Version information for an assembly consists of the following four values:
//
//      Major Version
//      Minor Version 
//      Build Number
//      Revision
//
// You can specify all the values or you can default the Build and Revision Numbers 
// by using the '*' as shown below:
// [assembly: AssemblyVersion("1.0.*")]
[assembly: AssemblyVersion("1.0.0.0")]
[assembly: AssemblyFileVersion("1.0.0.0")]
[assembly: AssemblyInformationalVersion("1.0.0-beta")]

[assembly: SuppressMessage("Microsoft.Usage", "CA2243:AttributeStringLiteralsShouldParseCorrectly", Justification = "Need to include suffix in AssemblyInformationalVersion.")]