﻿Imports System.IO

Namespace FlowByte
    Public Class Value
        Implements FlowByte.ISerializable

        'Values
        Public Type As FlowByte.ArgumentType
        Protected Value As Int32

        'Constructor
        Public Sub New(Type As FlowByte.ArgumentType, Optional Value As UInt32 = 0)
            Me.Type = Type
            Me.Value = Value
        End Sub

        'Serialize

        Private Sub Serialize(Writer As BinaryWriter) Implements ISerializable.Serialize

            'Convert and writer type
            Dim TypeInteger As Int32 = Type
            Writer.Write(TypeInteger)

            'Write value
            Writer.Write(Value)

        End Sub

        'Serialize null
        Private Shared Sub SerializeNull(Writer As BinaryWriter)

            'Create constant cause vbnet
            Const NullValue As Int32 = 0

            'Write null type
            Writer.Write(NullValue)

            'Write null value
            Writer.Write(NullValue)

        End Sub

        'Serialize a value
        Public Shared Sub Serialize(Writer As BinaryWriter, Value As FlowByte.Value)
            If Value Is Nothing Then
                FlowByte.Value.SerializeNull(Writer)
            Else
                Value.Serialize(Writer)
            End If
        End Sub

        'Deserialize
        Friend Shared Function Deserialize(reader As BinaryReader) As FlowByte.Value

            'Read type
            Dim Type As FlowByte.ArgumentType = reader.ReadUInt32()

            'Read value
            Dim Value As UInt32 = reader.ReadUInt32()

            'Return value
            Return New FlowByte.Value(Type, Value)

        End Function

        'To string
        Public Overrides Function ToString() As String

            Select Case Type

                Case ArgumentType.ARG_NULL
                    Return ""

                Case ArgumentType.ARG_INT
                    Return Value.ToString()

                Case ArgumentType.ARG_REG
                    Return "$" & Value.ToString()

                Case ArgumentType.ARG_RET
                    Return "?ret"

                Case ArgumentType.ARG_FUN
                    Return "fun_idx:" & Value.ToString()

                Case Else
                    Throw New NotImplementedException()

            End Select

        End Function

    End Class

End Namespace