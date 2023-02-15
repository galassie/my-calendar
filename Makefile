# Run the show command
show:
	dotnet run --project src/MyCalendar.fsproj show

# Run the todo add command
todo-add:
	dotnet run --project src/MyCalendar.fsproj todo add

# Format all the project
format:
	dotnet tool run fantomas . -r