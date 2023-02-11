# Run the show command
show:
	dotnet run --project src/MyCalendar.fsproj show

# Format all the project
format:
	dotnet tool run fantomas . -r