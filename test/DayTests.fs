module DayTests

open MyCalendar
open NUnit.Framework

[<Test>]
let ``ToDayInt should return proper int value`` () =
    Assert.AreEqual(1, Day.ToDayInt Day.First)
    Assert.AreEqual(2, Day.ToDayInt Day.Second)
    Assert.AreEqual(3, Day.ToDayInt Day.Third)
    Assert.AreEqual(4, Day.ToDayInt Day.Fourth)
    Assert.AreEqual(5, Day.ToDayInt Day.Fifth)
    Assert.AreEqual(6, Day.ToDayInt Day.Sixth)
    Assert.AreEqual(7, Day.ToDayInt Day.Seventh)
    Assert.AreEqual(8, Day.ToDayInt Day.Eighth)
    Assert.AreEqual(9, Day.ToDayInt Day.Ninth)
    Assert.AreEqual(10, Day.ToDayInt Day.Tenth)
    Assert.AreEqual(11, Day.ToDayInt Day.Eleventh)
    Assert.AreEqual(12, Day.ToDayInt Day.Twelfth)
    Assert.AreEqual(13, Day.ToDayInt Day.Thirteenth)
    Assert.AreEqual(14, Day.ToDayInt Day.Fourteenth)
    Assert.AreEqual(15, Day.ToDayInt Day.Fifteenth)
    Assert.AreEqual(16, Day.ToDayInt Day.Sixteenth)
    Assert.AreEqual(17, Day.ToDayInt Day.Seventeenth)
    Assert.AreEqual(18, Day.ToDayInt Day.Eighteenth)
    Assert.AreEqual(19, Day.ToDayInt Day.Nineteenth)
    Assert.AreEqual(20, Day.ToDayInt Day.Twentieth)
    Assert.AreEqual(21, Day.ToDayInt Day.Twentyfirst)
    Assert.AreEqual(22, Day.ToDayInt Day.Twentysecond)
    Assert.AreEqual(23, Day.ToDayInt Day.Twentythird)
    Assert.AreEqual(24, Day.ToDayInt Day.Twentyfourth)
    Assert.AreEqual(25, Day.ToDayInt Day.Twentyfifth)
    Assert.AreEqual(26, Day.ToDayInt Day.Twentysixth)
    Assert.AreEqual(27, Day.ToDayInt Day.Twentyseventh)
    Assert.AreEqual(28, Day.ToDayInt Day.Twentyeighth)
    Assert.AreEqual(29, Day.ToDayInt Day.Twentyninth)
    Assert.AreEqual(30, Day.ToDayInt Day.Thirtieth)
    Assert.AreEqual(31, Day.ToDayInt Day.Thirtyfirst)