namespace MyCalendar

open System
open FsSpectre
open Spectre.Console.Rendering

[<RequireQualifiedAccess>]
module Views =

    let calendar (now: DateTime) =
        grid {
            number_of_columns 1
            row [| calendar { events [| calendarEvent { date_time now } |] } |]

            row
                [| text {
                       text $"{now.Day} {now.DayOfWeek}"
                       centered
                   } |]
        }

    let todo (todos: ToDo array) (now: DateTime) =
        let toShow: IRenderable array =
            todos
            |> Array.filter (fun todo -> not todo.SoftDeleted)
            |> Array.filter (fun todo ->
                todo.MarkedDoneAt
                |> Option.map (fun dt -> (now - dt).Days < 5)
                |> Option.defaultValue true)
            |> Array.map (fun todo ->
                if Option.isSome todo.MarkedDoneAt then
                    $"[strikethrough]{todo.Name}[/]"
                else
                    todo.Name)
            |> Array.map (fun txt -> text { text txt })

        panel {
            expand
            width 32
            height 32
            header_text "Todo"
            content_renderable (rows { items_renderable toShow })
        }

    let nextEvents (recurringEvents: RecurringEvent array) (events: Event array) (now: DateTime) =
        panel {
            expand
            width 32
            height 32
            header_text "Events of the month"
            content_renderable (rows { items_renderable [||] })
        }
