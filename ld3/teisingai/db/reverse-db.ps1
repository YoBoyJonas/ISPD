#!/snap/bin/pwsh
# NOTE. Change the above path to where powershell executable is found on your system.

<#
Create EF DB context from existing DB tables.
#>

#define DBVS connection parameters
$DBConnStr = "Server=localhost;User=root;Password=root;Database=t120b197";
$DBProvider = "Microting.EntityFrameworkCore.MySql"

#define DB metadata
$DBContextName = "DB"
$DBContextNamespace = "Org.Ktu.T120B197.DBs"
$DBEntitiesNamespace = "Org.Ktu.T120B197.DBs.Entities"

$ContextDstDir = "./"
$EntitiesDstDir = "./Entities"

#clean destination folders
Remove-Item ($EntitiesDstDir + "/*")
Remove-Item ($ContextDstDir + $DBContextName + ".Generated.cs")

#define tables to include
$Tables = `
	@(
		"Author",
		"Book",
		"Customer",
		"Orders",
		"OrderItem"
	)
$Tables = $Tables | ForEach-Object {@("--table" , $_)}

#build command line arguments and invoke the reverse engineering tool
$CmdLineArgs = `
	@(
		"dbcontext",
		"scaffold",
		$DBConnStr,
		$DBProvider,
		"--no-build",
		"--data-annotations",
		"--use-database-names",
		"--no-pluralize",
		"--context-dir", $ContextDstDir,
		"--context", ($DBContextName + "Generated"),
		"--context-namespace", $DBContextNamespace,
		"--output-dir", $EntitiesDstDir,
		"--namespace", $DBEntitiesNamespace,
		"--no-onconfiguring"
	)
$CmdLineArgs = $CmdLineArgs + $Tables

& "dotnet-ef" $CmdLineArgs

#rename generated file to proper naming conventions
Rename-Item -Path ($ContextDstDir + $DBContextName + "Generated.cs") -NewName ($DBContextName + ".Generated.cs")

#change generated class name to a proper one
(Get-Content -path ($ContextDstDir + $DBContextName + ".Generated.cs") -Raw) `
	-replace ($DBContextName + "Generated"), $DBContextName `
	| Set-Content -Path ($ContextDstDir + $DBContextName + ".Generated.cs")

#hide generated constructors
(Get-Content -path ($ContextDstDir + $DBContextName + ".Generated.cs") -Raw) `
	-replace ("public " + $DBContextName), ("private " + $DBContextName) `
	| Set-Content -Path ($ContextDstDir + $DBContextName + ".Generated.cs")

#unhide no-args constructor
(Get-Content -path ($ContextDstDir + $DBContextName + ".Generated.cs") -Raw) `
	-replace ("private " + $DBContextName + "\(\)"), ("public " + $DBContextName + "()") `
	| Set-Content -Path ($ContextDstDir + $DBContextName + ".Generated.cs")
