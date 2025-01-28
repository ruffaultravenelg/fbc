Imports System.Data
Imports fbc.Token

Public Class Parser

    'Properties
    Private ReadOnly File As FlowByte.File
    Private ReadOnly Lines As IteratorAdapter(Of TokenLine)
    Private Tokens As IteratorAdapter(Of Token)

    'Instructions forms
    Private ReadOnly InstructionArgumentCount As New Dictionary(Of TokenType, (ArgumentCount As Integer, Instruction As FlowByte.InstructionType)) From {
        {FlowByte.InstructionType.INST_MOV, (2, FlowByte.InstructionType.INST_MOV)}
    }

    'Constructor
    Private Sub New(File As FlowByte.File, TokenLines As IEnumerable(Of TokenLine))
        Me.File = File
        Me.Lines = New IteratorAdapter(Of TokenLine)(TokenLines)
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

        'If token is a label name
        If Tokens.Current.Type = TokenType.TOK_WORD Then
            Tokens.NextIfPossible()
            Return New FlowByte.LabelValue(Tok.Location, Tok.Value)
        End If

        'Throw error
        Throw New SyntaxError(Tokens.Current.Location, "A value was expected")

    End Function

    'Parse instruction
    Private Function GetInstruction()

        'Enter line
        EnterLine()

        'Get instruction
        If Not InstructionArgumentCount.ContainsKey(Tokens.Current.Type) Then
            Throw New SyntaxError(Tokens.Current.Location, "Unknown instruction")
        End If

        'Get values
        Dim InstructionInformations As (ArgumentCount As Integer, Instruction As FlowByte.InstructionType) = InstructionArgumentCount(Tokens.Current.Type)

        'Get arguments
        Dim Argument1 As FlowByte.Value = Nothing
        Dim Argument2 As FlowByte.Value = Nothing
        If InstructionInformations.ArgumentCount > 0 Then

            'Get first arguments
            If Not Tokens.HasNext Then
                Throw New SyntaxError(Tokens.Current.Location, "A value was expected")
            End If
            Tokens.Next()

            Argument1 = GetValue()

            'Get second argument
            If InstructionInformations.ArgumentCount > 1 Then

                'Check comma
                If Not Tokens.Current.Type = TokenType.TOK_COMMA Then
                    Throw New SyntaxError(Tokens.Current.Location, "A comma was expected for a second argument")
                End If

                'Get second argument
                If Not Tokens.HasNext Then
                    Throw New SyntaxError(Tokens.Current.Location, "A value was expected for the second argument")
                End If
                Tokens.Next()

                Argument2 = GetValue()

            End If

        End If

        'Return object
        Return New FlowByte.Instruction(InstructionInformations.Instruction, Argument1, Argument2)

    End Function

    'Parse line
    Private Sub ParseLine(Fn As FlowByte.Function)

        'If label
        If Tokens.Current.Type = TokenType.TOK_COLON Then

            'If there is nothing next to the colon
            If Not Tokens.HasNext Then
                Throw New SyntaxError(Tokens.Current.Location, "Expected a label name after the colon.")
            End If

            'If this is not a word
            Tokens.Next()
            If Not Tokens.Current.Type = TokenType.TOK_WORD Then
                Throw New SyntaxError(Tokens.Current.Location, "Expected a valid label name.")
            End If

            'Create label
            Fn.AppendLabel(Tokens.Current.Location, Tokens.Current.Value)

            'Exit
            Exit Sub

        End If

        'Else -> search instruction
        Fn.AppendInstruction(GetInstruction())

    End Sub

    'Parse function
    Private Function GetFunction() As FlowByte.Function

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

        'Parse function body
        While Lines.HasNext

            'Goto next line
            Lines.Next()
            EnterLine()

            'Check if the line defines a new functions
            If Tokens.Current.Type = TokenType.TOK_DEF Then
                Exit While
            End If

            'Parse it
            ParseLine(Fn)

            'If the line is not empty -> error
            If Tokens.HasNext Then
                Tokens.Next()
                Throw New SyntaxError(Tokens.Current.Location, "This token was unexpected and not needed.")
            End If

        End While

        'Resolve label names
        Fn.Resolve()

        'Return function
        Return Fn

    End Function

    'Parse
    Public Shared Sub Parse(File As FlowByte.File, Tokens As IEnumerable(Of TokenLine))

        'If there is at least one line
        If Tokens.Count > 0 Then
            Dim Parser As New Parser(File, Tokens)
            Parser.Parse()
        End If

    End Sub

End Class
