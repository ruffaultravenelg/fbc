Public Class SimpleError
    Inherits DisplayableError

    Private Shadows Message As String

    Public Sub New(Message As String)
        Me.Message = Message
    End Sub

    Public Overrides Sub Display()

        Console.ForegroundColor = ConsoleColor.Red
        Console.WriteLine("ERROR: " & Message)
        Console.ResetColor()

    End Sub

End Class
