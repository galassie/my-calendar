namespace MyCalendar

open Argu

type ToDoSubArguments =
    | [<CliPrefix(CliPrefix.None)>] Add
    | [<CliPrefix(CliPrefix.None)>] Done

    interface IArgParserTemplate with
        member s.Usage =
            match s with
            | Add -> "add a ToDo to your list"
            | Done -> "mark done a ToDo"


type Arguments =
    | [<CliPrefix(CliPrefix.None)>] Show
    | [<CliPrefix(CliPrefix.None)>] ToDo of ParseResults<ToDoSubArguments>

    interface IArgParserTemplate with
        member s.Usage =
            match s with
            | Show -> "show the main view of your calendar"
            | ToDo _ -> "allow to perform operations on yout ToDos"
