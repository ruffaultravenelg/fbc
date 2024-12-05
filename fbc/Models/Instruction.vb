Imports System.IO

Namespace FlowByte
    Public MustInherit Class Instruction
        Implements FlowByte.ISerializable

        'Type
        Protected MustOverride ReadOnly Property InstructionType As FlowByte.InstructionType

        'Serialize
        Private Sub CommonSerialization(Writer As BinaryWriter) Implements ISerializable.Serialize

            'Write instruction type
            Dim InstructionTypeConverted As UInt32 = InstructionType
            Writer.Write(InstructionTypeConverted)

            'Write instruction content
            Serialize(Writer)

        End Sub
        Public MustOverride Sub Serialize(Writer As BinaryWriter)

        'To string
        Public Overrides Function ToString() As String
            Throw New NotImplementedException()
        End Function

        'Deserialize
        Public Overloads Shared Function Deserialize(reader As BinaryReader) As Instruction

            'Read instruction type
            Dim InstructionType As FlowByte.InstructionType = reader.ReadUInt32()

            'Deserialize instruction
            Select Case InstructionType

                Case FlowByte.InstructionType.INST_CALL
                    Return CallInstruction.Deserialize(reader)

                Case FlowByte.InstructionType.INST_MOV
                    Return MovInstruction.Deserialize(reader)

                Case Else
                    Throw New NotImplementedException()

            End Select

        End Function

    End Class

    Public Enum InstructionType
        INST_CALL
        INST_MOV
    End Enum

End Namespace