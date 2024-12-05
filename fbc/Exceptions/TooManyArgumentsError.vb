Public Class TooManyArgumentsError
    Inherits DisplayableError
    Public Overrides Sub Display()
        Console.ForegroundColor = ConsoleColor.Red
        Console.WriteLine("Too many arguments")
        Console.ResetColor()
    End Sub

End Class
