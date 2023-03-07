module EventTests

open System
open MyCalendar
open NUnit.Framework

let now = DateTime.Parse("2023-6-5")

let events: Event array =
    [| { Id = Guid.Parse("6a3abf48-9e1b-4ebb-a912-cbd571797ab1")
         Name = "test 2"
         Description = "it's a test 2"
         CreatedAt = now
         IsImportant = false
         When =
           { Year = now.Year
             Month = now.Month
             Day = now.Day }
         SoftDeleted = false }
       { Id = Guid.Parse("6bd86c63-350b-4210-a767-43a08fc9a6b0")
         Name = "very old 1"
         Description = "very old 1"
         CreatedAt = now.AddYears(-1)
         IsImportant = false
         When =
           { Year = now.Year - 1
             Month = now.Month
             Day = now.Day }
         SoftDeleted = false }
       { Id = Guid.Parse("c43830d5-35b0-45a8-812a-28d9dc3f7b7d")
         Name = "old 1"
         Description = "old 1"
         CreatedAt = now.AddMonths(-3)
         IsImportant = false
         When =
           { Year = now.Year
             Month = now.Month - 3
             Day = now.Day }
         SoftDeleted = false }
       { Id = Guid.Parse("137a7ba3-9331-4501-a15b-73bb0c9ee38d")
         Name = "deleted 1"
         Description = "deleted 1"
         CreatedAt = now
         IsImportant = false
         When =
           { Year = now.Year
             Month = now.Month
             Day = now.Day }
         SoftDeleted = true }
       { Id = Guid.Parse("46918e99-2704-411a-b2c0-eee80bf060ab")
         Name = "not so old 2"
         Description = "not so old 2"
         CreatedAt = now.AddDays(-2)
         IsImportant = false
         When =
           { Year = now.Year
             Month = now.Month
             Day = now.Day - 2 }
         SoftDeleted = false }
       { Id = Guid.Parse("d86ebde1-c3ab-4d34-a70d-7effd12a2988")
         Name = "important next year 1"
         Description = "important next year 1"
         CreatedAt = now.AddDays(-1)
         IsImportant = true
         When =
           { Year = now.Year + 1
             Month = now.Month
             Day = now.Day }
         SoftDeleted = false } |]


[<Test>]
let ``ToString should return proper stringify version of the Event`` () =
    let result = Array.map (fun evt -> evt.ToString()) events

    Assert.AreEqual(6, result.Length)

    Assert.AreEqual("test 2: it's a test 2 (2023-06-05)", result[0])
    Assert.AreEqual("very old 1: very old 1 (2022-06-05)", result[1])
    Assert.AreEqual("old 1: old 1 (2023-03-05)", result[2])
    Assert.AreEqual("deleted 1: deleted 1 (2023-06-05)", result[3])
    Assert.AreEqual("not so old 2: not so old 2 (2023-06-03)", result[4])
    Assert.AreEqual("important next year 1: important next year 1 (2024-06-05)", result[5])

[<Test>]
let ``update should return updated version of the Event array if Event is found, based on Id, or return the same version`` () =
    let updatableEvent = 
      { Id = Guid.Parse("6a3abf48-9e1b-4ebb-a912-cbd571797ab1")
        Name = "UPDATE"
        Description = "it's a test 2"
        CreatedAt = now
        IsImportant = false
        When =
          { Year = now.Year
            Month = now.Month
            Day = now.Day }
        SoftDeleted = false }
    let notUpdatableEvent = 
      { Id = Guid.Parse("466491f7-cf50-420c-871f-3669d4f8e6ea")
        Name = "TRY TO UPDATE"
        Description = "it's a test 2"
        CreatedAt = now
        IsImportant = false
        When =
          { Year = now.Year
            Month = now.Month
            Day = now.Day }
        SoftDeleted = false }
    
    let updatedResult = Event.update updatableEvent events
    let notUpdatedResult = Event.update notUpdatableEvent events

    Assert.AreEqual(6, updatedResult.Length)
    Assert.AreEqual(6, notUpdatedResult.Length)

    Assert.AreEqual("UPDATE", updatedResult[0].Name)
    Assert.AreEqual("very old 1", updatedResult[1].Name)
    Assert.AreEqual("old 1", updatedResult[2].Name)
    Assert.AreEqual("deleted 1", updatedResult[3].Name)
    Assert.AreEqual("not so old 2", updatedResult[4].Name)
    Assert.AreEqual("important next year 1", updatedResult[5].Name)

    Assert.AreEqual("test 2", notUpdatedResult[0].Name)
    Assert.AreEqual("very old 1", updatedResult[1].Name)
    Assert.AreEqual("old 1", updatedResult[2].Name)
    Assert.AreEqual("deleted 1", updatedResult[3].Name)
    Assert.AreEqual("not so old 2", updatedResult[4].Name)
    Assert.AreEqual("important next year 1", updatedResult[5].Name)

[<Test>]
let ``nextEvents should return proper Events`` () =
    let result = Event.nextEvents now events

    // The Events previous today and SoftDeleted should be removed
    Assert.AreEqual(2, result.Length)

    // Ordered by When with the upcoming one on top
    Assert.AreEqual("test 2", result[0].Name)
    Assert.AreEqual("important next year 1", result[1].Name)
