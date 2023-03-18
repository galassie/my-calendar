module RecurringEventTests

open System
open MyCalendar
open NUnit.Framework

let now = DateTime.Parse("2023-6-5")

let events: RecurringEvent array =
    [| { Id = Guid.Parse("6a3abf48-9e1b-4ebb-a912-cbd571797ab1")
         Name = "test week 2"
         Description = "it's a test week 2"
         CreatedAt = now
         IsImportant = false
         RecurringType = EveryWeek Tuesday
         SoftDeleted = false }
       { Id = Guid.Parse("6bd86c63-350b-4210-a767-43a08fc9a6b0")
         Name = "test month 2"
         Description = "it's a test month 2"
         CreatedAt = now.AddYears(-1)
         IsImportant = false
         RecurringType = EveryMonth Eleventh
         SoftDeleted = false }
       { Id = Guid.Parse("c43830d5-35b0-45a8-812a-28d9dc3f7b7d")
         Name = "test year 2"
         Description = "it's a test year 2"
         CreatedAt = now.AddMonths(-3)
         IsImportant = false
         RecurringType = EveryYear (July, Thirteenth) 
         SoftDeleted = false } |]


[<Test>]
let ``ToString should return proper stringify version of the RecurringEvent`` () =
    let result = Array.map (fun evt -> evt.ToString()) events

    Assert.AreEqual(3, result.Length)

    Assert.AreEqual("test week 2: it's a test week 2 (Every week on Tuesday)", result[0])
    Assert.AreEqual("test month 2: it's a test month 2 (Every month on Eleventh)", result[1])
    Assert.AreEqual("test year 2: it's a test year 2 (Every year on Thirteenth of July)", result[2])

[<Test>]
let ``update should return updated version of the RecurringEvent array if Event is found, based on Id, or return the same version`` () =
    let updatableRecurringEvent = 
       { Id = Guid.Parse("6a3abf48-9e1b-4ebb-a912-cbd571797ab1")
         Name = "UPDATE"
         Description = "it's a test week 2"
         CreatedAt = now
         IsImportant = false
         RecurringType = EveryWeek Saturday
         SoftDeleted = false }
    let notUpdatableRecurringEvent = 
      { Id = Guid.Parse("466491f7-cf50-420c-871f-3669d4f8e6ea")
        Name = "TRY TO UPDATE"
        Description = "it's a test 2"
        CreatedAt = now
        IsImportant = false
        RecurringType = EveryWeek Saturday
        SoftDeleted = false }
    
    let updatedResult = RecurringEvent.update updatableRecurringEvent events
    let notUpdatedResult = RecurringEvent.update notUpdatableRecurringEvent events

    Assert.AreEqual(3, updatedResult.Length)
    Assert.AreEqual(3, notUpdatedResult.Length)

    Assert.AreEqual("UPDATE", updatedResult[0].Name)
    Assert.AreEqual("test month 2", updatedResult[1].Name)
    Assert.AreEqual("test year 2", updatedResult[2].Name)

    Assert.AreEqual("test week 2", notUpdatedResult[0].Name)
    Assert.AreEqual("test month 2", updatedResult[1].Name)
    Assert.AreEqual("test year 2", updatedResult[2].Name)