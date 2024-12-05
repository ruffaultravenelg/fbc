Imports fbc.Token

Public Class Parser

    'Properties
    Private File As FlowByte.File
    Private TokenLines As IEnumerable(Of TokenLine)
    Private Lines As IteratorAdapter(Of TokenLine)
    Private Tokens As IteratorAdapter(Of Token)

    'Constructor
    Private Sub New(File As FlowByte.File, Tokens As IEnumerable(Of TokenLine))
        Me.File = File
        Me.TokenLines = Tokens
        Me.Lines = New IteratorAdapter(Of TokenLine)(Me.TokenLines)
    End Sub

    'Advance to the next token
    Private Sub EnterLine()
        Tokens = New IteratorAdapter(Of Token)(Lines.Current.Tokens)
    End Sub

    'Parse
    Private Sub Parse()
        While Lines.HasNext
            Dim Fn As FlowByte.Function = GetFunction()
            File.AppendFunction(Fn)
        End While
    End Sub

    'Parse value
    Private Function GetValue() As FlowByte.Value

        'Save token to advance
        Dim Tok As Token = Tokens.Current

        'If token is a register
        If Tokens.Current.Type = TokenType.TOK_REG Then
            Tokens.NextIfPossible()
            Return New FlowByte.Value(FlowByte.ArgumentType.ARG_REG, Convert.ToUInt32(Tok.Value))

        End If

        'If token is a number
        If Tokens.Current.Type = TokenType.TOK_INT Then
            Tokens.NextIfPossible()
            Return New FlowByte.Value(FlowByte.ArgumentType.ARG_INT, Convert.ToUInt32(Tok.Value))

        End If

        'If token is a result value
        If Tokens.Current.Type = TokenType.TOK_RESULT Then
            Tokens.NextIfPossible()
            Return New FlowByte.Value(FlowByte.ArgumentType.ARG_RET)
        End If

        'Throw error
        Throw New SyntaxError(Tokens.Current.Location, "A value was expected")

    End Function

    'Parse line
    Private Function GetInstruction()

        'Enter line
        EnterLine()

        'Parse mov
        If Tokens.Current.Type = TokenType.TOK_MOV Then

            'Get register number
            Tokens.Next()
            Dim Source As FlowByte.Value = GetValue()

            'Get comma
            If Not Tokens.Current.Type = TokenType.TOK_COMMA Then
                Throw New SyntaxError(Tokens.Current.Location, "Expected a comma")
            End If
            Tokens.Next()

            'Get new value
            Dim Destination As FlowByte.Value = GetValue()

            'Return instruction
            Return New FlowByte.MovInstruction(Source, Destination)

        End If

        'Error -> unknown instruction
        Throw New SyntaxError(Tokens.Current.Location, "Expected a valid instruction name")

    End Function

    'Parse bloc
    Private Function GetBloc() As IEnumerable(Of FlowByte.Instruction)

        'Get return indentation level
        Dim ReturnIndentationLevel As Integer = Lines.Current.IndentationLever

        'Create result
        Dim Result As New List(Of FlowByte.Instruction)

        'Get each lines
        While Lines.HasNext

            'Get next line
            Lines.Next()

            'Check line indentation
            If Lines.Current.IndentationLever <= ReturnIndentationLevel Then
                Exit While
            End If

            'Parse line
            Result.Add(GetInstruction())

        End While

        'Return result
        Return Result

    End Function

    'Parse function
    Private Function GetFunction() As FlowByte.Function

        'Check identation
        If Lines.Current.IndentationLever <> 0 Then
            Throw New SyntaxError(Lines.Current.Location, "A function cannot be indented")
        End If

        'Get "def" keyword
        EnterLine()
        If Tokens.Current.Type <> TokenType.TOK_DEF Then
            Throw New SyntaxError(Tokens.Current.Location, "Keyword 'def' was expected")
        End If

        'Get function name
        Tokens.Next()
        If Tokens.Current.Type <> TokenType.TOK_WORD Then
            Throw New SyntaxError(Tokens.Current.Location, "Function name was expected")
        End If
        Dim Name As String = Tokens.Current.Value

        'Get argument count
        Dim ArgumentCount As Integer = 0
        If Tokens.HasNext Then
            Tokens.Next()
            If Tokens.Current.Type <> TokenType.TOK_INT Then
                Throw New SyntaxError(Tokens.Current.Location, "Expected a integer")
            End If
        End If

        'Create function
        Dim Fn As New FlowByte.Function(Name, ArgumentCount)

        'Parse bloc
        Dim Content As IEnumerable(Of FlowByte.Instruction) = GetBloc()
        For Each Instruction As FlowByte.Instruction In Content
            Fn.AppendInstruction(Instruction)
        Next

        'Return function
        Return Fn

    End Function

    'Parse
    Public Shared Sub Parse(File As FlowByte.File, Tokens As IEnumerable(Of TokenLine))

        'Create & use parser
        Dim Parser As New Parser(File, Tokens)

        'If there is at least one line
        If Tokens.Count > 0 Then
            Parser.Parse()
        End If

    End Sub

End Class
