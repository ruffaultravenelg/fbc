Imports System.Diagnostics.CodeAnalysis
Imports System.Diagnostics.Eventing
Imports System.IO
Imports System.Text

Namespace FlowByte
    Public Class File

        'List of functions
        Private Functions As IDictionary(Of String, FlowByte.Function)

        'Constructor
        Private Sub New()
            Me.Functions = New Dictionary(Of String, FlowByte.Function)
        End Sub

        'Append function
        Public Sub AppendFunction(Fun As FlowByte.Function)
            Try
                Me.Functions.Add(Fun.Name, Fun)
            Catch ex As ArgumentException
                Throw New SyntaxError(Nothing, "A function nammed """ & Fun.Name & """ already exists")
            End Try
        End Sub

        'Build the file -> serialize content
        Public Sub Build(Path As String)

            'Create writer
            Dim Stream As New FileStream(Path, FileMode.Create)
            Dim Writer As New BinaryWriter(Stream)

            'Write version
            Writer.Write(Convert.ToDouble(Config.Version))

            'Write function count
            Writer.Write(Convert.ToUInt32(Functions.Count))

            'Write functions
            For Each Fun As FlowByte.ISerializable In Functions.Values
                Fun.Serialize(Writer)
            Next

            'Close writer
            Writer.Close()
            Stream.Close()

        End Sub

        'Load the file -> deserialize content
        Public Shared Function LoadBinary(Path As String) As FlowByte.File

            'File do not exist
            If Not IO.File.Exists(Path) Then
                Throw New FileDoNotExistError(Path)
            End If

            'Create reader
            Dim Stream As New FileStream(Path, FileMode.Open)
            Dim Reader As New BinaryReader(Stream)

            'Read version
            Dim Version As Double = Reader.ReadDouble()
            If Version <> Config.Version Then
                Throw New Exception("Invalid version")
            End If

            'Create file
            Dim File As New FlowByte.File()

            'Read function count
            Dim FunctionsCount As UInt32 = Reader.ReadUInt32()

            'Read each functions
            For i As Integer = 0 To FunctionsCount - 1
                File.AppendFunction(FlowByte.Function.Deserialize(Reader))
            Next

            'Close reader
            Reader.Close()
            Stream.Close()

            'Return file
            Return File

        End Function

        ' Load the file -> parse the string
        Public Shared Function LoadText(Path As String) As FlowByte.File

            'File do not exist
            If Not IO.File.Exists(Path) Then
                Throw New FileDoNotExistError(Path)
            End If

            'Create file
            Dim File As New FlowByte.File()

            'Tokenize file
            Dim Lines As IEnumerable(Of TokenLine) = TokenLine.TokenizeFile(Path)

            'Parse token
            Parser.Parse(File, Lines)

            'Return file
            Return File

        End Function

        'To string
        Public Overrides Function ToString() As String

            'Create result
            Dim Result As New StringBuilder()

            'Add each functions
            For Each Fun In Functions.Values
                Result.Append(Fun.ToString())
                Result.Append(Environment.NewLine)
            Next

            'Return result
            Return Result.ToString()

        End Function

    End Class
End Namespace