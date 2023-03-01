namespace MyCalendar

open Argu

type ToDoArguments =
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

and EventArguments =
    | [<CliPrefix(CliPrefix.None)>] Add
    | [<CliPrefix(CliPrefix.None)>] Edit
    | [<CliPrefix(CliPrefix.None)>] Delete

    interface IArgParserTemplate with
        member s.Usage =
            match s with
            | Add -> "add an Event to your list"
            | Edit -> "edit an Event from your list"
            | Delete -> "remove an Event from your list"

and RecurringEventArguments =
    | [<CliPrefix(CliPrefix.None)>] Add
    | [<CliPrefix(CliPrefix.None)>] Edit
    | [<CliPrefix(CliPrefix.None)>] Delete

    interface IArgParserTemplate with
        member s.Usage =
            match s with
            | Add -> "add a RecurringEvent to your list"
            | Edit -> "edit a RecurringEvent from your list"
            | Delete -> "remove a RecurringEvent from your list"


and Arguments =
    | [<CliPrefix(CliPrefix.None)>] Show
    | [<CliPrefix(CliPrefix.None)>] ToDo of ParseResults<ToDoArguments>
    | [<CliPrefix(CliPrefix.None)>] Event of ParseResults<EventArguments>
    | [<CliPrefix(CliPrefix.None)>] RecurringEvent of ParseResults<RecurringEventArguments>

    interface IArgParserTemplate with
        member s.Usage =
            match s with
            | Show -> "show the main view of your calendar"
            | ToDo _ -> "allow to perform operations on your ToDos"
            | Event _ -> "allow to perform operations on your Events"
            | RecurringEvent _ -> "allow to perform operations on your RecurringEvents"
