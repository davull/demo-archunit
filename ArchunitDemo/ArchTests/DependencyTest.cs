using ArchunitDemo.AnotherDesktopModule;
using ArchunitDemo.BusinessModule;
using ArchunitDemo.DataModule;
using ArchunitDemo.DesktopModule;
using ArchUnitNET.Domain;
using ArchUnitNET.Fluent.Slices;
using ArchUnitNET.Loader;
using ArchUnitNET.xUnit;
using Xunit;

namespace ArchTests;

using static ArchUnitNET.Fluent.ArchRuleDefinition;

public class DependencyTest
{
    private static readonly Architecture Architecture = new ArchLoader()
        .LoadAssemblies(assemblies: new[]
        {
            typeof(DataModuleMarker).Assembly,
            typeof(BusinessModuleMarker).Assembly,
            typeof(DesktopModuleMarker).Assembly,
            typeof(AnotherDesktopModuleMarker).Assembly,
        })
        .Build();

    // Define layers
    private readonly IObjectProvider<IType> DataLayer =
        Types().That().ResideInAssembly(typeof(DataModuleMarker).Assembly).As("Data layer");
    
    private readonly IObjectProvider<IType> BusinessLayer =
        Types().That().ResideInAssembly(typeof(BusinessModuleMarker).Assembly).As("Business layer");
    
    private readonly IObjectProvider<IType> DesktopLayer =
        Types().That().ResideInAssembly(typeof(DesktopModuleMarker).Assembly).As("Desktop layer");
    
    private readonly IObjectProvider<IType> AnotherDesktopLayer =
        Types().That().ResideInAssembly(typeof(AnotherDesktopModuleMarker).Assembly).As("Another Desktop layer");
    
    // Repositories
    private readonly IObjectProvider<Class> RepositoryClasses =
        Classes().That().HaveNameEndingWith("Repository").As("Repository classes");

    [Fact]
    public void RepositoryClassesShouldBeInDataLayer()
    {
        // Arrange
        var rule = Classes().That().Are(RepositoryClasses).Should().Be(DataLayer);
        
        // Assert
        rule.Check(Architecture);
    }
    
    [Fact]
    public void RepositoryClassesShouldNotBeInBusinessLayer()
    {
        // Arrange
        var rule = Classes().That().Are(RepositoryClasses).Should().NotBe(BusinessLayer);
        
        // Assert
        rule.Check(Architecture);
    }

    [Fact]
    public void ShouldNotHaveAnyCycles()
    {
        // Arrange
        var rule = SliceRuleDefinition.Slices().Matching("ArchunitDemo.(*)").Should().BeFreeOfCycles();
        
        // Act
        rule.Check(Architecture);
    }

    [Fact]
    public void DesktopLayerShouldNotReferenceDataLayer()
    {
        // Arrange
        var rule = Types().That().Are(DesktopLayer).Should().NotDependOnAny(DataLayer);
        
        // Act
        rule.Check(Architecture);
    }
    
    [Fact]
    public void DesktopLayerShouldNotReferenceAnotherDesktopLayerAndReverse()
    {
        // Arrange
        var rule1 = Types().That().Are(DesktopLayer).Should().NotDependOnAny(AnotherDesktopLayer);
        var rule2 = Types().That().Are(AnotherDesktopLayer).Should().NotDependOnAny(DesktopLayer);

        // Act
        rule1.Check(Architecture);
        rule2.Check(Architecture);
    }
}