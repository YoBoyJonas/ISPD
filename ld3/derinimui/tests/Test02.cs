namespace Tests;

using NUnit;
using NUnit.Framework;

using Org.Ktu.T120B197.DBs;
using Org.Ktu.T120B197.DBs.Entities;
using System.Diagnostics;


/// <summary>
/// Unit test for the second set of queries.
/// </summary>
[TestFixture][NonParallelizable]
public class Test02
{
	/// <summary>
	/// Constructor.
	/// </summary>
	public Test02()
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
	/// Užklausa grąžina 10 daugiausiai pinigų išleidusių Vilniaus klientų (atmetus atšauktus užsakymus).
	/// Jungiamos lentelės Customer, Orders ir OrderItem; duomenys grupuojami pagal klientą,
	/// sumuojama quantity * unit_price ir rikiuojama mažėjančia tvarka.
	/// Filtravimo sąlygos: Customer.city = "Vilnius" ir Orders.status != "CANCELLED".
	/// </summary>
	[Test][NonParallelizable]
	public void TestTopSpendingCustomersInVilnius()
	{
		using (var db = new DB())
		{
			var top =
				(
					from c in db.Customer
					join o in db.Orders on c.id equals o.customer_id
					join i in db.OrderItem on o.id equals i.order_id
					where c.city == "Vilnius" && o.status != "CANCELLED"
					group i by new { c.id, c.name } into g
					select new { g.Key.id, g.Key.name, Total = g.Sum(x => x.quantity * x.unit_price) }
				)
				.OrderByDescending(x => x.Total)
				.Take(10)
				.ToList();

			Assert.That(top, Is.Not.Empty);
			Assert.That(top.Count, Is.LessThanOrEqualTo(10));

			//sumos surikiuotos mažėjimo tvarka ir teigiamos
			Assert.That(top.Select(x => x.Total), Is.Ordered.Descending);
			Assert.That(top.All(x => x.Total > 0), Is.True);

			//didžiausią sumą turinčio kliento suma perskaičiuojama kita užklausa
			var best = top[0];
			var recomputed = db.OrderItem
				.Where(i => i.Order.customer_id == best.id && i.Order.status != "CANCELLED")
				.Sum(i => i.quantity * i.unit_price);
			Assert.That(recomputed, Is.EqualTo(best.Total));
			Assert.That(db.Customer.Find(best.id).city, Is.EqualTo("Vilnius"));
		}
	}
}
