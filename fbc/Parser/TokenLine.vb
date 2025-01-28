Imports System.IO
Imports System.Text

Public Class TokenLine
    Implements ILocated

    Public ReadOnly Property Location As Location Implements ILocated.Location
    Public ReadOnly Property Tokens As IEnumerable(Of Token)

    'Constructor
    Private Sub New(Filename As String, LinePosition As Integer, Line As String)

        'Set location
        Me.Location = New Location(Filename, LinePosition)

        'Get tokens from the string
        Me.Tokens = Token.Tokenize(Line, Function(Column As Integer) New Location(Filename, LinePosition, Column))

        'Set parent of tokens
        For Each Token As Token In Tokens
            Token.SetParent(Me)
        Next

    End Sub

    ' Tokenize file
    Public Shared Function TokenizeFile(Path As String) As IEnumerable(Of TokenLine)

        'Create reader
        Dim StreamReader As New StreamReader(Path)

        'Get relative path
        Path = IO.Path.GetRelativePath(".", Path)

        ' Read lines
        Dim Lines As New List(Of TokenLine)
        Dim LineNumber As Integer = 0
        Do Until StreamReader.EndOfStream

            'Read line
            Dim Line As String = StreamReader.ReadLine().Trim()

            'Pass if empty or comment
            If Line = "" Or Line.StartsWith(";") Then
                Continue Do
            End If

            'Increment line number
            LineNumber += 1

            'Tokenize line
            Dim Result As New TokenLine(Path, LineNumber, Line)
            If Result.Tokens.Count = 0 Then
                Continue Do
            End If
            Lines.Add(Result)

        Loop

        'Close stream
        StreamReader.Close()

        'Return file
        Return Lines

    End Function

    ' To string
    Public Overrides Function ToString() As String
        Dim Result As New StringBuilder()
        For Each Tok As Token In Tokens
            Result.Append("[")
            Result.Append(Tok.Type.ToString())
            If Tok.Value IsNot Nothing Then
                Result.Append(", ")
                Result.Append(Tok.Value)
            End If
            Result.Append("]")
        Next
        Return Result.ToString()
    End Function


End Class
