namespace Tests;

using NUnit;
using NUnit.Framework;

using Org.Ktu.T120B197.DBs;
using Org.Ktu.T120B197.DBs.Entities;
using System.Diagnostics;


/// <summary>
/// Unit test for the fifth set of queries.
/// </summary>
[TestFixture][NonParallelizable]
public class Test05
{
	/// <summary>
	/// Constructor.
	/// </summary>
	public Test05()
	{
		//set DB logger
		DB.Logger =
			(msg) =>
			{
				//this will show in console when running with "dotnet test -v n"
				TestContext.Out.WriteLine(msg);

				//this will show in debug console when running test via debugger
				Debug.WriteLine(msg);
			};
	}

	/// <summary>
	/// Test for query/queries.
	///
	/// Užklausa grąžina Kauno klientų pirktas knygas, kurių kaina didesnė nei jų žanro vidutinė kaina.
	/// Jungiamos lentelės OrderItem, Orders, Customer ir Book; vidutinė žanro kaina skaičiuojama
	/// koreliuotame subselect (Book lentelė). Filtravimo sąlygos: Customer.city = "Kaunas"
	/// ir Book.price > žanro vidutinė kaina.
	/// </summary>
	[Test][NonParallelizable]
	public void TestKaunasPurchasesAboveGenreAverage()
	{
		using (var db = new DB())
		{
			var rows =
				(
					from i in db.OrderItem
					where i.Order.Customer.city == "Kaunas"
						&& i.Book.price > db.Book.Where(b => b.genre == i.Book.genre).Average(b => b.price)
					select new { Customer = i.Order.Customer.name, i.Book.title, i.Book.genre, i.Book.price }
				).ToList();

			Assert.That(rows, Is.Not.Empty);

			//žanrų vidutinės kainos skaičiuojamos atskira užklausa
			var avg = db.Book
				.GroupBy(b => b.genre)
				.Select(g => new { Genre = g.Key, Avg = g.Average(b => b.price) })
				.ToDictionary(x => x.Genre, x => x.Avg);

			Assert.That(rows.All(r => r.price > avg[r.genre]), Is.True);

			//rezultatų negali būti daugiau nei visų Kauno klientų užsakymų eilučių
			var kaunasCount = db.OrderItem.Count(i => i.Order.Customer.city == "Kaunas");
			Assert.That(rows.Count, Is.LessThanOrEqualTo(kaunasCount));
		}
	}
}
