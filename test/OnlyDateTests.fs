module OnlyDate

open MyCalendar
open NUnit.Framework

[<Test>]
let ``tryParse should return proper OnlyDate if the date is valid, None otherwise`` () =
    [| ("2023-2-28", Some { Year = 2023; Month = 2; Day = 28 })
       ("2023-02-28", Some { Year = 2023; Month = 2; Day = 28 })
       ("2023-02-29", None)
       ("2020-02-29", Some { Year = 2020; Month = 2; Day = 29 }) |]
    |> Array.iter (fun (input, expected) ->
        let actual = OnlyDate.TryParse(input)
        Assert.AreEqual(expected, actual))

[<Test>]
let ``compare should compare properly`` () =
    [| ({ Year = 2023; Month = 2; Day = 28 }, { Year = 2023; Month = 2; Day = 27 }, 1)
       ({ Year = 2023; Month = 2; Day = 27 }, { Year = 2023; Month = 2; Day = 28 }, -1)
       ({ Year = 2023; Month = 1; Day = 27 }, { Year = 2023; Month = 2; Day = 1 }, -1)
       ({ Year = 2022; Month = 2; Day = 28 }, { Year = 2023; Month = 2; Day = 27 }, -1)
       ({ Year = 2023; Month = 2; Day = 27 }, { Year = 2023; Month = 2; Day = 27 }, 0) |]
    |> Array.iter (fun (e1, e2, expected) ->
        let actual = compare e1 e2
        Assert.AreEqual(expected, actual))
