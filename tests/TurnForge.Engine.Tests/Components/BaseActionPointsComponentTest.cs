namespace TurnForge.Engine.Tests.Components;

using NUnit.Framework;
using TurnForge.Engine.Components;

[TestFixture]
public class BaseActionPointsComponentTests
{
    [Test]
    public void Constructor_InitializesCorrectly() { 
        var component = new BaseActionPointsComponent(10);
        Assert.That(component.CurrentActionPoints, Is.EqualTo(10));
        Assert.That(component.MaxActionPoints, Is.EqualTo(10));
    }
    
    [Test]
    public void SpendActionPoints_DeductsCorrectly() { 
        var component = new BaseActionPointsComponent(10);
        component.SpendActionPoints(5);
        Assert.That(component.CurrentActionPoints, Is.EqualTo(5));
    }
    
    [Test]
    public void SpendActionPoints_CannotGoNegative() { 
        var component = new BaseActionPointsComponent(10);
        Assert.Throws<InvalidOperationException>(() => component.SpendActionPoints(15));
    }
    
    [Test]
    public void CanAfford_ReturnsTrueWhenSufficient() { 
        var component = new BaseActionPointsComponent(10);
        Assert.Multiple(() =>
        {
            Assert.That(component.CanAfford(5), Is.True);
            Assert.That(component.CanAfford(10), Is.True);
            
        });

    }
    
    [Test]
    public void CanAfford_ReturnsFalseWhenInsufficient() {
        var component = new BaseActionPointsComponent(10);
        Assert.Multiple(() =>
        {
            Assert.That(component.CanAfford(15), Is.False);
            Assert.That(component.CanAfford(20), Is.False);
        });
     }
    
    [Test]
    public void RestoreActionPoints_IncreasesCorrectly() {
        var component = new BaseActionPointsComponent(10);
        component.SpendActionPoints(5);
        component.RestoreActionPoints(3);
        Assert.That(component.CurrentActionPoints, Is.EqualTo(8));
     }
    
    [Test]
    public void RestoreActionPoints_ClampsToMax() {
        var component = new BaseActionPointsComponent(10);
        component.SpendActionPoints(5);
        component.RestoreActionPoints(15);
        Assert.That(component.CurrentActionPoints, Is.EqualTo(10));
     }
    
    [Test]
    public void ResetActionPoints_RestoresToMax() {
        var component = new BaseActionPointsComponent(10);
        component.SpendActionPoints(5);
        component.ResetActionPoints();
        Assert.That(component.CurrentActionPoints, Is.EqualTo(10));
     }
    
    [Test]
    public void ResetActionPoints_RestoresToMaxWithAmount() {
        var component = new BaseActionPointsComponent(10);
        component.SpendActionPoints(5);
        component.ResetActionPoints(15);
        Assert.That(component.CurrentActionPoints, Is.EqualTo(15));
     }
    
    [Test]
    public void IsEmpty_ReturnsTrueWhenZero() {
        var component = new BaseActionPointsComponent(0);
        Assert.That(component.IsEmpty(), Is.True);
     }
}