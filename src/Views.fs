namespace MyCalendar

open System
open FsSpectre
open Spectre.Console
open Spectre.Console.Rendering

[<RequireQualifiedAccess>]
module Views =

    let calendarView (now: DateTime) =
        grid {
            number_of_columns 1

            row
                [| calendar {
                       events [| calendarEvent { date_time now } |]
                       highlight_style (style { foreground Color.Green })
                       header_style (style { foreground Color.Blue })
                   } |]

            row
                [| markup {
                       text $"[blue]{now.Day} {now.DayOfWeek}[/]"
                       centered
                   } |]
        }

    let todoView (todos: ToDo array) =
        let toShow: IRenderable array =
            todos
            |> Array.map (fun todo ->
                if Option.isSome todo.MarkedDoneAt then
                    markup { text $"[strikethrough grey]➢ {todo}[/]" }
                else
                    markup { text $"➢ {todo}" })

        panel {
            expand
            width 32
            height 32
            header_text "[bold]Todo[/]"
            border_color Color.Blue
            box_border BoxBorder.Rounded
            content_renderable (rows { items_renderable toShow })
        }

    let private todayEvent (now: DateTime) (event: Event) =
        now.Day = event.When.Day
        && now.Month = event.When.Month
        && now.Year = event.When.Year

    let eventsView (headerText: string) (now: DateTime) (events: Event array) =
        let toShow: IRenderable array =
            events
            |> Array.map (fun event ->
                let isImportant = if event.IsImportant then " [red]⚠[/]" else ""
                // Events generated from RecurringEvents have an empty Guid as Id
                let isFromRecurring = if event.Id = Guid.Empty then " [yellow]⟳[/]" else ""

                if todayEvent now event then
                    markup { text $"➢ [green]{event}[/]{isFromRecurring}{isImportant}" }
                else
                    markup { text $"➢ {event}{isFromRecurring}{isImportant}" })

        panel {
            expand
            width 32
            height 16
            header_text $"[bold]{headerText}[/]"
            border_color Color.Blue
            box_border BoxBorder.Rounded
            content_renderable (rows { items_renderable toShow })
        }

    let mainView (now: DateTime) (data: MyCalendarData) =
        let todos = ToDo.getViewable now data.ToDos

        let nextEvents = Event.nextEvents false Constants.maxCount now data.Events

        let nextRecurringEvents =
            RecurringEvent.generateNextEvents false Constants.maxCount now data.RecurringEvents

        let events =
            [| nextEvents; nextRecurringEvents |]
            |> Array.reduce Array.append
            |> Array.sortBy (fun evt -> evt.When)
            |> Array.truncate Constants.maxCount

        let nextImportantEvents = Event.nextEvents true Constants.maxCount now data.Events

        let nextImportantRecurringEvents =
            RecurringEvent.generateNextEvents true Constants.maxCount now data.RecurringEvents

        let importantEvents =
            [| nextImportantEvents; nextImportantRecurringEvents |]
            |> Array.reduce Array.append
            |> Array.sortBy (fun evt -> evt.When)
            |> Array.truncate Constants.maxCount

        grid {
            number_of_columns 3
            empty_row

            row
                [| calendarView now
                   grid {
                       number_of_columns 1
                       row [| eventsView "Next events" now events |]
                       row [| eventsView "Next important events!" now importantEvents |]
                   }
                   todoView todos |]

            empty_row
        }
