Imports System.IO

Namespace FlowByte
    Public Class CallInstruction
        Inherits Instruction

        'Properties
        Private FunctionName As String
        Private PassedArguments As List(Of FlowByte.Value)

        'Constructor
        Public Sub New(FunctionName As String, PassedArguments As List(Of FlowByte.Value))
            Me.FunctionName = FunctionName
            Me.PassedArguments = PassedArguments
        End Sub

        ' Instruction type
        Protected Overrides ReadOnly Property InstructionType As FlowByte.InstructionType = FlowByte.InstructionType.INST_CALL

        'Serialize
        Public Overrides Sub Serialize(Writer As BinaryWriter)

            'Write function name
            Writer.Write(Convert.ToUInt32(FunctionName.Length))
            Writer.Write(FunctionName.ToCharArray())

            'Write passed arguments
            Writer.Write(Convert.ToUInt32(PassedArguments.Count))
            For Each Arg As FlowByte.ISerializable In PassedArguments
                Arg.Serialize(Writer)
            Next

        End Sub

        'Deserialize
        Public Overloads Shared Function Deserialize(reader As BinaryReader) As FlowByte.CallInstruction

            'Read function name
            Dim NameLength As UInt32 = reader.ReadUInt32()
            Dim Name As String = New String(reader.ReadChars(NameLength))

            'Read passed arguments
            Dim ArgsCount As UInt32 = reader.ReadUInt32()
            Dim Args As New List(Of FlowByte.Value)
            For i As Integer = 0 To ArgsCount - 1
                Args.Add(FlowByte.Value.Deserialize(reader))
            Next

            'Return instruction
            Return New FlowByte.CallInstruction(Name, Args)

        End Function

        'To string
        Public Overrides Function ToString() As String
            Return "call " & FunctionName & String.Join(", ", PassedArguments.Select(Function(Value) Value.ToString()))
        End Function

    End Class

End Namespace