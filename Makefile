# Run the show command
show:
	dotnet run --project src/MyCalendar.fsproj show

# Run the todo add command
todo-add:
	dotnet run --project src/MyCalendar.fsproj todo add

# Run the todo edit command
todo-edit:
	dotnet run --project src/MyCalendar.fsproj todo edit

# Run the todo done command
todo-done:
	dotnet run --project src/MyCalendar.fsproj todo done

# Run the todo undone command
todo-undone:
	dotnet run --project src/MyCalendar.fsproj todo undone

# Run the todo delete command
todo-delete:
	dotnet run --project src/MyCalendar.fsproj todo delete

# Run the event add command
event-add:
	dotnet run --project src/MyCalendar.fsproj event add

# Run the event edit command
event-edit:
	dotnet run --project src/MyCalendar.fsproj event edit

# Run the event delete command
event-delete:
	dotnet run --project src/MyCalendar.fsproj event delete

# Format all the project
format:
	dotnet tool run fantomas . -r