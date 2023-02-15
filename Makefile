# Run the show command
show:
	dotnet run --project src/MyCalendar.fsproj show

# Run the todo add command
todo-add:
	dotnet run --project src/MyCalendar.fsproj todo add

# Run the todo done command
todo-done:
	dotnet run --project src/MyCalendar.fsproj todo done

# Run the todo undone command
todo-undone:
	dotnet run --project src/MyCalendar.fsproj todo undone

# Format all the project
format:
	dotnet tool run fantomas . -r