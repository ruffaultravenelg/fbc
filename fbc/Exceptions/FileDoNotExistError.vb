Public Class FileDoNotExistError
    Inherits DisplayableError

    Private Filename As String
    Public Sub New(Filename As String)
        Me.Filename = Filename
    End Sub
    Public Overrides Sub Display()
        Console.ForegroundColor = ConsoleColor.Red
        Console.WriteLine("ERROR: File do not exist")
        Console.ResetColor()
        Console.WriteLine("The file """ & Filename & """ do not exist")
    End Sub
End Class
