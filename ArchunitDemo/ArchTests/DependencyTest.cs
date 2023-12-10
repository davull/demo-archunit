using ArchunitDemo.AnotherDesktopModule;
using ArchunitDemo.BusinessModule;
using ArchunitDemo.DataModule;
using ArchunitDemo.DesktopModule;
using ArchUnitNET.Domain;
using ArchUnitNET.Fluent.Slices;
using ArchUnitNET.Loader;
using ArchUnitNET.xUnit;
using Microsoft.Data.SqlClient;
using Xunit;

namespace ArchTests;

using static ArchUnitNET.Fluent.ArchRuleDefinition;

public class DependencyTest
{
    private static readonly System.Reflection.Assembly DataModuleAssembly = typeof(DataModuleMarker).Assembly;
    private static readonly System.Reflection.Assembly BusinessModuleAssembly = typeof(BusinessModuleMarker).Assembly;
    private static readonly System.Reflection.Assembly DesktopModuleAssembly = typeof(DesktopModuleMarker).Assembly;

    private static readonly System.Reflection.Assembly AnotherDesktopModuleAssembly =
        typeof(AnotherDesktopModuleMarker).Assembly;

    private static readonly System.Reflection.Assembly[] ProjectAssemblies =
    {
        DataModuleAssembly,
        BusinessModuleAssembly,
        DesktopModuleAssembly,
        AnotherDesktopModuleAssembly
    };

    private static readonly System.Reflection.Assembly SystemAssembly =
        typeof(DateTime).Assembly;

    private static readonly System.Reflection.Assembly SystemDataAssembly =
        typeof(SqlConnection).Assembly;

    private static readonly Architecture Architecture = new ArchLoader()
        .LoadAssemblies(
            DataModuleAssembly,
            BusinessModuleAssembly,
            DesktopModuleAssembly,
            AnotherDesktopModuleAssembly,
            SystemAssembly,
            SystemDataAssembly)
        .Build();

    // Define layers
    private readonly IObjectProvider<IType> DataLayer =
        Types().That().ResideInAssembly(DataModuleAssembly).As("Data layer");

    private readonly IObjectProvider<IType> BusinessLayer =
        Types().That().ResideInAssembly(BusinessModuleAssembly).As("Business layer");

    private readonly IObjectProvider<IType> DesktopLayer =
        Types().That().ResideInAssembly(DesktopModuleAssembly).As("Desktop layer");

    private readonly IObjectProvider<IType> AnotherDesktopLayer =
        Types().That().ResideInAssembly(AnotherDesktopModuleAssembly).As("Another Desktop layer");

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

    [Fact]
    public void DateTimeNowShouldNotBeUsedInBusinessLayer()
    {
        var types = Types().That().Are(BusinessLayer);

        var methodsNotToCall = MethodMembers()
            .That()
            .AreDeclaredIn(typeof(DateTime))
            .And().AreStatic()
            .And().HaveNameEndingWith("get_UtcNow()")
            .Or().HaveNameEndingWith("get_Now()");

        var rule = types.Should().NotCallAny(methodsNotToCall);
        rule.Check(Architecture);
    }

    [Fact]
    public void OnlyDataLayerShouldUseDatabase()
    {
        var types = Types()
            .That().ResideInAssembly(ProjectAssemblies[0], ProjectAssemblies[1..])
            .And()
            .AreNot(DataLayer);
        var typesNotToUse = Types().That().ResideInAssembly(SystemDataAssembly);

        var rule = types.Should().NotDependOnAny(typesNotToUse);
        rule.Check(Architecture);
    }
}