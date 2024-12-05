Public Class Location

    Public Property Line As Integer
    Public Property Column As Integer
    Public Property Filename As String

    Public Sub New(Filename As String, Line As Integer, Column As Integer)
        Me.Line = Line
        Me.Column = Column
        Me.Filename = Filename
    End Sub
    Public Sub New(Filename As String, Line As Integer)
        Me.Line = Line
        Me.Column = -1
        Me.Filename = Filename
    End Sub

    Public Overrides Function ToString() As String
        If Column <> -1 Then
            Return "<" & Filename & "> at " & Line & ":" & Column
        Else
            Return "<" & Filename & "> at line " & Line
        End If
    End Function

End Class
