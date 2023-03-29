module DayInWeekTests

open System
open MyCalendar
open NUnit.Framework

[<Test>]
let ``ToDayOfWeek should return proper DayOfWeek`` () =
    Assert.AreEqual(DayOfWeek.Sunday, DayInWeek.ToDayOfWeek DayInWeek.Sunday)
    Assert.AreEqual(DayOfWeek.Monday, DayInWeek.ToDayOfWeek DayInWeek.Monday)
    Assert.AreEqual(DayOfWeek.Tuesday, DayInWeek.ToDayOfWeek DayInWeek.Tuesday)
    Assert.AreEqual(DayOfWeek.Wednesday, DayInWeek.ToDayOfWeek DayInWeek.Wednesday)
    Assert.AreEqual(DayOfWeek.Thursday, DayInWeek.ToDayOfWeek DayInWeek.Thursday)
    Assert.AreEqual(DayOfWeek.Friday, DayInWeek.ToDayOfWeek DayInWeek.Friday)
    Assert.AreEqual(DayOfWeek.Saturday, DayInWeek.ToDayOfWeek DayInWeek.Saturday)
