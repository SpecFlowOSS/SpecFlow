param($installPath, $toolsPath, $package, $project)

$nunitCoreRef = $project.Object.References.Item("nunit.core.interfaces")
if ($nunitCoreRef) { $nunitCoreRef.Remove() }
