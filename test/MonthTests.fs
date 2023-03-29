module MonthTests

open MyCalendar
open NUnit.Framework

[<Test>]
let ``ToMonthInt should return proper int value`` () =
    Assert.AreEqual(1, Month.ToMonthInt Month.January)
    Assert.AreEqual(2, Month.ToMonthInt Month.February)
    Assert.AreEqual(3, Month.ToMonthInt Month.March)
    Assert.AreEqual(4, Month.ToMonthInt Month.April)
    Assert.AreEqual(5, Month.ToMonthInt Month.May)
    Assert.AreEqual(6, Month.ToMonthInt Month.June)
    Assert.AreEqual(7, Month.ToMonthInt Month.July)
    Assert.AreEqual(8, Month.ToMonthInt Month.August)
    Assert.AreEqual(9, Month.ToMonthInt Month.September)
    Assert.AreEqual(10, Month.ToMonthInt Month.October)
    Assert.AreEqual(11, Month.ToMonthInt Month.November)
    Assert.AreEqual(12, Month.ToMonthInt Month.December)
