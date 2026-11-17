namespace Tests;

using NUnit;
using NUnit.Framework;

using Org.Ktu.T120B197.DBs;
using Org.Ktu.T120B197.DBs.Entities;
using System.Diagnostics;


/// <summary>
/// Unit test for the fourth set of queries.
/// </summary>
[TestFixture][NonParallelizable]
public class Test04
{
	/// <summary>
	/// Constructor.
	/// </summary>
	public Test04()
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
	/// Užklausa grąžina Estijos autorius, kurie turi bent vieną knygą, bet nė viena jų niekada nebuvo užsakyta.
	/// Naudojamos lentelės Author, Book ir OrderItem (NOT EXISTS tipo sąlyga per !Any).
	/// Filtravimo sąlygos: Author.country = "Estonia", autorius turi knygų, ir nė viena knyga neturi užsakymo eilučių.
	/// </summary>
	[Test][NonParallelizable]
	public void TestEstonianAuthorsWithoutOrderedBooks()
	{
		using (var db = new DB())
		{
			var authors = db.Author
				.Where(a => a.country == "Estonia"
					&& a.Book.Any()
					&& !a.Book.Any(b => b.OrderItem.Any()))
				.ToList();

			Assert.That(authors, Is.Not.Empty);
			Assert.That(authors.All(a => a.country == "Estonia"), Is.True);

			//kita užklausa: šių autorių knygų užsakymų eilučių neturi būti
			var ids = authors.Select(a => a.id).ToList();
			var orderedCount = db.OrderItem.Count(i => ids.Contains(i.Book.author_id));
			Assert.That(orderedCount, Is.EqualTo(0));

			//ir kiekvienas jų turi bent vieną knygą
			var withBooks = db.Book.Where(b => ids.Contains(b.author_id)).Select(b => b.author_id).Distinct().Count();
			Assert.That(withBooks, Is.EqualTo(ids.Count));
		}
	}
}
