using DocGen;

// Composition root: wire the pipeline and write the guide.
// Usage: DocGen [requirementsDir] [outputPath]
string requirementsDir = args.Length > 0 ? args[0] : "requirements";
string outputPath = args.Length > 1 ? args[1] : Path.Combine("docs", "user", "user-guide.md");

var generator = new UserGuideGenerator(
    new FileRequirementSource(requirementsDir),
    new RequirementParser(),
    new GuideProjection(),
    new MarkdownGuideRenderer());

string guide = generator.Generate();

string? outputDir = Path.GetDirectoryName(outputPath);
if (!string.IsNullOrEmpty(outputDir))
{
    Directory.CreateDirectory(outputDir);
}

File.WriteAllText(outputPath, guide);
Console.WriteLine($"Wrote user guide to {outputPath}");
