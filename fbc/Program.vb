Module Program

    'Main input
    '   fbc <input>
    Private Input As String

    'Main output
    '   fbc <input> <output>
    Private Output As String

    'Flags
    Public Flags As New Dictionary(Of String, Boolean) From {
        {"-r", False},
        {"-h", False},
        {"-v", False}
    }

    'Main
    Sub Main(args As String())

        Try

            'Handle arguments
            For Each Arg As String In args
                HandleArgument(Arg)
            Next

            'Help
            If Flags("-h") Then
                DisplayHelp()
                End
            End If

            'Version
            If Flags("-v") Then
                DisplayVersion()
                End
            End If

            'Read
            If Flags("-r") Then
                ReadFile()
                End
            End If

            'Compile
            CompileFile()
            End

        Catch ex As DisplayableError

            'Display error
            ex.Display()
            End

#If Not DEBUG Then
        Catch ex As Exception

            'Display error
            Console.WriteLine("An error occured: " & ex.Message)
            End

#End If
        End Try

    End Sub

    ' Handle argument
    Private Sub HandleArgument(Arg As String)

        'Flag
        If Arg.StartsWith("-") Then

            'Flag doesn't exist
            If Not Flags.ContainsKey(Arg) Then
                Console.WriteLine("Unknown flag """ & Arg & """")
                End
            End If

            'Set flag to true
            Flags(Arg) = True

            'Stop
            Return

        End If

        'Input
        If Input = "" Then
            Input = Arg
            Return
        End If

        'Output
        If Output = "" Then
            Output = Arg
            Return
        End If

        'Nothing to do
        Throw New TooManyArgumentsError()

    End Sub

    'Display help
    Private Sub DisplayHelp()
        Console.WriteLine("Usage: fbc <file> <output> [flags...]")
        Console.WriteLine()
        Console.WriteLine("<file>:" & vbTab & "Targeted file")
        Console.WriteLine("<output>:" & vbTab & "Output file for compiling")
        Console.WriteLine()
        Console.WriteLine("[flags...]:" & vbTab & "List of flags")
        For Each flag As String In Flags.Keys
            Console.WriteLine(vbTab & flag)
        Next
    End Sub

    'Display version
    Private Sub DisplayVersion()
        Console.WriteLine("FlowByte Compiler (fbc) version " & Config.Version.ToString())
    End Sub

    'Read file
    Private Sub ReadFile()

        'No input file
        If Input = "" Then
            Console.WriteLine("No input file specified")
            End
        End If

        'Read the file
        Dim File As FlowByte.File = FlowByte.File.LoadBinary(Input)

        'Write the file to the string
        Console.WriteLine(File)

    End Sub

    'Compile file
    Private Sub CompileFile()

        'No input file
        If Input = "" Then
            Console.WriteLine("No input file specified")
            End
        End If

        'Compile the file
        Dim File As FlowByte.File = FlowByte.File.LoadText(Input)

        'Check destination
        Dim Destination As String = Output
        If Destination = "" Then
            Destination = Input & ".bin"
        End If

        'Write file to the destination
        File.Build(Destination)

    End Sub

End Module
