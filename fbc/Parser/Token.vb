Imports System.Globalization

Public Class Token
    Implements ILocated

    Public ReadOnly Property Location As Location Implements ILocated.Location
        Get
            If ParentLine Is Nothing Then
                Throw New NotSupportedException("Parent line is not set")
            End If
            Return New Location(ParentLine.Location.Filename, ParentLine.Location.Line, Column)
        End Get
    End Property

    Private ParentLine As TokenLine = Nothing
    Public Sub SetParent(Parent As TokenLine)
        Me.ParentLine = Parent
    End Sub

    Public ReadOnly Property Column As Integer
    Public ReadOnly Property Type As TokenType
    Public ReadOnly Property Value As Object

    Private Sub New(Column As Integer, Type As TokenType, Optional Value As Object = Nothing)
        Me.Column = Column
        Me.Type = Type
        Me.Value = Value
    End Sub

    Public Enum TokenType
        TOK_DEF     ' def
        TOK_MOV     ' mov
        TOK_RET     ' ret
        TOK_COMMA   ' ,
        TOK_WORD    ' word
        TOK_CHAR    ' 'h'
        TOK_INT     ' 123
        TOK_REG     ' $4
        TOK_RESULT  ' ?ret
        TOK_COLON   ' :
    End Enum

    Public Shared Function Tokenize(Line As String, LocationFromColumn As Func(Of Integer, Location)) As IEnumerable(Of Token)

        'Create result
        Dim Result As New List(Of Token)

        'Loop trough line
        Dim Column As Integer = 0
        While Column < Line.Length

            'Get current character
            Dim CurrentChar As Char = Line(Column)

            'Check if it's a space
            If Char.IsWhiteSpace(CurrentChar) Then
                Column += 1
                Continue While
            End If

            'Check if it's a word
            If Char.IsLetter(CurrentChar) Then

                'Get word
                Dim Word As String = ""
                While Column < Line.Length AndAlso Char.IsLetterOrDigit(Line(Column))
                    Word &= Line(Column)
                    Column += 1
                End While

                'Check if it's a keyword
                Select Case Word.ToLower()
                    Case "def"
                        Result.Add(New Token(Column, TokenType.TOK_DEF))
                    Case "ret"
                        Result.Add(New Token(Column, TokenType.TOK_RET))
                    Case "mov"
                        Result.Add(New Token(Column, TokenType.TOK_MOV))
                    Case Else
                        Result.Add(New Token(Column, TokenType.TOK_WORD, Word))
                End Select

                Continue While

            End If

            'Check if it's a number
            If Char.IsDigit(CurrentChar) Then

                'Get number
                Dim Number As String = ""
                While Column < Line.Length AndAlso Char.IsDigit(Line(Column))
                    Number &= Line(Column)
                    Column += 1
                End While

                Result.Add(New Token(Column, TokenType.TOK_INT, Integer.Parse(Number, CultureInfo.InvariantCulture)))
                Continue While

            End If

            'Check if it's a register
            If CurrentChar = "$" Then

                'Get register
                Dim Register As String = ""
                Column += 1
                While Column < Line.Length AndAlso Char.IsDigit(Line(Column))
                    Register &= Line(Column)
                    Column += 1
                End While

                Result.Add(New Token(Column, TokenType.TOK_REG, Integer.Parse(Register, CultureInfo.InvariantCulture)))
                Continue While

            End If

            'Check if it's a character
            If CurrentChar = "'" Then

                'Get character
                Dim Character As Char = Line(Column + 1)
                Result.Add(New Token(Column, TokenType.TOK_CHAR, Character))

                Column += 3
                Continue While

            End If

            'Check if it's a comma
            If CurrentChar = "," Then
                Result.Add(New Token(Column, TokenType.TOK_COMMA))
                Column += 1
                Continue While
            End If

            'Check if it's a colon
            If CurrentChar = ":" Then
                Result.Add(New Token(Column, TokenType.TOK_COLON))
                Column += 1
                Continue While
            End If

            'Check if this is the word '?ret'
            If Line.Substring(Column).StartsWith("?ret") Then
                Result.Add(New Token(Column, TokenType.TOK_RESULT))
                Column += 4
                Continue While
            End If

            'Throw error
            Throw New SyntaxError(LocationFromColumn(Column), $"Invalid character '{CurrentChar}' at column {Column}")

        End While

        'Return result
        Return Result

    End Function

End Class
