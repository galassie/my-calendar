namespace MyCalendar

open System
open Argu

module Program =

    [<EntryPoint>]
    let main args =
        let errorHandler = ProcessExiter(colorizer = function ErrorCode.HelpText -> None | _ -> Some ConsoleColor.Red)
        let parser = ArgumentParser.Create<Arguments>(programName = "mycalendar", errorHandler = errorHandler)

        let results = parser.ParseCommandLine args

        results.GetAllResults() |> List.map Handlers.Handle |> ignore

        0
