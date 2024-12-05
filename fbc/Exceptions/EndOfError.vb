Public Class EndOfError
    Inherits SyntaxError

    'Constructor
    Public Sub New(Elements As IEnumerable(Of ILocated))
        MyBase.New(Elements.Last.Location, "End of file was expected")
    End Sub

End Class
