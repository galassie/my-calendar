namespace MyCalendar

open System
open Spectre.Console
open FsSpectre

type Handlers =

    static member handle (args: Arguments) =
        let now = DateTime.Now

        match args with
        | Show -> 
            calendar {
                year now.Year
                month now.Month
                day now.Day
            }
            |> AnsiConsole.Write 

            AnsiConsole.WriteLine $"{now.Day} {now.DayOfWeek}"