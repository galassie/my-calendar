module ToDoTests

open System
open MyCalendar
open NUnit.Framework

let now = DateTime.Parse("2023-03-12")

let todos: ToDo array =
    [| { Id = Guid.Parse("6a3abf48-9e1b-4ebb-a912-cbd571797ab1")
         Name = "test 2"
         Description = "it's a test 2"
         CreatedAt = now
         MarkedDoneAt = None
         SoftDeleted = false }
       { Id = Guid.Parse("6bd86c63-350b-4210-a767-43a08fc9a6b0")
         Name = "done very old 1"
         Description = "this is done and very old 1"
         CreatedAt = now.AddDays(-6)
         MarkedDoneAt = Some(now.AddDays(-5))
         SoftDeleted = false }
       { Id = Guid.Parse("c43830d5-35b0-45a8-812a-28d9dc3f7b7d")
         Name = "done 1"
         Description = "done 1"
         CreatedAt = now.AddDays(-3)
         MarkedDoneAt = Some(now.AddDays(-2))
         SoftDeleted = false }
       { Id = Guid.Parse("137a7ba3-9331-4501-a15b-73bb0c9ee38d")
         Name = "deleted 1"
         Description = "deleted 1"
         CreatedAt = now
         MarkedDoneAt = None
         SoftDeleted = true }
       { Id = Guid.Parse("46918e99-2704-411a-b2c0-eee80bf060ab")
         Name = "done 2"
         Description = "done 2"
         CreatedAt = now.AddDays(-2)
         MarkedDoneAt = Some(now.AddDays(-1))
         SoftDeleted = false }
       { Id = Guid.Parse("d86ebde1-c3ab-4d34-a70d-7effd12a2988")
         Name = "test 1"
         Description = "test 1"
         CreatedAt = now.AddDays(-1)
         MarkedDoneAt = None
         SoftDeleted = false } |]


[<Test>]
let ``ToString should return proper stringify ToDo`` () =
    let result = Array.map (fun td -> td.ToString()) todos

    Assert.AreEqual(6, result.Length)

    Assert.AreEqual("test 2: it's a test 2", result[0])
    Assert.AreEqual("done very old 1: this is done and very old 1", result[1])
    Assert.AreEqual("done 1: done 1", result[2])
    Assert.AreEqual("deleted 1: deleted 1", result[3])
    Assert.AreEqual("done 2: done 2", result[4])
    Assert.AreEqual("test 1: test 1", result[5])

[<Test>]
let ``update should return updated ToDo array if ToDo is found, based on Id, or return the same version`` () =
    let updatableToDo =
        { Id = Guid.Parse("6a3abf48-9e1b-4ebb-a912-cbd571797ab1")
          Name = "UPDATE"
          Description = "it's a test 2"
          CreatedAt = now
          MarkedDoneAt = None
          SoftDeleted = false }

    let notUpdatableToDo =
        { Id = Guid.Parse("466491f7-cf50-420c-871f-3669d4f8e6ea")
          Name = "TRY TO UPDATE"
          Description = "it's a test 2"
          CreatedAt = now
          MarkedDoneAt = None
          SoftDeleted = false }

    let updatedResult = ToDo.update updatableToDo todos
    let notUpdatedResult = ToDo.update notUpdatableToDo todos

    Assert.AreEqual(6, updatedResult.Length)
    Assert.AreEqual(6, notUpdatedResult.Length)

    Assert.AreEqual("UPDATE", updatedResult[0].Name)
    Assert.AreEqual("done very old 1", updatedResult[1].Name)
    Assert.AreEqual("done 1", updatedResult[2].Name)
    Assert.AreEqual("deleted 1", updatedResult[3].Name)
    Assert.AreEqual("done 2", updatedResult[4].Name)
    Assert.AreEqual("test 1", updatedResult[5].Name)

    Assert.AreEqual("test 2", notUpdatedResult[0].Name)
    Assert.AreEqual("done very old 1", notUpdatedResult[1].Name)
    Assert.AreEqual("done 1", notUpdatedResult[2].Name)
    Assert.AreEqual("deleted 1", notUpdatedResult[3].Name)
    Assert.AreEqual("done 2", notUpdatedResult[4].Name)
    Assert.AreEqual("test 1", notUpdatedResult[5].Name)

[<Test>]
let ``active should return proper ToDos`` () =
    let result = ToDo.active todos

    // The SoftDeleted and the MarkedDone should be removed
    Assert.AreEqual(2, result.Length)

    // Ordered by DateTime with the most recently created on top
    Assert.AreEqual("test 2", result[0].Name)
    Assert.AreEqual("test 1", result[1].Name)

[<Test>]
let ``markedDone should return proper ToDos`` () =
    let result = ToDo.markedDone todos

    // Only the MarkedDone should be returned
    Assert.AreEqual(3, result.Length)

    // Ordered by DateTime with the most recently created on top
    Assert.AreEqual("done 2", result[0].Name)
    Assert.AreEqual("done 1", result[1].Name)
    Assert.AreEqual("done very old 1", result[2].Name)

[<Test>]
let ``deletable should return proper ToDos`` () =
    let result = ToDo.deletable todos

    // Only the not SoftDelete should be returned
    Assert.AreEqual(5, result.Length)

    // Ordered by DateTime with the most recently created on top
    Assert.AreEqual("test 2", result[0].Name)
    Assert.AreEqual("test 1", result[1].Name)
    Assert.AreEqual("done 2", result[2].Name)
    Assert.AreEqual("done 1", result[3].Name)
    Assert.AreEqual("done very old 1", result[4].Name)

[<Test>]
let ``getViewable should return proper ToDos`` () =
    let result = ToDo.getViewable now todos

    // The SoftDeleted and the MarkedDone older than 5 days should be removed
    Assert.AreEqual(4, result.Length)

    // The ToDos not MarkedDone should be on top and ordered by DateTime with the most recently created on top
    Assert.AreEqual("test 2", result[0].Name)
    Assert.AreEqual("test 1", result[1].Name)
    // The MarkedDones should be the last, ordered by DateTime with the most recently created on top
    Assert.AreEqual("done 2", result[2].Name)
    Assert.AreEqual("done 1", result[3].Name)
