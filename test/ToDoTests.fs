module ToDoTests

open MyCalendar
open System
open NUnit.Framework

[<Test>]
let ``extractForView should return proper ToDos`` () = 
    let now  = DateTime.Now
    let todos: ToDo array = [|
        { Name = "test 2"
          Description = "test 2"
          CreatedAt = now
          MarkedDoneAt = None
          SoftDeleted = false }
        { Name = "done very old 1"
          Description = "done very old 1"
          CreatedAt = now.AddDays(-6)
          MarkedDoneAt = Some (now.AddDays(-5))
          SoftDeleted = false }
        { Name = "done 1"
          Description = "done 1"
          CreatedAt = now.AddDays(-3)
          MarkedDoneAt = Some (now.AddDays(-2))
          SoftDeleted = false }
        { Name = "deleted 1"
          Description = "deleted 1"
          CreatedAt = now
          MarkedDoneAt = None
          SoftDeleted = true }
        { Name = "done 2"
          Description = "done 2"
          CreatedAt = now.AddDays(-2)
          MarkedDoneAt = Some (now.AddDays(-1))
          SoftDeleted = false }
        { Name = "test 1"
          Description = "test 1"
          CreatedAt = now.AddDays(-1)
          MarkedDoneAt = None
          SoftDeleted = false }
    |]

    let result = ToDo.extractForView now todos

    // The SoftDeleted and the MarkedDone older than 5 days should be removed
    Assert.AreEqual(4, result.Length)

    // The ToDos not MarkedDone should be on top and ordered by DateTime
    Assert.AreEqual("test 1", result[0].Name) 
    Assert.AreEqual("test 2", result[1].Name)
    // The MarkedDones should be the last, ordered by DateTime
    Assert.AreEqual("done 1", result[2].Name) 
    Assert.AreEqual("done 2", result[3].Name)
