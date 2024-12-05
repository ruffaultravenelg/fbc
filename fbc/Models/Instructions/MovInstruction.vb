Imports System.IO

Namespace FlowByte
    Public Class MovInstruction
        Inherits Instruction

        'Properties
        Public TargetedRegister As FlowByte.Value
        Public NewValue As FlowByte.Value

        'Constructor
        Public Sub New(TargetedRegister As FlowByte.Value, NewValue As FlowByte.Value)
            Me.TargetedRegister = TargetedRegister
            Me.NewValue = NewValue
        End Sub

        ' Instruction type
        Protected Overrides ReadOnly Property InstructionType As FlowByte.InstructionType = FlowByte.InstructionType.INST_MOV

        'Serialize
        Public Overrides Sub Serialize(Writer As BinaryWriter)

            'Write arguments
            TargetedRegister.Serialize(Writer)
            NewValue.Serialize(Writer)

        End Sub

        'Deserialize
        Public Overloads Shared Function Deserialize(reader As BinaryReader) As FlowByte.MovInstruction

            'Read arguments
            Dim TargetedRegister As Value = Value.Deserialize(reader)
            Dim NewValue As Value = Value.Deserialize(reader)

            'Return instruction
            Return New MovInstruction(TargetedRegister, NewValue)

        End Function

        'To string
        Public Overrides Function ToString() As String
            Return "mov " & TargetedRegister.ToString() & ", " & NewValue.ToString()
        End Function

    End Class

End Namespace