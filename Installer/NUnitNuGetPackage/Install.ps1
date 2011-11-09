param($installPath, $toolsPath, $package, $project)

$nunitPackage = Get-Package -Filter NUnit -First 1

$nunitPackageToolsLibFolder = $installPath + "\..\NUnit." + $nunitPackage.Version.ToString() + "\tools\lib\"

$nunitCoreRef = $project.Object.References.Item("nunit.core.interfaces")
if ($nunitCoreRef) { $nunitCoreRef.Remove() }

$project.Object.References.Add($nunitPackageToolsLibFolder + "nunit.core.interfaces.dll")
