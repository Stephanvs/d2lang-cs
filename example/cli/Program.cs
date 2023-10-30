using d2;

var umbrella = new D2Shape("alphabet", "Alphabet Inc", Shape.Rectangle);
var company = new D2Shape("google", null, Shape.Rectangle);

company.Add(new D2Shape("gmail", "Gmail", Shape.Rectangle));
company.Add(new D2Shape("meet", "Meet", Shape.Rectangle));
company.Add(new D2Shape("deepmind", "DeepMind", Shape.Rectangle));

company.Icon = "https://www.google.com/images/branding/googlelogo/2x/googlelogo_color_92x30dp.png";

var connection = new D2Connection(company.Name, umbrella.Name, Direction.TO, "BELONGS_TO");

var diagram = new D2Diagram(new[] { umbrella, company }, new[] { connection });

Console.WriteLine(diagram.ToString());