Imports System.IO

Namespace FlowByte
    Public Class Instruction
        Implements FlowByte.ISerializable

        'Type
        Protected ReadOnly Property InstructionType As FlowByte.InstructionType

        'Arguments
        Protected Argument1 As FlowByte.Value
        Protected Argument2 As FlowByte.Value

        'Constructor
        Public Sub New(InstructionType As FlowByte.InstructionType, Optional Argument1 As FlowByte.Value = Nothing, Optional Argument2 As FlowByte.Value = Nothing)
            Me.InstructionType = InstructionType
            Me.Argument1 = Argument1
            Me.Argument2 = Argument2
        End Sub

        'Resolve
        Public Sub Resolve(Fn As FlowByte.Function)
            Argument1?.ResolveLabels(Fn)
            Argument2?.ResolveLabels(Fn)
        End Sub

        'Serialize
        Private Sub CommonSerialization(Writer As BinaryWriter) Implements ISerializable.Serialize

            'Write instruction type
            Dim InstructionTypeConverted As UInt32 = InstructionType
            Writer.Write(InstructionTypeConverted)

            'Write first argument
            FlowByte.Value.Serialize(Writer, Argument1)

            'Write second argument
            FlowByte.Value.Serialize(Writer, Argument2)

        End Sub

        'To string
        Public Overrides Function ToString() As String

            'Create instruction string
            Dim Result As String = InstructionType.ToString()
            Const ENUM_PREFIX As String = "INST_"
            If Result.StartsWith(ENUM_PREFIX) Then
                Result = Result.Substring(ENUM_PREFIX.Length)
            End If
            Result = Result.ToLower()

            'If not argument
            If Argument1 Is Nothing Then
                Return Result
            End If

            'Add argument 1
            Result &= " " & Argument1.ToString()

            'If no other arguments
            If Argument2 Is Nothing Then
                Return Result
            End If

            'Add argument 2
            Result &= ", " & Argument2.ToString()
            Return Result

        End Function

        'Deserialize
        Public Shared Function Deserialize(Reader As BinaryReader) As Instruction

            'Read instruction type
            Dim InstructionType As FlowByte.InstructionType = Reader.ReadUInt32()

            'Deserialize arguments
            Dim Argument1 As FlowByte.Value = FlowByte.Value.Deserialize(Reader)
            Dim Argument2 As FlowByte.Value = FlowByte.Value.Deserialize(Reader)

            'Create instruction
            Return New FlowByte.Instruction(InstructionType, Argument1, Argument2)

        End Function

    End Class

    Public Enum InstructionType
        INST_CALL
        INST_MOV
    End Enum

End Namespace