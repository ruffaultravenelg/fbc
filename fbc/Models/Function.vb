Imports System.IO
Imports System.Text

Namespace FlowByte
    Public Class [Function]
        Implements FlowByte.ISerializable

        'Properties
        Public ReadOnly Property Name As String
        Private ArgsCount As UInt32
        Private Instructions As List(Of FlowByte.Instruction)

        'Constructor
        Public Sub New(Name As String, ArgsCount As UInt32)
            Me.Name = Name
            Me.ArgsCount = ArgsCount
            Me.Instructions = New List(Of FlowByte.Instruction)
        End Sub

        'Append instruction
        Public Sub AppendInstruction(Fun As FlowByte.Instruction)
            Me.Instructions.Add(Fun)
        End Sub

        'Serialize
        Public Sub Serialize(Writer As BinaryWriter) Implements ISerializable.Serialize

            'Write function name
            Writer.Write(Convert.ToUInt32(Name.Length))
            Writer.Write(Name.ToCharArray())

            'Write argument count
            Writer.Write(ArgsCount)

            'Write instructions
            Writer.Write(Convert.ToUInt32(Instructions.Count))
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
                Result.Append(vbTab)
                Result.Append(Instruction.ToString())
            Next

            'Return result
            Return Result.ToString()

        End Function

    End Class

End Namespace