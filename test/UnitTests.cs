namespace Tests;

[TestClass]
public class UnitTests
{
  [TestMethod]
  public void TestDiagram()
  {
    var umbrella = new D2Shape("alphabet", "Alphabet Inc", Shape.Rectangle);
    var company = new D2Shape("google", "Google", Shape.Rectangle);

    company.Add(new D2Shape("gmail", "Gmail", Shape.Rectangle));
    company.Add(new D2Shape("meet", "Meet", Shape.Rectangle));
    company.Add(new D2Shape("deepmind", "DeepMind", Shape.Rectangle));

    var connection = new D2Connection(company.Name, umbrella.Name, Direction.TO, "BELONGS_TO");

    var diagram = new D2Diagram(new[] { umbrella, company }, new[] { connection });
    var actual = diagram.ToString();

    Assert.AreEqual("expected", actual);
  }
}