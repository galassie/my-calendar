namespace MyCalendar

open System
open System.IO
open FSharp.Json

[<RequireQualifiedAccess>]
module Storage =

    let retrieve () =
        let path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, $"my-calendar-store.json")

        let data =
            if (File.Exists(path)) then
                File.ReadAllText(path) |> Json.deserialize<MyCalendarData>
            else
                MyCalendarData.Default

        data

    let store (data: MyCalendarData) =
        try
            let path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, $"my-calendar-store.json")

            let str = Json.serialize data
            File.WriteAllText(path, str)
        with ex ->
            printfn "%A" (ex.ToString())
