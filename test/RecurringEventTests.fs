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
         Name = "important test year 2"
         Description = "it's a test year 2"
         CreatedAt = now.AddMonths(-3)
         IsImportant = true
         RecurringType = EveryYear (June, Thirteenth) 
         SoftDeleted = false }
       { Id = Guid.Parse("f3b150de-bf64-49dd-8af4-bbac81929237")
         Name = "test week 2 DELETED"
         Description = "it's a test week 2"
         CreatedAt = now
         IsImportant = false
         RecurringType = EveryWeek Tuesday
         SoftDeleted = true } |]


[<Test>]
let ``ToString should return proper stringify version of the RecurringEvent`` () =
    let result = Array.map (fun evt -> evt.ToString()) events

    Assert.AreEqual(4, result.Length)

    Assert.AreEqual("test week 2: it's a test week 2 (Every week on Tuesday)", result[0])
    Assert.AreEqual("test month 2: it's a test month 2 (Every month on Eleventh)", result[1])
    Assert.AreEqual("important test year 2: it's a test year 2 (Every year on Thirteenth of June)", result[2])
    Assert.AreEqual("test week 2 DELETED: it's a test week 2 (Every week on Tuesday)", result[3])

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

    Assert.AreEqual(4, updatedResult.Length)
    Assert.AreEqual(4, notUpdatedResult.Length)

    Assert.AreEqual("UPDATE", updatedResult[0].Name)
    Assert.AreEqual("test month 2", updatedResult[1].Name)
    Assert.AreEqual("important test year 2", updatedResult[2].Name)
    Assert.AreEqual("test week 2 DELETED", updatedResult[3].Name)

    Assert.AreEqual("test week 2", notUpdatedResult[0].Name)
    Assert.AreEqual("test month 2", notUpdatedResult[1].Name)
    Assert.AreEqual("important test year 2", notUpdatedResult[2].Name)
    Assert.AreEqual("test week 2 DELETED", notUpdatedResult[3].Name)

[<Test>]
let ``generateNextEvents with onlyImportants set to false should return the correct Events from the RecurringEvents`` () =
    let maxCount = 5
    let result = RecurringEvent.generateNextEvents false maxCount now events

    // The Events previous today and SoftDeleted should be removed
    Assert.AreEqual(5, result.Length)

    // Ordered by When with the upcoming one on top
    Assert.AreEqual("test week 2", result[0].Name)
    Assert.AreEqual("test month 2", result[1].Name)
    Assert.AreEqual("test week 2", result[2].Name)
    Assert.AreEqual("important test year 2", result[3].Name)
    Assert.AreEqual("test week 2", result[4].Name)

[<Test>]
let ``generateNextEvents with onlyImportants set to true should return the correct Events from the RecurringEvents`` () =
    let maxCount = 5
    let result = RecurringEvent.generateNextEvents true maxCount now events

    // The Events previous today and SoftDeleted and not IsImportant should be removed
    Assert.AreEqual(5, result.Length)

    // Ordered by When with the upcoming one on top
    Assert.AreEqual("important test year 2", result[0].Name)
    Assert.AreEqual("important test year 2", result[1].Name)
    Assert.AreEqual("important test year 2", result[2].Name)
    Assert.AreEqual("important test year 2", result[3].Name)
    Assert.AreEqual("important test year 2", result[4].Name)


[<Test>]
let ``getViewable should return proper RecurringEvents`` () =
    let maxCount = 5
    let result = RecurringEvent.getViewable maxCount events

    // The SoftDeleted should be removed
    Assert.AreEqual(3, result.Length)

    // Ordered by CreatedAt
    Assert.AreEqual("test month 2", result[0].Name)
    Assert.AreEqual("important test year 2", result[1].Name)
    Assert.AreEqual("test week 2", result[2].Name)
