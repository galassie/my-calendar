module RecurringTypeTests

open MyCalendar
open NUnit.Framework

[<Test>]
let ``ToString should return proper string version`` () =
    [| (EveryWeek Sunday, "Every week on Sunday")
       (EveryMonth Fifteenth, "Every month on Fifteenth")
       (EveryYear(January, Eighteenth), "Every year on Eighteenth of January") |]
    |> Array.iter (fun (input, expected) ->
        let actual = input.ToString()
        Assert.AreEqual(expected, actual))
