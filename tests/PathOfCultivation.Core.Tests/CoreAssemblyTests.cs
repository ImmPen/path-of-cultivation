using System.Reflection;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace PathOfCultivation.Core.Tests;

[TestClass]
public sealed class CoreAssemblyTests
{
    [TestMethod]
    public void CoreAssemblyHasExpectedNameAndNoDirectGameReferences()
    {
        Assembly coreAssembly = Assembly.Load("PathOfCultivation.Core");

        Assert.AreEqual("PathOfCultivation.Core", coreAssembly.GetName().Name);

        string[] forbiddenReferences =
        [
            "Verse", "RimWorld", "Assembly-CSharp", "UnityStubs",
            "Waylith", "Harmony", "0Harmony"
        ];

        foreach (AssemblyName reference in coreAssembly.GetReferencedAssemblies())
        {
            Assert.IsFalse(
                forbiddenReferences.Contains(reference.Name, StringComparer.OrdinalIgnoreCase),
                $"Core must not directly reference {reference.Name}.");
        }
    }
}
