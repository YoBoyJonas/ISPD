namespace Tests;

using NUnit;
using NUnit.Framework;

using Org.Ktu.T120B197.DBs;
using Org.Ktu.T120B197.DBs.Entities;
using System.Diagnostics;


/// <summary>
/// Unit test for the third set of queries.
/// </summary>
[TestFixture][NonParallelizable]
public class Test03
{
	/// <summary>
	/// Constructor.
	/// </summary>
	public Test03()
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
	/// Užklausa apskaičiuoja kiekvieno knygų žanro pardavimų pajamas ir parduotų knygų kiekį.
	/// Jungiamos lentelės Book, OrderItem ir Orders, grupuojama pagal Book.genre.
	/// Filtravimo sąlyga: neįskaičiuojami atšaukti užsakymai (Orders.status != "CANCELLED").
	/// </summary>
	[Test][NonParallelizable]
	public void TestRevenuePerGenre()
	{
		using (var db = new DB())
		{
			var stats =
				(
					from i in db.OrderItem
					join b in db.Book on i.book_id equals b.id
					join o in db.Orders on i.order_id equals o.id
					where o.status != "CANCELLED"
					group i by b.genre into g
					select new { Genre = g.Key, Revenue = g.Sum(x => x.quantity * x.unit_price), Copies = g.Sum(x => x.quantity) }
				).ToList();

			Assert.That(stats, Is.Not.Empty);

			//kiekvienas žanras pasirodo tik vieną kartą
			Assert.That(stats.Select(s => s.Genre), Is.Unique);

			//žanrų pajamų suma lygi visų neatšauktų užsakymų eilučių sumai
			var total = db.OrderItem
				.Where(i => i.Order.status != "CANCELLED")
				.Sum(i => i.quantity * i.unit_price);
			Assert.That(stats.Sum(s => s.Revenue), Is.EqualTo(total));

			//parduotų knygų kiekis sutampa su bendru kiekiu
			var copies = db.OrderItem.Where(i => i.Order.status != "CANCELLED").Sum(i => i.quantity);
			Assert.That(stats.Sum(s => s.Copies), Is.EqualTo(copies));
		}
	}
}
