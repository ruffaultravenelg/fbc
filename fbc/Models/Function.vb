Imports System.IO
Imports System.Text

Namespace FlowByte
    Public Class [Function]
        Implements FlowByte.ISerializable

        'Properties
        Public ReadOnly Property Name As String
        Private ReadOnly ArgsCount As UInt32
        Private ReadOnly Instructions As IEnumerable(Of FlowByte.Instruction) = New List(Of FlowByte.Instruction)
        Public ReadOnly Labels As New Dictionary(Of String, UInt32)

        'Constructor
        Public Sub New(Name As String, ArgsCount As UInt32)
            Me.Name = Name
            Me.ArgsCount = ArgsCount
        End Sub

        'Append instruction
        Public Sub AppendInstruction(Fun As FlowByte.Instruction)
            DirectCast(Me.Instructions, List(Of FlowByte.Instruction)).Add(Fun) 'Weird cast but this allow Instruction to be "readonly", cannot append like a list
        End Sub

        'Append label
        Sub AppendLabel(Location As Location, Name As String)

            'Check if the label already exist
            If Labels.ContainsKey(Name) Then
                Throw New SyntaxError(Location, "Label """ & Name & """ already exists")
            End If

            'Add label
            Labels.Add(Name, Convert.ToUInt32(Instructions.Count))

        End Sub

        'Resolve (resolve aspects where the functions needs to be complete like labels)
        Public Sub Resolve()
            For Each Instruction As FlowByte.Instruction In Instructions
                Instruction.Resolve(Me)
            Next
        End Sub

        'Serialize
        Public Sub Serialize(Writer As BinaryWriter) Implements ISerializable.Serialize

            'Write function name
            Writer.Write(Convert.ToUInt32(Name.Length))
            Writer.Write(Name.ToCharArray())

            'Write argument count
            Writer.Write(ArgsCount)

            'Write instruction count
            Writer.Write(Convert.ToUInt32(Instructions.Count))

            'Write instructions
            For Each Instruction As FlowByte.ISerializable In Instructions
                Instruction.Serialize(Writer)
            Next

        End Sub

        'Deserialize
        Public Shared Function Deserialize(Reader As BinaryReader) As FlowByte.Function

            'Read function name
            Dim NameLength As UInt32 = Reader.ReadUInt32()
            Dim Name As String = New String(Reader.ReadChars(NameLength))

            'Read argument count
            Dim ArgsCount As UInt32 = Reader.ReadUInt32()

            'Create function
            Dim Result As New FlowByte.Function(Name, ArgsCount)

            'Read instructions
            Dim InstructionsCount As UInt32 = Reader.ReadUInt32()
            For i As Integer = 0 To InstructionsCount - 1
                Result.AppendInstruction(FlowByte.Instruction.Deserialize(Reader))
            Next

            'Return result
            Return Result

        End Function

        'To string
        Public Overrides Function ToString() As String

            'Create function header
            Dim Result As New StringBuilder()
            Result.Append("def ")
            Result.append(Name)

            'Write instructions
            For Each Instruction In Instructions
                Result.Append(Environment.NewLine)
                Result.Append("    ")
                Result.Append(Instruction.ToString())
            Next

            'Return result
            Return Result.ToString()

        End Function

    End Class

End Namespace