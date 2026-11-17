namespace Tests;

using NUnit;
using NUnit.Framework;

using Org.Ktu.T120B197.DBs;
using Org.Ktu.T120B197.DBs.Entities;
using System.Diagnostics;


/// <summary>
/// Unit test for the first set of queries.
/// </summary>
[TestFixture][NonParallelizable]
public class Test01
{
	/// <summary>
	/// Constructor.
	/// </summary>
	public Test01()
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
	/// Užklausa grąžina visas užsakymų eilutes, kuriose parduota knyga, parašyta Lietuvos autoriaus,
	/// o užsakymas yra apmokėtas (status = PAID). Užklausa jungia 4 lenteles:
	/// OrderItem, Book, Author ir Orders. Filtravimo sąlygos: Author.country = "Lithuania"
	/// ir Orders.status = "PAID".
	/// </summary>
	[Test][NonParallelizable]
	public void TestPaidOrderItemsOfLithuanianAuthors()
	{
		using (var db = new DB())
		{
			var rows =
				(
					from i in db.OrderItem
					join b in db.Book on i.book_id equals b.id
					join a in db.Author on b.author_id equals a.id
					join o in db.Orders on i.order_id equals o.id
					where a.country == "Lithuania" && o.status == "PAID"
					select new { ItemId = i.id, b.title, AuthorCountry = a.country, o.status }
				).ToList();

			//rezultatas negali būti tuščias
			Assert.That(rows, Is.Not.Empty);

			//visi įrašai atitinka filtrus
			Assert.That(rows.All(r => r.AuthorCountry == "Lithuania" && r.status == "PAID"), Is.True);

			//rezultatų skaičius sutampa su ta pačia užklausa, užrašyta per navigacines savybes
			var expectedCount = db.OrderItem.Count(i => i.Book.Author.country == "Lithuania" && i.Order.status == "PAID");
			Assert.That(rows.Count, Is.EqualTo(expectedCount));
		}
	}
}
