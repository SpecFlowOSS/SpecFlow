param($installPath, $toolsPath, $package, $project)

$version = ""

# Get-Package -Filter NUnit.Runners also returns packages with NUnit.Runners in the description (e.g. NUnit or SpecFlow.NUnit.Runners)
# we need to find the one, that we are looking for

$nunitPackages = Get-Package -Filter NUnit.Runners
foreach ($nunitPackage in $nunitPackages)
{
	# there is a bug in NuGet: the package id for all returned packages is "NUnit.Runners", so filtering that is not enough
	if ($nunitPackage.Id = "NUnit.Runners" -and $nunitPackage.ProjectUrl.ToString().Contains("nunit")) 
	{ 
		$version = $nunitPackage.Version.ToString() 
	}
}

if ($version -ne "") 
{ 
	$nunitPackageToolsLibFolder = $installPath + "\..\NUnit.Runners." + $version + "\tools\lib\"

	$nunitCoreRef = $project.Object.References.Item("nunit.core.interfaces")
	if ($nunitCoreRef) { $nunitCoreRef.Remove() }

	$project.Object.References.Add($nunitPackageToolsLibFolder + "nunit.core.interfaces.dll")
}

