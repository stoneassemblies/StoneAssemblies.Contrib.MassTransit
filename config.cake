string NuGetVersionV2 = "";
string SolutionFileName = "src/StoneAssemblies.Contrib.MassTransit.sln";

string[] DockerFiles = System.Array.Empty<string>();

string[] OutputImages = System.Array.Empty<string>();

string[] ComponentProjects  = new [] {
	"./src/StoneAssemblies.Contrib.MassTransit/StoneAssemblies.Contrib.MassTransit.csproj",
};

string TestProject = "src/StoneAssemblies.Contrib.MassTransit.Tests/StoneAssemblies.Contrib.MassTransit.Tests.csproj";

string SonarProjectKey = "StoneAssemblies.Contrib.MassTransit";
string SonarOrganization = "stoneassemblies";