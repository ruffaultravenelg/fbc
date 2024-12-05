Public Class SyntaxError
    Inherits DisplayableError

    Private Text As String
    Private Location As Location

    Public Sub New(Location As Location, Text As String)
        Me.Location = Location
        Me.Text = Text
    End Sub

    Public Overrides Sub Display()

        'Display error title
        Console.ForegroundColor = ConsoleColor.Red
        Console.WriteLine("SYNTAX ERROR:")

        'Display error message
        Console.ResetColor()
        Console.WriteLine(Text)

        'Display location
        Console.ForegroundColor = ConsoleColor.Red
        Console.WriteLine(Location)

        'Reset color
        Console.ResetColor()

    End Sub

End Class
