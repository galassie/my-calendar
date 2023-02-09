namespace MyCalendar

open System
open System.IO
open FSharp.Json

[<RequireQualifiedAccess>]
module Storage =

    let retrieve (date: DateTime) =
        let year = date.Year
        let path = Path.Combine(Path.GetTempPath(), $"mycalendar-{year}.json")

        let data =
                if (File.Exists(path)) then
                    File.ReadAllText(path)
                    |> Json.deserialize<MyCalendarData>
                else
                    MyCalendarData.Default

        data