Namespace FlowByte
    Public Module Types

        'Instruction types
        Public Enum InstructionType
            INST_ARG
            INST_CALL
            INST_INT
            INST_MOV
            INST_RET
            INST_RETVAL
            INST_INC
            INST_DEC
            INST_JMP
            INST_JMPIF
            INST_EQU
            INST_NOT
        End Enum

        'Value types
        Public Enum ArgumentType
            ARG_NULL ' No value
            ARG_INT  ' 42
            ARG_REG  ' $42
            ARG_RET  ' ?ret
            ARG_FUN  ' Function index -> {0, 1, 2} can be changed when importing the file
        End Enum

    End Module

End Namespace