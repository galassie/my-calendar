namespace MyCalendar

open Argu

type ToDoSubArguments =
    | [<CliPrefix(CliPrefix.None)>] Add
    | [<CliPrefix(CliPrefix.None)>] Edit
    | [<CliPrefix(CliPrefix.None)>] Done
    | [<CliPrefix(CliPrefix.None)>] Undone
    | [<CliPrefix(CliPrefix.None)>] Delete

    interface IArgParserTemplate with
        member s.Usage =
            match s with
            | Add -> "add a ToDo to your list"
            | Edit -> "edit a ToDo from your list"
            | Done -> "mark done a ToDo"
            | Undone -> "undone a ToDo marked done"
            | Delete -> "remove a ToDo from your list"

and EventSubArguments =
    | [<CliPrefix(CliPrefix.None)>] Add
    | [<CliPrefix(CliPrefix.None)>] Edit
    | [<CliPrefix(CliPrefix.None)>] Delete

    interface IArgParserTemplate with
        member s.Usage =
            match s with
            | Add -> "add an Event to your list"
            | Edit -> "edit an Event from your list"
            | Delete -> "remove an Event from your list"


and Arguments =
    | [<CliPrefix(CliPrefix.None)>] Show
    | [<CliPrefix(CliPrefix.None)>] ToDo of ParseResults<ToDoSubArguments>
    | [<CliPrefix(CliPrefix.None)>] Event of ParseResults<EventSubArguments>

    interface IArgParserTemplate with
        member s.Usage =
            match s with
            | Show -> "show the main view of your calendar"
            | ToDo _ -> "allow to perform operations on your ToDos"
            | Event _ -> "allow to perform operations on your Events"
