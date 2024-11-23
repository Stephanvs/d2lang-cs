namespace Tests;

[TestClass]
public class UnitTests
{
  [TestMethod]
  public void TestDiagram()
  {
    var umbrella = new D2Shape("alphabet", "Alphabet Inc", Shape.Rectangle);
    var company = new D2Shape("google", "Google", Shape.Rectangle)
	{
		new D2Shape("gmail", "Gmail", Shape.Rectangle),
		new D2Shape("meet", "Meet", Shape.Rectangle),
		new D2Shape("deepmind", "DeepMind", Shape.Rectangle),
	};

    var connection = new D2Connection(company.Name, umbrella.Name, Direction.TO, "BELONGS_TO");

    var diagram = new D2Diagram(new[] { umbrella, company }, new[] { connection });
    var actual = diagram.ToString();
    var expected = @"alphabet: Alphabet Inc {
  shape: rectangle
}
google: Google {
  gmail: Gmail {
    shape: rectangle
  }
  meet: Meet {
    shape: rectangle
  }
  deepmind: DeepMind {
    shape: rectangle
  }
  shape: rectangle
}
google -> alphabet: BELONGS_TO";

    Assert.AreEqual(expected, actual);
  }
}
