# MyCalendar

![Showcase](https://raw.githubusercontent.com/galassie/my-calendar/main/assets/Showcase.png)

Simple calendar application that just fits into your terminal.

Built using:
- [Argu](https://github.com/fsprojects/Argu)
- [FSharp.Json](https://github.com/fsprojects/FSharp.Json)
- [FsSpectre](https://github.com/galassie/fs-spectre)

## Install

```shell
dotnet tool install --global MyCalendar --version 0.1.1
```

## Update

```shell
dotnet tool update -g MyCalendar
```

## Commands

``` shell
# Add a ToDo element
$ my-calendar todo add

# Edit a ToDo element
$ my-calendar todo edit

# Mark as Done a ToDo element
$ my-calendar todo done

# Remove Done mark to a ToDo element
$ my-calendar todo undone

# Delete a ToDo element
$ my-calendar todo delete


# Add an Event element
$ my-calendar event add

# Edit an Event element
$ my-calendar event edit

# Delete an Event element
$ my-calendar event delete


# Add a RecurringEvent element
$ my-calendar recurring-event add

# Edit a RecurringEvent element
$ my-calendar recurring-event edit

# Delete a RecurringEvent element
$ my-calendar recurring-event delete
```
## Contributing

Code contributions are more than welcome! 😻

Please commit any pull requests against the `main` branch.  
If you find any issue, please [report it](https://github.com/galassie/my-calendar/issues)!

## License

This project is licensed under [The MIT License (MIT)](https://raw.githubusercontent.com/galassie/my-calendar/master/LICENSE.md).

Author: [Enrico Galassi](https://twitter.com/enricogalassi88)