namespace MyCalendar

open Argu

type Arguments =
    | [<CliPrefix(CliPrefix.None)>] Show

    interface IArgParserTemplate with
        member s.Usage =
            match s with
            | Show -> "show the main view of your calendar"
